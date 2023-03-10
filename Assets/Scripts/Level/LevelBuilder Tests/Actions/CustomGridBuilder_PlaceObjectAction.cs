using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObject = CustomGridBuilder.GridObject;

public class CustomGridBuilder_PlaceObjectAction : IAction
{
    private CustomGrid<CustomGridBuilder.GridObject> grid;
    private PlaceableObjectSO selectedObject;
    private Vector2Int gridPos;
    private PlaceableObjectSO.Dir dir;
    private CustomGridBuilder builder;
    public CustomGridBuilder_PlaceObjectAction(CustomGrid<GridObject> grid, PlaceableObjectSO selectedObject, Vector2Int gridPos, PlaceableObjectSO.Dir dir, CustomGridBuilder builder)
    {
        this.grid = grid;
        this.selectedObject = selectedObject;
        this.gridPos = gridPos;
        this.dir = dir;
        this.builder = builder;
    }

    private CustomPlacedObject placedObject;

    public CustomPlacedObject GetObject()
    {
        return placedObject;
    }
    public void ExecuteCommand()
    {
        List<Vector2Int> gridPositionList = selectedObject.GetGridPositionList(gridPos, dir);

        //Create the object at this position
        Vector2Int rotationOffset = selectedObject.GetRotationOffset(dir);
        Vector3 objWorldPos = grid.GetWorldPosition(gridPos.x, gridPos.y) +
                              new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
        
        placedObject = CustomPlacedObject.Create(objWorldPos, gridPos, dir, selectedObject, builder.placedObjectsParent);

        //Set transform for each of the grid spaces this occupies
        foreach (var pos in gridPositionList)
        {
            grid.GetValue(pos).SetObject(placedObject);
        }
    }

    public void UndoCommand()
    {
        CustomPlacedObject currentPlacedObject = grid.GetValue(gridPos).GetPlacedObject();
        if (currentPlacedObject != placedObject)
        {
            PlaceableObjectSO currentSO = currentPlacedObject.GetPlaceableObjectSO();
            PlaceableObjectSO oldSO = placedObject.GetPlaceableObjectSO();
            
            if (currentSO == oldSO)
            {
                new CustomGridBuilder_DeleteObjectAction(grid, gridPos, builder).ExecuteCommand();
            }
        }
        else
        {
            new CustomGridBuilder_DeleteObjectAction(grid, gridPos, builder).ExecuteCommand();
        }
        
    }
}
