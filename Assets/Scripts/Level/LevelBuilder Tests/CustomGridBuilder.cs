using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Kam.Utils;
using TMPro;
using UnityEngine;

public class CustomGridBuilder : MonoBehaviour
{
    private CustomGrid<GridObject> grid;

    public Action<PlaceableObjectSO> OnSelectedChanged = (selected) => {};
    private Action OnDrawGizmosAction = () => { };
    public List<PlaceableObjectSO> placeableObjects;
    public Transform placedObjectsParent;
    private PlaceableObjectSO selectedObject;
    private int selectedId = 0;
    public int SelectedId {
        get
        {
            return selectedId;
        } 
        set 
        { 
            selectedId = value;

            if (selectedId > placeableObjects.Count - 1)
            {
                selectedId = 0;
            }
            
            if (selectedId < 0)
            {
                selectedId = placeableObjects.Count - 1;
            }

            selectedObject = placeableObjects[selectedId];
            
            OnSelectedChanged.Invoke(selectedObject);
        } 
    }

    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;
    [SerializeField] private bool showDebug = true;
    
    public bool showObjectsInGrid = true;
    [SerializeField] private bool showTextDebug = false;
    
    private void Awake()
    {
        SelectedId = 0;
        grid = new CustomGrid<GridObject>(gridWidth, gridHeight, cellSize, gridOrigin,
            ((grid, x, z) => new GridObject(grid, x, z)), showTextDebug);

        
    }

    private bool showingGizmos = false;
    private void Update()
    {
        if (showDebug)
        {
            if (!showingGizmos)
            {
                showingGizmos = true;
                OnDrawGizmosAction += CustomGizmos;
            }
        }
        else
        {
            if (showingGizmos)
            {
                showingGizmos = false;
                OnDrawGizmosAction = () => { };
            }
        }
    }

    void CustomGizmos()
    {
        Gizmos.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.05f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1));
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y));
            }
        }
        
        Gizmos.DrawLine(grid.GetWorldPosition(0, gridHeight), grid.GetWorldPosition(gridWidth, gridHeight));
        Gizmos.DrawLine(grid.GetWorldPosition(gridWidth, 0), grid.GetWorldPosition(gridWidth, gridHeight));
    }
    public CustomGrid<GridObject> GetGrid()
    {
        return grid;
    }

    public PlaceableObjectSO GetSelectedObject()
    {
        return selectedObject;
    }

    public Vector3 GetOrigin()
    {
        return gridOrigin;
    }
    public void SelectObject(PlaceableObjectSO so)
    {
        for (int i = 0; i < placeableObjects.Count; i++)
        {
            if (placeableObjects[i] == so)
            {
                SelectedId = i;
            }
        }
    }
    
    public void ShowObjectsInGrid(bool state)
    {
        showObjectsInGrid = state;
        placedObjectsParent.gameObject.SetActive(state);
    }

    private void OnDrawGizmos()
    {
        OnDrawGizmosAction.Invoke();
    }

    public void ClearGrid(ref CustomGridBuilder_MultiActionAction multiAction)
    {
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector2Int origin = new Vector2Int(x, y);
                CustomPlacedObject obj = grid.GetValue(origin).GetPlacedObject();
                if (obj != null)
                {
                    multiAction.Add(new CustomGridBuilder_DeleteObjectAction(grid, origin, this));
                }
            }
        }
    }

    public void ClearGridInstant()
    {
        CustomGridBuilder_MultiActionAction multiAction = new CustomGridBuilder_MultiActionAction();
        ClearGrid(ref multiAction);
        CommandManager.instance.ExecuteCommand(multiAction);
    }

    public class GridObject
    {
        private CustomGrid<GridObject> grid;
        private int x;
        private int z;

        private CustomPlacedObject placedObj;
        
        public GridObject(CustomGrid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            string result = x + ", " + z;
            if (placedObj != null)
            {
                result += $"\n{placedObj.name}";
            }
            return result;
        }

        public CustomPlacedObject GetPlacedObject()
        {
            return placedObj;
        }

        public void SetObject(CustomPlacedObject placedObj)
        {
            this.placedObj = placedObj;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearObject()
        {
            placedObj = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool IsEmpty()
        {
            return placedObj == null;
        }
    }
    

}
