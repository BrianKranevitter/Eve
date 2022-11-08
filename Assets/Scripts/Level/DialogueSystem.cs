using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem i;
    
    [System.Serializable]
    public struct DialogueSequence
    {
        public List<Dialogue> _dialogues;
    }

    [System.Serializable]
    public struct Dialogue
    {
        public string subtitleText;
        public AudioClip audio;
        public float length;

        public List<DialogueEvent> dialogueEvents;
    }

    [System.Serializable]
    public struct DialogueEvent
    {
        public float triggerTimeDelay;
        public UnityEvent action;
    }

    public AudioSource dialogueSource;

    public GameObject subtitleRoot;
    public TextMeshProUGUI subtitleText;

    
    private List<Coroutine> dialogueCoroutines = new List<Coroutine>();

    private void Awake()
    {
        i = this;
        
        subtitleRoot.SetActive(false);
    }

    public void PlayDialogue(Dialogue dialogue, Action onFinish = null)
    {
        subtitleRoot.SetActive(true);
        
        dialogueSource.clip = dialogue.audio;
        subtitleText.text = dialogue.subtitleText;
        dialogueSource.Play();

        dialogueCoroutines.Add(StartCoroutine(KamUtilities.Delay(dialogue.length, delegate
        {
            onFinish.Invoke();
            subtitleRoot.SetActive(false);
            foreach (var coroutine in dialogueCoroutines)
            {
                StopCoroutine(coroutine);
            }
        })));

        foreach (var action in dialogue.dialogueEvents)
        {
            dialogueCoroutines.Add(StartCoroutine(KamUtilities.Delay(action.triggerTimeDelay, action.action.Invoke)));
        }
    }

    
    public void PlayDialogueSequence(DialogueSequence dialogueSequence, Action onFinish = null)
    {
        StopAllCoroutines();
        dialogueCoroutines = new List<Coroutine>();
        StartCoroutine(DialogueSequenceCoroutine(dialogueSequence, onFinish));
    }

    IEnumerator DialogueSequenceCoroutine(DialogueSequence dialogueSequence, Action onFinish = null)
    {
        int index = 0;

        while (index < dialogueSequence._dialogues.Count)
        {
            bool nextTrigger = false;
            while (!nextTrigger)
            {
                PlayDialogue(dialogueSequence._dialogues[index], () =>
                {
                    index++;
                    nextTrigger = true;
                });
            
                yield return null;
            }
        }
        
        onFinish?.Invoke();
    }
}
