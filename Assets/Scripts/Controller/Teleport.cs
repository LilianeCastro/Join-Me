using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform _portalToTeleport = default;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TryGetComponent(out Transform playerTransform);
            playerTransform.position = new Vector2(_portalToTeleport.transform.position.x, playerTransform.position.y);
        }
    }
}
