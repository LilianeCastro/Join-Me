using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private int _idTarget;

    public Platform _Platform;
    public Transform[] _posPlat;

    [SerializeField] private PlatformType _platformType = default;
    [SerializeField] [Range(1.0f, 5.0f)] private float _speed = 2.0f;
    
    void Start()
    {
        if(_platformType.Equals(PlatformType.Movement))
        {
            StartCoroutine("PlatformMove");
        }
    }

    private void Update()
    {
        if (_platformType.Equals(PlatformType.Movement))
        {
            PlatformMove();
        }
    }

    private void PlatformMove()
    {
        _Platform.transform.position = Vector3.MoveTowards(_Platform.transform.position, _posPlat[_idTarget].position, _speed * Time.deltaTime);

        if (_Platform.transform.position.Equals(_posPlat[_idTarget].position))
        {
            _idTarget += 1;
            if (_idTarget == _posPlat.Length)
            {
                _idTarget = 0;
            }
        }
    }

    IEnumerator PlatformFall()
    {
        yield return new WaitForEndOfFrame();

        _Platform.TryGetComponent(out Rigidbody2D _PlatformRb);
        _PlatformRb.bodyType = RigidbodyType2D.Dynamic;

    }

    IEnumerator PlatformFallReturnToOrigin()
    {
        _Platform.TryGetComponent(out Rigidbody2D _PlatformRb);
        _PlatformRb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForEndOfFrame();
        StartCoroutine("PlatformReturnMove");
        
    }

    IEnumerator PlatformReturnMove()
    {
        yield return new WaitForEndOfFrame();

        _Platform.transform.position = Vector3.MoveTowards(_Platform.transform.position, _posPlat[0].position, _speed * Time.deltaTime);

        if(_Platform.transform.position.Equals(_posPlat[0].position))
        {
            StopCoroutine("PlatformReturnMove");
        }
        else
        {
            StartCoroutine("PlatformReturnMove");
        }

    }

    public void CollisionDetected(bool isOnCollision)
    {
        if(_platformType.Equals(PlatformType.Fall))
        {
            if (isOnCollision)
            {
                StartCoroutine("PlatformFall");
            }
            else
            {
                StopCoroutine("PlatformFall");
                StartCoroutine("PlatformFallReturnToOrigin");
            }
        }
        
        
    }
}
