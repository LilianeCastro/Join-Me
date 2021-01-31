using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLoadNewScene : MonoBehaviour
{
    [SerializeField] private string _nameSceneToLoad;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameController.Instance.ChangeScene(_nameSceneToLoad);  
    }
}
