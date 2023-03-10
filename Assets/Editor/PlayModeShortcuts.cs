using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeShortcuts
{
    static PlayModeShortcuts()
    {
        EditorApplication.playModeStateChanged += ModeChanged;
        EditorApplication.quitting += Quitting;
    }
 
    static void ModeChanged(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredPlayMode)
            ShortcutManager.instance.activeProfileId = "BuildingTool";
        else if (playModeState == PlayModeStateChange.EnteredEditMode)
            ShortcutManager.instance.activeProfileId = "Default";
    }
 
    static void Quitting()
    {
        ShortcutManager.instance.activeProfileId = "Default";
    }
}
