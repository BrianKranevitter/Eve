using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomPlacedObject : MonoBehaviour
{
    public static CustomPlacedObject Create(Vector3 worldPos, Vector2Int origin, PlaceableObjectSO.Dir dir,
        PlaceableObjectSO placedObjectSO, Transform parent)
    {
        Transform placedObjTransform = Instantiate(placedObjectSO.prefab, worldPos,
            Quaternion.Euler(0, placedObjectSO.GetRotationAngle(dir), 0), parent);
        CustomPlacedObject obj = placedObjTransform.GetComponent<CustomPlacedObject>();

        obj.placeableObjectSO = placedObjectSO;
        obj.origin = origin;
        obj.dir = dir;
        
        return obj;
    }
    
    private PlaceableObjectSO placeableObjectSO;
    private Vector2Int origin;
    private PlaceableObjectSO.Dir dir;

    public List<Vector2Int> GetGridPositionList()
    {
        return placeableObjectSO.GetGridPositionList(origin, dir);
    }

    public PlaceableObjectSO GetPlaceableObjectSO()
    {
        return placeableObjectSO;
    }

    public PlaceableObjectSO.Dir GetDir()
    {
        return dir;
    }

    public Vector2Int GetOrigin()
    {
        return origin;
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    public SaveObject GetSaveObject()
    {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(placeableObjectSO.GetInstanceID(), out string guid, out long _);
        return new SaveObject()
        {
            origin = origin,
            soName = placeableObjectSO.name,
            soGuid = guid,
            dir = dir,
            worldPos = transform.position
        };
    }

    public void LoadSaveObject(SaveObject savedObj)
    {
        origin = savedObj.origin;
        placeableObjectSO = CustomGridBuilderManager.Instance.GetSObyName(savedObj.soName, savedObj.soGuid);
        dir = savedObj.dir;
    }
    
    [Serializable]
    public class SaveObject
    {
        public Vector3 worldPos;
        public string soName;
        public string soGuid;
        public Vector2Int origin;
        public PlaceableObjectSO.Dir dir;
    }
}
