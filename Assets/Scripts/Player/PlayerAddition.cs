using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddition : MonoBehaviour
{
    private bool _canRig;

    private void Update()
    {
        if (_canRig && GameController.Instance.IsCommonWorldActive)
        {
            print("logica pra riggar");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Piece") && GameController.Instance.IsCommonWorldActive)
        {
            _canRig = true;
            print("Pode adicionar");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Piece"))
        {
            _canRig = false;
            print("Não pode adicionar");
        }
    }
}
