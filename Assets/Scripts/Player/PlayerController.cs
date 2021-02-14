using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private Rigidbody2D _playerRb;
    private SpriteRenderer _playerSr;
    private Animator _playerAnim;
    private Collider2D _playerCol;

    [Header("Sound")]
    [SerializeField] private AudioSource _playerAudioSource = default;
    [SerializeField] private AudioSource _playerAudioSfx = default;
    [SerializeField] private AudioClip[] _playerSfx = default;

    [SerializeField] private LayerMask layerMask = default; //pulo
    [SerializeField] private Camera _mainCamPlayer = default;
    [SerializeField] private Color _colorDisabled = default;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckL = default;
    [SerializeField] private Transform _groundCheckR = default;
    [SerializeField] private float _distance = 1.0f;
    private bool _isGrounded = true;
    private float _horizontalInput = 0.0f;

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
            _mainCamPlayer.gameObject.SetActive(_canControl);   
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

    [SerializeField] private GameObject _feedbackControl = default;
    public bool FeedbackControl
    {
        set
        {
            bool isActive = value;
            _feedbackControl.SetActive(isActive);
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
        _playerAudioSource = GetComponent<AudioSource>();
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
        if (!CanControl || Pause.Instance.IsPaused || GameController.Instance.GameOver) { return ; }

        Movement();
        Jump();
        PlayWalkSfx();

        ChangeWorld();
        HitPlayer();
        ChangePlayer();
    }

    private void FixedUpdate()
    {
        if (!CanControl || Pause.Instance.IsPaused || GameController.Instance.GameOver) { return ; }

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
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        transform.Translate(Vector3.right * _speedPlayer * _horizontalInput * Time.deltaTime);

        _playerAnim.SetBool("isGrounded", _isGrounded);
        _playerAnim.SetInteger("speedX", (int)_horizontalInput);

        if(_horizontalInput > 0 && _isLookLeft)
        {
            Flip();

        }
        else if(_horizontalInput < 0 && !_isLookLeft)
        {
            Flip();    
        }
    }

    void PlayWalkSfx()
    {   
        if (_horizontalInput != 0 && _isGrounded)
        {
            if (!_playerAudioSource.isPlaying)
            {
                _playerAudioSource.Play();
            }    
        }
        else
        {
            if (_playerAudioSource.isPlaying)
            {
                _playerAudioSource.Stop();
            } 
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

            _playerAudioSfx.PlayOneShot(_playerSfx[1]);
        }
    }
    #endregion

    #region Change World and Player
    private void ChangeWorld()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {  
            transform.parent = null;

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!GameController.Instance.ListPlayerIsLarge()) { return; }

            transform.parent = null;

            if (GameController.Instance.IsInvertedWorldActive)
            {
                GameController.Instance.ChangeWorld();
            }
            GameController.Instance.NextPlayerControl();
        }
    }

    public void PlaySoundChange()
    {
        _playerAudioSfx.PlayOneShot(_playerSfx[0]);
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
        _hitPlayerCollider = col;

        col.TryGetComponent(out PlayerController _otherPlayer);
        _otherPlayer.GetComponent<SpriteRenderer>().color = Color.white;
        
        //Se o player nao estiver ativo na cena, ele vai falar que ele foi tocado e o colocara nas lista de players q pode trocar
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
