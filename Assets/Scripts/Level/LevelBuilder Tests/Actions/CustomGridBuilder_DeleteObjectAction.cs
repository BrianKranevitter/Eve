using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObject = CustomGridBuilder.GridObject;

public class CustomGridBuilder_DeleteObjectAction : IAction
{
    private CustomGrid<GridObject> grid;
    private Vector2Int gridPos;
    
    private CustomPlacedObject obj;
    private CustomGridBuilder builder;
    public CustomGridBuilder_DeleteObjectAction(CustomGrid<GridObject> grid, Vector2Int gridPos, CustomGridBuilder builder)
    {
        this.grid = grid;
        this.gridPos = gridPos;
        this.builder = builder;
    }
    
    public void ExecuteCommand()
    {
        obj = grid.GetValue(gridPos).GetPlacedObject();

        if (obj == null) return;
        
        obj.DestroySelf();
        
        List<Vector2Int> gridPositionList = obj.GetGridPositionList();
                
        foreach (var pos in gridPositionList)
        {
            grid.GetValue(pos).ClearObject();
        }
    }

    public void UndoCommand()
    {
        CustomGridBuilder_PlaceObjectAction action = 
            new CustomGridBuilder_PlaceObjectAction(grid, obj.GetPlaceableObjectSO(), obj.GetOrigin(), obj.GetDir(), builder);

        action.ExecuteCommand();
        obj = action.GetObject();
    }
}
