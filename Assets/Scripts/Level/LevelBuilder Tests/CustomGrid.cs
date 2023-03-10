using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using TMPro;
using UnityEngine;

public class CustomGrid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged = (sender, eventArgs) => { };

    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPos;
    private TGridObject[,] gridArray;
    private TextMeshPro[,] textArray;

    public CustomGrid(int width, int height, float cellSize, Vector3 originPos, Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject, bool showDebug = true)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;

        gridArray = new TGridObject[width, height];
        textArray = new TextMeshPro[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                SetValue(x, y, createGridObject(this, x, y));
            }
        }
        
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    textArray[x,y] = KamUtilities.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize, cellSize) * .5f, 0.25f, Color.white,
                        TextAnchor.MiddleCenter);
                
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
                }
            }
        
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 1000f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 1000f);

            OnGridValueChanged += (sender, eventArgs) =>
            {
                textArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return GetWorldPosition(gridPos.x, gridPos.y);
    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPos;
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        Vector2Int raw = new Vector2Int(Mathf.FloorToInt((worldPos - originPos).x / cellSize),
            Mathf.FloorToInt((worldPos - originPos).z / cellSize));
        Vector2Int clamp = new Vector2Int(Mathf.Clamp(raw.x, 0, width - 1),Mathf.Clamp(raw.y, 0, height - 1));
        return clamp;
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            
        }
    }

    public void SetValue(Vector3 worldPos, TGridObject value)
    {
        Vector2Int gridPos = GetGridPos(worldPos);
        SetValue(gridPos.x, gridPos.y, value);
    }

    public TGridObject GetValue(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height)
        {
            return gridArray[pos.x, pos.y];
        }
        else
        {
            throw new Exception("Invalid Array Pos Value");
        }
    }
    
    public TGridObject GetValue(Vector3 worldPos)
    {
        Vector2Int gridPos = GetGridPos(worldPos);
        return GetValue(gridPos);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridValueChangedEventArgs args = new OnGridValueChangedEventArgs();
        args.x = x;
        args.y = y;
        OnGridValueChanged.Invoke(this, args);
    }
}
