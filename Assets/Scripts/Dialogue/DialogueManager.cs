﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private Text _dialogText;

    public void StartDialogue(Dialogue dialogue, float timeToShowInScene)
    {
       //Colocar texto para nome de bot?
        _dialogText.text = dialogue._sentence;
        StartCoroutine(DestroyDialogue(timeToShowInScene));
    }
    
    IEnumerator DestroyDialogue(float timeToShow)
    {
        yield return new WaitForSeconds(timeToShow);
        _dialogText.text = "";
    }
}
