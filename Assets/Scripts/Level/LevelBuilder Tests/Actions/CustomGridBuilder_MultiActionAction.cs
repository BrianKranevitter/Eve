using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGridBuilder_MultiActionAction : IAction
{
    private List<IAction> actions = new List<IAction>();
    public bool finished = false;

    public void Add(IAction action)
    {
        actions.Add(action);
        action.ExecuteCommand();
    }

    public void SetFinished()
    {
        finished = true;
    }

    public bool IsEmpty()
    {
        return actions.Count == 0;
    }
    
    public void ExecuteCommand()
    {
        if (!finished) return;
        
        foreach (var action in actions)
        {
            action.ExecuteCommand();
        }
    }

    public void UndoCommand()
    {
        foreach (var action in actions)
        {
            action.UndoCommand();
        }
    }
}
