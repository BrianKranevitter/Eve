using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAction : IAction
{
    private Action onExecute;
    private Action onUndo;
    
    public GenericAction(Action onExecute, Action onUndo)
    {
        this.onExecute = onExecute;
        this.onUndo = onUndo;
    }

    public void ExecuteCommand()
    {
        onExecute.Invoke();
    }

    public void UndoCommand()
    {
        onUndo.Invoke();
    }
}
