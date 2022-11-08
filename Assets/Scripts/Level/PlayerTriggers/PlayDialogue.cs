using System.Collections;
using System.Collections.Generic;
using Game.Level.Triggers;
using UnityEngine;

namespace Game.Level.Triggers
{
    public class PlayDialogue : PlayerTriggerAction
    {
        public DialogueSystem.DialogueSequence dialogueSequence;

        public override void OnTriggerEnter()
        {
            DialogueSystem.i.PlayDialogueSequence(dialogueSequence);
        }
    }
}
