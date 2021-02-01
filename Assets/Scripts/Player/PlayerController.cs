﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private Rigidbody2D _playerRb;
    private SpriteRenderer _playerSr;
    private Animator _playerAnim;
    private Collider2D _playerCol;

    [SerializeField] private LayerMask layerMask; //pulo
    [SerializeField] private GameObject _feedbackControl;
    [SerializeField] private Camera _mainCamPlayer;
    [SerializeField] private Color _colorDisabled;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckL = default;
    [SerializeField] private Transform _groundCheckR = default;
    [SerializeField] private float _distance = 1;
    private bool _isGrounded = true;

    [Header("Player Check")]
    private RaycastHit2D _hitPlayerL;
    private RaycastHit2D _hitPlayerR;
    private Collider2D _hitPlayerCollider;
    [SerializeField] private Transform _playerCheckL = default;
    [SerializeField] private Transform _playerCheckR = default;
    [SerializeField] private float _distanceToPlayer = 0.2f;

    [Header("Physics")]
    [SerializeField][Range(1.0f, 10.0f)] private float _speedPlayer = 5.0f;
    [SerializeField][Range(500.0f, 1600.0f)] private float _jumpForce = 500.0f;

    private bool _canChangePlayer = false;
    private bool _canChangeWorld = false;
    private bool _isLookLeft = false;
    #endregion

    #region Properties
    [SerializeField] private bool _canControl = false;
    public bool CanControl
    { 
        get
        {
            return _canControl;
        }

        set
        {
            _canControl = value;
            _mainCamPlayer.gameObject.SetActive(value);       
        }
    }

    [SerializeField] private bool _activeForTheFirstTime = false;
    private bool ActiveForTheFirstTime
    {
        get
        {
            return _activeForTheFirstTime;
        }
        set
        {
            _activeForTheFirstTime = value;
        }
    }
    #endregion

    #region Unity Methods
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSr = GetComponent<SpriteRenderer>();
        _playerAnim = GetComponent<Animator>();
        _playerCol = GetComponent<Collider2D>();        
    }

    private void Start()
    {
        if (ActiveForTheFirstTime)
        {
            GameController.Instance.InitializeListPlayer(this.gameObject);
        }

        if (!CanControl)
        {
            _playerSr.color = _colorDisabled;
        }     
    }
    
    void Update()
    {
        if (!CanControl) { return ; }

        Movement();
        Jump();
        ChangeWorld();
        HitPlayer();
        ChangePlayer();
    }

    private void FixedUpdate()
    {
        if (!CanControl) { return ; }

        _isGrounded = Physics2D.Raycast(_groundCheckL.position, Vector2.down, _distance, layerMask)
        || Physics2D.Raycast(_groundCheckR.position, Vector2.down, _distance, layerMask);

        _hitPlayerL = Physics2D.Raycast(_playerCheckL.position, Vector2.left, _distanceToPlayer, 1 << LayerMask.NameToLayer("Player"));
        _hitPlayerR = Physics2D.Raycast(_playerCheckR.position, Vector2.right, _distanceToPlayer, 1 << LayerMask.NameToLayer("Player"));
    
        _playerAnim.SetFloat("speedY", _playerRb.velocity.y);
    }
    #endregion

    #region Basic
    private void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        transform.Translate(Vector3.right * _speedPlayer * horizontalInput * Time.deltaTime);

        _playerAnim.SetBool("isGrounded", _isGrounded);
        _playerAnim.SetInteger("speedX", (int)horizontalInput);

        if(horizontalInput > 0 && _isLookLeft)
        {
            Flip();

        }
        else if(horizontalInput < 0 && !_isLookLeft)
        {
            Flip();    
        }

    }

    private void Flip()
    {
        _isLookLeft = !_isLookLeft;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _playerRb.AddForce(Vector2.up * _jumpForce);
        }
    }
    #endregion

    #region Change World and Player
    private void ChangeWorld()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {    
            if (GameController.Instance.IsInvertedWorldActive)
            {
                GameController.Instance.ActiveWorld(this.gameObject, false); 
            }
            else
            {
                GameController.Instance.ActiveWorld(this.gameObject, true);
            }
        }    
    }

    private void ChangePlayer()
    {
        if (!_canChangePlayer) { return ; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameController.Instance.IsInvertedWorldActive)
            {
                GameController.Instance.ChangeWorld();
            }

            _canChangePlayer = false;
                            
            GameController.Instance.ActiveWorld(this.gameObject, false);

            CanControl = false;
            _feedbackControl.SetActive(false);

            _hitPlayerCollider.gameObject.TryGetComponent(out PlayerController otherPlayer);
            otherPlayer.CanControl = true;
            otherPlayer._feedbackControl.SetActive(true);
        }
    }
    #endregion

    #region  Hit Player
    private void HitPlayer()
    {        
        if (_hitPlayerL.collider != null && _hitPlayerL.collider.gameObject.CompareTag("Player") && CanControl)
        {
            PlayerDetected(_hitPlayerL.collider);
        }
        else if (_hitPlayerR.collider != null && _hitPlayerR.collider.gameObject.CompareTag("Player") && CanControl)
        {
            PlayerDetected(_hitPlayerR.collider);
        }    
    }

    private void PlayerDetected(Collider2D col)
    {
        //Quando encostar no outro player ele nao vai poder mais andar, e o mundo vai para o mundo normal de ambos
        _canChangePlayer = true;
        _hitPlayerCollider = col;

        col.TryGetComponent(out PlayerController _otherPlayer);
        _otherPlayer.GetComponent<SpriteRenderer>().color = Color.white;

        if (!_otherPlayer.ActiveForTheFirstTime)
        {
            _otherPlayer.ActiveForTheFirstTime = true;
            GameController.Instance.InitializeListPlayer(_otherPlayer.gameObject);
        }
    }

    private void DrawRayChecks()
    {
        Debug.DrawRay(_groundCheckL.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);
        Debug.DrawRay(_groundCheckR.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);

        Debug.DrawRay(_playerCheckL.position, Vector2.left * _distanceToPlayer, _hitPlayerL ? Color.green : Color.red);
        Debug.DrawRay(_playerCheckR.position, Vector2.right * _distanceToPlayer, _hitPlayerR ? Color.green : Color.red);
    }
    #endregion

    #region  Collision Platform
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlatformMovement"))
        {
            transform.parent = other.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {  
        if (other.gameObject.CompareTag("PlatformMovement"))
        {
            transform.parent = null;
        }
    }
    #endregion
}
