using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RepeatedPlayDialogue : MonoBehaviour
{
    public bool playOnAwake;
    
    public MonoBehaviour coroutineHandler;
    
    [Tooltip("Type of trigger for the repeating dialogue. Time will wait X amount of time and trigger again infinitely, while EachTrigger will choose one of the options for each manual trigger.")]
    public TriggerType triggerType = TriggerType.Time;
    
    [Tooltip("Type of system choosing dialogue. InOrder will go through the list of dialogues in order with each trigger, while Random will choose one of the options at random each trigger.")]
    public ChoosingType choosingType = ChoosingType.InOrder;

    [Tooltip("Time in between each dialogue trigger")]
    public float timeBetweenTriggers;
    
    [Tooltip("Dialogue options to choose/go through with each trigger.")]
    public List<DialogueSystem.DialogueSequence> dialogueOptions;

    
    private int index;
    
    
    public enum ChoosingType
    {
        InOrder, Random
    }

    public enum TriggerType
    {
        Time, EachTrigger
    }
    
    
    private void Start()
    {
        if (playOnAwake)
        {
            Play();
        }
    }

    private Coroutine playCoroutine = null;
    public void Play()
    {
        if (playCoroutine != null)
        {
            coroutineHandler.StopCoroutine(playCoroutine);
        }


        switch (triggerType)
        {
            case TriggerType.Time:
                switch (choosingType)
                {
                    case ChoosingType.InOrder:
                        playCoroutine = coroutineHandler.StartCoroutine(PlayCoroutine_InOrder());
                        return;
            
                    case ChoosingType.Random:
                        playCoroutine = coroutineHandler.StartCoroutine(PlayCoroutine_Random());
                        return;
                }

                return;
            
            case TriggerType.EachTrigger:
                switch (choosingType)
                {
                    case ChoosingType.InOrder:
                        DialogueSystem.i.PlayDialogueSequence(dialogueOptions[index], delegate
                        {
                            if (index < dialogueOptions.Count - 1)
                            {
                                index++;
                            }
                        });
                        return;
            
                    case ChoosingType.Random:
                        DialogueSystem.i.PlayDialogueSequence(dialogueOptions[Random.Range(0, dialogueOptions.Count)]);
                        return;
                }
                
                return;
                
        }
        

        throw new Exception("Could not find play type on repeating dialogue.");
    }

    public void Stop()
    {
        if (playCoroutine != null)
        {
            coroutineHandler.StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
    }

    private IEnumerator PlayCoroutine_InOrder()
    {
        while (true)
        {
            bool next = false;
            DialogueSystem.i.PlayDialogueSequence(dialogueOptions[index], delegate
            {
                next = true;
                

                if (index < dialogueOptions.Count - 1)
                {
                    index++;
                }
            });

            while (!next)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(timeBetweenTriggers);
        }
    }

    private IEnumerator PlayCoroutine_Random()
    {
        while (true)
        {
            bool next = false;
            DialogueSystem.i.PlayDialogueSequence(dialogueOptions[Random.Range(0, dialogueOptions.Count)], delegate
            {
                next = true;
            });

            while (!next)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(timeBetweenTriggers);
        }
    }
}
