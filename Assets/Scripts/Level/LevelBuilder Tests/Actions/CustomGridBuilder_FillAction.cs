using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGridBuilder_FillAction : IAction
{
    private CustomGridBuilder grid;
    private PlaceableObjectSO selectedObject;
    private Vector2Int pointA;
    private Vector2Int pointB;
    private PlaceableObjectSO.Dir dir;

    private bool destroyMode;
    public CustomGridBuilder_FillAction(CustomGridBuilder grid, PlaceableObjectSO selectedObject, Vector2Int pointA,
        Vector2Int pointB, PlaceableObjectSO.Dir dir)
    {
        this.grid = grid;
        this.selectedObject = selectedObject;
        this.pointA = pointA;
        this.pointB = pointB;
        this.dir = dir;
        
        destroyMode = selectedObject == null;
    }

    private List<IAction> fillActions = new List<IAction>();
    public void ExecuteCommand()
    {
        Vector2Int higherPoint = Vector2Int.Max(pointA, pointB);
        Vector2Int lowerPoint = Vector2Int.Min(pointA, pointB);
        Vector2Int dragAmount = higherPoint - lowerPoint;
        fillActions.Clear();
        
        for (int x = 0; x <= dragAmount.x; x++)
        {
            for (int y = 0; y <= dragAmount.y; y++)
            {
                Vector2Int currentDrag = new Vector2Int(x, y);
                Vector2Int currentPos = lowerPoint + currentDrag;

                if (destroyMode)
                {
                    CustomGridBuilder.GridObject obj = grid.GetGrid().GetValue(currentPos);
                    CustomPlacedObject placedObj = obj.GetPlacedObject();
                    
                    if (placedObj != null)
                    {
                        CustomGridBuilder_DeleteObjectAction destroyAction = new CustomGridBuilder_DeleteObjectAction(grid.GetGrid(), placedObj.GetOrigin(), grid);
                        destroyAction.ExecuteCommand();
                        fillActions.Add(destroyAction);
                    }
                }
                else
                {
                    if (CustomGridBuilderManager.Instance.CanBuildCheck(grid.GetGrid(), selectedObject, currentPos))
                    {
                        CustomGridBuilder_PlaceObjectAction placeAction = new CustomGridBuilder_PlaceObjectAction(grid.GetGrid(), selectedObject, currentPos, dir, grid);
                        placeAction.ExecuteCommand();
                        fillActions.Add(placeAction);
                    }
                }
            }
        }
    }

    public void UndoCommand()
    {
        Debug.Log("Fillactions count: " + fillActions.Count);
        foreach (var action in fillActions)
        {
            action.UndoCommand();
        }
    }
}
