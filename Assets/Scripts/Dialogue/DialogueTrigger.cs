using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private bool _canDestroy = false;
    [SerializeField] private Color _colorText = default;
    [SerializeField] [Range(0.5f, 5.0f)] private float _delayTimeInScene = 2.0f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        //DialogueManager.Instance.StartDialogue(_dialogue, _delayTimeInScene);
        DialogueManager.Instance.StartDialogue(_dialogue, _delayTimeInScene, _colorText);

        if (_canDestroy)
        {
            Destroy(this.gameObject, _delayTimeInScene);
        }
    }

    
}
