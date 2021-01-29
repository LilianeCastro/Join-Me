using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _playerRb;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckL;
    [SerializeField] private Transform _groundCheckR;
    [SerializeField] private float _distance = 1;
    private bool _isGrounded = true;

    [Header("Physics")]
    [SerializeField][Range(1.0f, 10.0f)] private float _speedPlayer = 5.0f;
    [SerializeField][Range(500.0f, 1200.0f)] private float _jumpForce = 500.0f;

    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * _speedPlayer * horizontalInput * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _playerRb.AddForce(Vector2.up * _jumpForce);
        }
        Debug.DrawRay(_groundCheckL.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red);
        Debug.DrawRay(_groundCheckR.position, Vector2.down * _distance, _isGrounded ? Color.yellow : Color.red); 
    }

    private void FixedUpdate()
    {

        _isGrounded = Physics2D.Raycast(_groundCheckL.position, Vector2.down, _distance, 1 << LayerMask.NameToLayer("Ground"))
        || Physics2D.Raycast(_groundCheckR.position, Vector2.down, _distance, 1 << LayerMask.NameToLayer("Ground"));

    }    
}
