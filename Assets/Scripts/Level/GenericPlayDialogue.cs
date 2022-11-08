using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPlayDialogue : MonoBehaviour
{
    public bool playOnAwake;
    public DialogueSystem.DialogueSequence dialogueSequence;

    private void Start()
    {
        if (playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        DialogueSystem.i.PlayDialogueSequence(dialogueSequence);
    }
}
