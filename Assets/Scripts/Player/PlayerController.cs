using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private bool _canChangePlayer = false;
    private bool _canChangeWorld = false;
    private bool _isLookLeft = false;

    [SerializeField] private string _namePlayer;
    public string NamePlayer{ get; }

    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSr = GetComponent<SpriteRenderer>();
        _playerAnim = GetComponent<Animator>();
        _playerCol = GetComponent<Collider2D>();

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
        //PlatformPass();
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

    void Movement()
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

    void Flip()
    {
        _isLookLeft = !_isLookLeft;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _playerRb.AddForce(Vector2.up * _jumpForce);
        }
    }

    private void PlatformPass()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(PassUnderPlatform());
        }
    }

    IEnumerator PassUnderPlatform()
    {
        _playerCol.enabled = false;
        yield return new WaitForSeconds(0.05f);
        _playerCol.enabled = true;
    }

    void ChangeWorld()
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

    void ChangePlayer()
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

    #region  Hit Player
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

        col.TryGetComponent(out PlayerController _otherPlayer);
        _otherPlayer.GetComponent<SpriteRenderer>().color = Color.white;
    }
    #endregion

    void DrawRayChecks()
    {
        Debug.DrawRay(_groundCheckL.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);
        Debug.DrawRay(_groundCheckR.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);

        Debug.DrawRay(_playerCheckL.position, Vector2.left * _distanceToPlayer, _hitPlayerL ? Color.green : Color.red);
        Debug.DrawRay(_playerCheckR.position, Vector2.right * _distanceToPlayer, _hitPlayerR ? Color.green : Color.red);
    }

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
