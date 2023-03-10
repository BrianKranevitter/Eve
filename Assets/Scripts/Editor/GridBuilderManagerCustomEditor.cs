using System.Collections;
using System.Collections.Generic;
using Enderlook.Enumerables;
using UnityEngine;
using SaveObject = CustomGridBuilderManager.SaveObject;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CustomGridBuilderManager))]
public class GridBuilderManagerCustomEditor : KamCustomEditor
{
    private string savename;
    public override void GameDesignerInspector()
    {
        CustomGridBuilderManager manager = (CustomGridBuilderManager)target;
        savename = EditorGUILayout.TextField(savename);
        if (GUILayout.Button("Load file"))
        {
            string json = PlayerPrefs.GetString(CustomGridBuilderManager.BuilderSavesPrefix + savename);

            if (json.IsNullOrEmpty())
            {
                return;
            }

            GameObject parentObj = new GameObject($"{savename} (Editing mode)");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

            foreach (CustomGridBuilderManager.PlacedObjectSaveObjectArray array in saveObject.placedObjectSaveObjectArrayArray)
            {
                GameObject subParentObj = new GameObject(array.gridName);
                subParentObj.transform.parent = parentObj.transform;
                foreach (CustomPlacedObject.SaveObject savedObj in array.placedObjectSaveObjectArray)
                {
                    PlaceableObjectSO placedObjectTypeSO = manager.GetSObyName(savedObj.soName, savedObj.soGuid);
                    CustomPlacedObject.Create(savedObj.worldPos, savedObj.origin, savedObj.dir, placedObjectTypeSO, subParentObj.transform);
                }
            }

            Debug.Log($"Loaded slot into edit mode: {savename}");
        }
    }
}
#endif