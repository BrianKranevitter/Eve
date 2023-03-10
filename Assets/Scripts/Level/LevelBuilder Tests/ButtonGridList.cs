using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGridList : ButtonList
{
    public int width;
    public int height;
    public Button[,] grid { get; private set; }


    private ConstraintMode mode;

    private enum ConstraintMode
    {
        width, height, none
    }

    private void Awake()
    {
        if (width > 0 && height == 0)
        {
            mode = ConstraintMode.width;
        }
        else if (width == 0 && height > 0)
        {
            mode = ConstraintMode.height;
        }
        else
        {
            mode = ConstraintMode.none;
        }
        
        grid = new Button[width, height];
    }

    public override GameObject AddObject(GameObject objToInstantiate = null)
    {
        GameObject obj = base.AddObject(objToInstantiate);
        UpdateGrid();
        return obj;
    }

    public override void ClearList()
    {
        base.ClearList();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = null;
            }
        }
    }

    void UpdateGrid()
    {
        switch (mode)
        {
            case ConstraintMode.height:
                width = Mathf.CeilToInt((float) buttons.Count / height);
                grid = new Button[width, height];
                break;
            
            case ConstraintMode.none:
                break;
            
            case ConstraintMode.width:
                height = Mathf.CeilToInt((float) buttons.Count / width);
                grid = new Button[width, height];
                break;
        }
        
        for (int i = 0; i < buttons.Count; i++)
        {
            int y = Mathf.FloorToInt(i / width);
            int x = i - (y * width);

            grid[x, y] = buttons[i];
        }
    }
}
