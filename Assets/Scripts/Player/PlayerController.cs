using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject _feedbackControl;
    //[SerializeField] private Camera _mainCamPlayer;

    private Rigidbody2D _playerRb;
    [SerializeField] private LayerMask layerMask;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckL;
    [SerializeField] private Transform _groundCheckR;
    [SerializeField] private float _distance = 1;
    private bool _isGrounded = true;

    [Header("Player Check")]
    private RaycastHit2D _hitPlayerL;
    private RaycastHit2D _hitPlayerR;
    private Collider2D _hitPlayerCollider;
    [SerializeField] private Transform _playerCheckL;
    [SerializeField] private Transform _playerCheckR;
    [SerializeField] private float _distanceToPlayer = 0.2f;

    [Header("Physics")]
    [SerializeField][Range(1.0f, 10.0f)] private float _speedPlayer = 5.0f;
    [SerializeField][Range(500.0f, 1600.0f)] private float _jumpForce = 500.0f;

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
            //_mainCamPlayer.gameObject.SetActive(value);
        }
    }

    private bool _canChangePlayer;

    [SerializeField] private string _namePlayer;
    public string NamePlayer{ get; }

    private bool _canChangeWorld = true;

    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
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
        //if (!CanControl) { return ; }
        _isGrounded = Physics2D.Raycast(_groundCheckL.position, Vector2.down, _distance, layerMask)
        || Physics2D.Raycast(_groundCheckR.position, Vector2.down, _distance, layerMask);

        _hitPlayerL = Physics2D.Raycast(_playerCheckL.position, Vector2.left, _distanceToPlayer, 1 << LayerMask.NameToLayer("Player"));
        _hitPlayerR = Physics2D.Raycast(_playerCheckR.position, Vector2.right, _distanceToPlayer, 1 << LayerMask.NameToLayer("Player"));
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * _speedPlayer * horizontalInput * Time.deltaTime);    
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _playerRb.AddForce(Vector2.up * _jumpForce);
        }
    }

    void ChangeWorld()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {    
            GameController.Instance.ActiveWorld(this.gameObject, _canChangeWorld);
            _canChangeWorld = !_canChangeWorld;
        }    
    }

    void ChangePlayer()
    {
        if (!_canChangePlayer) { return ; }

        if (Input.GetKeyDown(KeyCode.N))
        {
            _canChangePlayer = false;
                              
            GameController.Instance.ActiveWorld(this.gameObject, false);

            CanControl = false;
            _feedbackControl.SetActive(false);

            _hitPlayerCollider.gameObject.TryGetComponent(out PlayerController otherPlayer);
            otherPlayer.CanControl = true;
            otherPlayer._feedbackControl.SetActive(true);
        }
    }

    void HitPlayer()
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

    void PlayerDetected(Collider2D col)
    {
        //Quando encostar no outro player ele nao vai poder mais andar, e o mundo vai para o mundo normal de ambos
        _canChangePlayer = true;
        _hitPlayerCollider = col;
    }

    void DrawRayChecks()
    {
        Debug.DrawRay(_groundCheckL.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);
        Debug.DrawRay(_groundCheckR.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);

        Debug.DrawRay(_playerCheckL.position, Vector2.left * _distanceToPlayer, _hitPlayerL ? Color.green : Color.red);
        Debug.DrawRay(_playerCheckR.position, Vector2.right * _distanceToPlayer, _hitPlayerR ? Color.green : Color.red);
    }
}
