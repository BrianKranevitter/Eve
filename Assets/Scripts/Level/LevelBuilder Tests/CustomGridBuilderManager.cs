using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Enderlook.Enumerables;
using Kam.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GridObject = CustomGridBuilder.GridObject;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class CustomGridBuilderManager : MonoBehaviour
{
    public static CustomGridBuilderManager Instance { get; private set; }
    
    [SerializeField] private List<CustomGridBuilder> gridList;
    private CustomGridBuilder selectedGrid;
    
    private int selectedId = 0;
    private int SelectedId {
        get
        {
            return selectedId;
        } 
        set 
        { 
            selectedId = value;

            if (selectedId > gridList.Count - 1)
            {
                selectedId = 0;
            }

            if (selectedId < 0)
            {
                selectedId = gridList.Count - 1;
            }

            selectedGrid = gridList[selectedId];

            foreach (var grid in gridList)
            {
                grid.gameObject.SetActive(false);
            }
            
            selectedGrid.gameObject.SetActive(true);

            

            OnSelectedChanged.Invoke(selectedGrid.GetSelectedObject());

            #region Create all of the item buttons
            buttonList_Items.ClearList();
            foreach (var item in selectedGrid.placeableObjects)
            {
                GameObject obj = buttonList_Items.AddObject();
                GridBuilderItemUI ui = obj.GetComponent<GridBuilderItemUI>();
        
                ui.LoadItem(item,selectedGrid);

                KamButton button = obj.GetComponent<KamButton>();
                button.onSelect.AddListener(delegate
                {
                    button.onClick.Invoke(); 
                    lastSelected = button;
                    
                    scrollviewAutoscroll_Item.Select(button.GetComponent<RectTransform>());
                });
            }
            #endregion

            #region create Navigation references for all of the item buttons you just created
            Button selectedGridButton = null;
            if (buttonList_Grids.list.Count != 0)
            {
                selectedGridButton = buttonList_Grids.list
                    .Find(x => x.GetComponent<GridBuilderUI>().GetBuilder() == selectedGrid)
                    .GetComponent<Button>();
            }

            int width = buttonList_Items.width;
            int height = buttonList_Items.height;
            Button[,] uiGrid = buttonList_Items.grid;

            for (int i = 0; i < buttonList_Items.list.Count; i++)
            {
                int y = Mathf.FloorToInt(i / width);
                int x = i - (y * width);
                
                Button current = uiGrid[x, y];

                int downY = (y + 1) < height ? y + 1 : 0;
                Button down = null;
                if (uiGrid[x, downY] == null)
                {
                    downY = 0;
                }
                down = uiGrid[x, downY];
                
                
                Button up = y - 1 > -1 ? uiGrid[x, y - 1] : selectedGridButton;
                
                
                int leftX = (x - 1) > -1 ? x - 1 : width - 1;
                Button left = null;
                while (uiGrid[leftX, y] == null)
                {
                    leftX--;
                }

                left = uiGrid[leftX, y];
                
                
                int rightX = (x + 1) < width ? x + 1 : 0;
                Button right = null;
                if (uiGrid[rightX, y] == null)
                {
                    rightX = 0;
                }
                right = uiGrid[rightX, y];
                
                
                Navigation newNav = new Navigation
                {
                    mode = Navigation.Mode.Explicit, 
                    selectOnRight = right, 
                    selectOnLeft = left,
                    selectOnDown = down,
                    selectOnUp = up
                };
                current.navigation = newNav;
            }
            #endregion
            
            UpdateGridBuilderButtonNavigationReferences();
            
            viewObjectsInLayerToggle.SetIsOnWithoutNotify(selectedGrid.showObjectsInGrid);
        } 
    }
    
    public PlaceableObjectSO.Dir dir = PlaceableObjectSO.Dir.Down;
    public LayerMask hitmask;
    public NoClipCameraController cameracontroller;
    public BuildingGhost ghost;
    public ObjectList buttonList_Grids;
    public ButtonGridList buttonList_Items;
    public Toggle viewObjectsInLayerToggle;
    public TextMeshProUGUI selectedText;
    public TMP_InputField saveInputField;
    public TMP_Dropdown loadDropdown;
    public GameObject loadScreen;
    public ScrollViewAutoScroll_Horizontal scrollviewAutoscroll_Builder;
    public ScrollViewAutoScroll_Horizontal scrollviewAutoscroll_Item;
    
    public Action<PlaceableObjectSO> OnSelectedChanged = (selected) => {};

    private Vector2Int lastGridPosition;
    private Vector3 lastMousePosition;
    
    private Vector2Int clickDown_FirstPoint;
    private Vector2Int clickDown_LastPoint;
    private bool draggingLeftClick = false;
    private bool draggingRightClick = false;
    private bool isPointerOverUI = false;

    private CustomGridBuilder_MultiActionAction currentMultiAction;


    private KamButton lastSelected = null;
    private void Awake()
    {
        Instance = this;
        OnSelectedChanged += so =>
        {
            string res = "Currently Selected:";
            res += " " + so.nameString;

            selectedText.text = res;
        };
        foreach (var grid in gridList)
        {
            grid.OnSelectedChanged += (selected) => { OnSelectedChanged.Invoke(selected);};
        }

        cameracontroller.onAbleToMove += ableToMove =>
        {
            if (ableToMove)
            {
                //Camera movement mode
                ghost.gameObject.SetActive(false);
                EventSystem.current.sendNavigationEvents = false;
                currentlyRunning = false;
            }
            else
            {
                //Locked, building mode
                EventSystem.current.sendNavigationEvents = true;
                ghost.gameObject.SetActive(true);
                currentlyRunning = true;
                lastSelected.Select();
            }
        };
    }

    private void Start()
    {
        SelectedId = 0;
        foreach (var grid in gridList)
        {
            GameObject obj = buttonList_Grids.AddObject();
            GridBuilderUI ui = obj.GetComponent<GridBuilderUI>();
            
            ui.LoadItem(grid, this);
            
            KamButton button = obj.GetComponent<KamButton>();
            button.onSelect.AddListener(delegate {  
                button.onClick.Invoke();
                lastSelected = button;
                
                scrollviewAutoscroll_Builder.Select(button.GetComponent<RectTransform>());
            });
        }

        UpdateGridBuilderButtonNavigationReferences();
        SelectedId = 0;
        buttonList_Items.grid[0,0].Select();
    }

    private bool currentlyRunning = true;
    private void Update()
    {
        if (saveInputField.isFocused || loadScreen.activeSelf)
        {
            cameracontroller.detectingInput = false;
            currentlyRunning = false;
        }
        else
        {
            cameracontroller.detectingInput = true;

            if (!cameracontroller.ableToMove)
            {
                currentlyRunning = true;
            }
        }
        
        if (!currentlyRunning) return;

        bool leftButtonDown = Input.GetMouseButtonDown(0);
        bool leftButton = Input.GetMouseButton(0);
        bool leftButtonUp = Input.GetMouseButtonUp(0);
        
        bool rightButtonDown = Input.GetMouseButtonDown(1);
        bool rightButton = Input.GetMouseButton(1);
        bool rightButtonUp = Input.GetMouseButtonUp(1);

        if (leftButtonDown || leftButton || leftButtonUp)
        {
            rightButtonDown = false;
            rightButton = false;
            rightButtonUp = false;
        }


        if (leftButtonDown || rightButtonDown)
        {
            currentMultiAction = new CustomGridBuilder_MultiActionAction();
        }

        if (leftButtonUp || rightButtonUp)
        {
            if (!currentMultiAction.IsEmpty())
            {
                CommandManager.instance.ExecuteCommand(currentMultiAction);
            }
            
            currentMultiAction.SetFinished();
            currentMultiAction = new CustomGridBuilder_MultiActionAction();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!draggingRightClick && !draggingLeftClick)
            {
                if (leftButtonDown || rightButtonDown)
                {
                    Vector3 mousePos = KamUtilities.GetMouseWorldPos_WithZ(hitmask);
                    CustomGrid<GridObject> grid = selectedGrid.GetGrid();
                    Vector2Int gridPos = grid.GetGridPos(mousePos);

                    clickDown_FirstPoint = gridPos;

                    if (leftButtonDown)
                    {
                        draggingLeftClick = true;
                    }
                    else
                    {
                        draggingRightClick = true;
                    }
                }
            }
            
        }

        if (draggingLeftClick || draggingRightClick)
        {
            Vector3 mousePos = KamUtilities.GetMouseWorldPos_WithZ(hitmask);
            CustomGrid<GridObject> grid = selectedGrid.GetGrid();
            Vector2Int gridPos = grid.GetGridPos(mousePos);

            clickDown_LastPoint = gridPos;
            DrawDragVisual(draggingLeftClick ? Color.green : Color.red);
            
            if (leftButtonUp || rightButtonUp)
            {
                if (leftButtonUp)
                {
                    Fill(selectedGrid, selectedGrid.GetSelectedObject(), clickDown_FirstPoint, clickDown_LastPoint);
                    draggingLeftClick = false;
                }
                else
                {
                    Fill(selectedGrid, null, clickDown_FirstPoint, clickDown_LastPoint);
                    draggingRightClick = false;
                }
            }
        }
        else if (leftButton)
        {
            Vector3 mousePos = KamUtilities.GetMouseWorldPos_WithZ(hitmask);

            if (lastMousePosition != mousePos)
            {
                lastMousePosition = mousePos;
                
                CustomGrid<GridObject> grid = selectedGrid.GetGrid();
                Vector2Int gridPos = grid.GetGridPos(mousePos);

                if (lastGridPosition != gridPos)
                {
                    lastGridPosition = gridPos;

                    PlaceableObjectSO selectedObject = selectedGrid.GetSelectedObject();
                    
                    PlaceObject(grid, selectedObject, gridPos, dir);
                }
            }
        }
        else if (rightButton)
        {
            Vector3 mousePos = KamUtilities.GetMouseWorldPos_WithZ(hitmask);

            if (lastMousePosition != mousePos)
            {
                lastMousePosition = mousePos;
                
                CustomGrid<GridObject> grid = selectedGrid.GetGrid();
                Vector2Int gridPos = grid.GetGridPos(mousePos);

                if (lastGridPosition != gridPos)
                {
                    lastGridPosition = gridPos;

                    GridObject obj = grid.GetValue(gridPos);
                    CustomPlacedObject placedObj = obj.GetPlacedObject();

                    if (placedObj != null)
                    {
                        currentMultiAction.Add(new CustomGridBuilder_DeleteObjectAction(grid, gridPos, selectedGrid));
                    }
                    //selectedGrid.DestroyPlacedObject(placedObj);
                }
            }
        }
        else
        {
            lastMousePosition = Vector3.negativeInfinity;
            lastGridPosition = new Vector2Int(-1, -1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlaceableObjectSO.GetNextDir(dir);
        }

        
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            lastSelected.Select();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShowObjectsInGridFunc();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CommandManager.instance.UndoCommand();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                CommandManager.instance.RedoCommand();
            }
        }
        
    }
    
    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePos = KamUtilities.GetMouseWorldPos_WithZ(hitmask);
        CustomGrid<GridObject> grid = selectedGrid.GetGrid();
        Vector2Int gridPos = grid.GetGridPos(mousePos);

        if (selectedGrid.GetSelectedObject() != null) {
            Vector2Int rotationOffset = selectedGrid.GetSelectedObject().GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(gridPos.x, gridPos.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } 
        else 
        {
            return mousePos;
        }
    }

    public PlaceableObjectSO GetSelectedObject()
    {
        return selectedGrid.GetSelectedObject();
    }

    public void SelectGrid(CustomGridBuilder builder)
    {
        for (int i = 0; i < gridList.Count; i++)
        {
            if (gridList[i] == builder)
            {
                SelectedId = i;
            }
        }
    }

    public CustomGridBuilder GetSelectedGrid()
    {
        return selectedGrid;
    }

    public void Fill(CustomGridBuilder grid, PlaceableObjectSO selectedObject, Vector2Int pointA,
        Vector2Int pointB)
    {
        bool fillable = false;
        
        #region Initial Check
            //This entire section is an initial check to see if anything within the bounds is actually placeable/destroyable
            //this is to make sure you only execute and save the command if it will do something
            bool destroyMode = selectedObject == null;

            Vector2Int higherPoint = Vector2Int.Max(pointA, pointB);
            Vector2Int lowerPoint = Vector2Int.Min(pointA, pointB);
            Vector2Int dragAmount = higherPoint - lowerPoint;

            for (int x = 0; x <= dragAmount.x; x++)
            {
                for (int y = 0; y <= dragAmount.y; y++)
                {
                    Vector2Int currentDrag = new Vector2Int(x, y);
                    Vector2Int currentPos = lowerPoint + currentDrag;

                    if (destroyMode)
                    {
                        CustomPlacedObject obj = grid.GetGrid().GetValue(currentPos).GetPlacedObject();
                        
                        if (obj != null)
                        {
                            //Destroyable
                            fillable = true;
                        }
                    }
                    else
                    {
                        if (CanBuildCheck(grid.GetGrid(), selectedObject, currentPos))
                        {
                            //Placeable
                            fillable = true;
                        }
                    }
                }
            }
        #endregion
        
        if (fillable)
        {
            CommandManager.instance.ExecuteCommand(new CustomGridBuilder_FillAction(grid, selectedObject, pointA, pointB, dir));
        }
        /*
        bool destroy = selectedObject == null;

        Vector2Int higherPoint = Vector2Int.Max(pointA, pointB);
        Vector2Int lowerPoint = Vector2Int.Min(pointA, pointB);
        Vector2Int dragAmount = higherPoint - lowerPoint;

        for (int x = 0; x <= dragAmount.x; x++)
        {
            for (int y = 0; y <= dragAmount.y; y++)
            {
                Vector2Int currentDrag = new Vector2Int(x, y);
                Vector2Int currentPos = lowerPoint + currentDrag;

                if (destroy)
                {
                    GridObject obj = grid.GetGrid().GetValue(currentPos);
                    CustomPlacedObject placedObj = obj.GetPlacedObject();
                    grid.DestroyPlacedObject(placedObj);
                }
                else
                {
                    PlaceObject(grid.GetGrid(), selectedObject, currentPos);
                }
            }
        }
        
        */
    }

    public void DrawDragVisual(Color color)
    {
        Vector2Int higherPoint = Vector2Int.Max(clickDown_FirstPoint, clickDown_LastPoint);
        Vector2Int lowerPoint = Vector2Int.Min(clickDown_FirstPoint, clickDown_LastPoint);
        CustomGrid<GridObject> grid = selectedGrid.GetGrid();

        Vector3 downLeft = grid.GetWorldPosition(lowerPoint.x, lowerPoint.y);
        Vector3 upLeft = grid.GetWorldPosition(lowerPoint.x, higherPoint.y + 1);
        Vector3 downRight = grid.GetWorldPosition(higherPoint.x + 1, lowerPoint.y);
        Vector3 upRight = grid.GetWorldPosition(higherPoint.x + 1, higherPoint.y + 1);
        //From down left corner to up left corner
        Debug.DrawLine(downLeft, upLeft, color);
        
        //From down left corner to down right corner
        Debug.DrawLine(downLeft, downRight, color);
        
        //From up right corner to down right corner
        Debug.DrawLine(upRight, downRight, color);
        
        //From up right corner to up left corner
        Debug.DrawLine(upRight, upLeft, color);
    }
    public void PlaceObject(CustomGrid<GridObject> grid, PlaceableObjectSO selectedObject, Vector2Int gridPos, PlaceableObjectSO.Dir dir)
    {
        if (CanBuildCheck(grid, selectedObject,gridPos))
        {
            currentMultiAction.Add(new CustomGridBuilder_PlaceObjectAction(grid, selectedObject, gridPos, dir, selectedGrid));
        }
    }

    public bool CanBuildCheck(CustomGrid<GridObject> grid, PlaceableObjectSO selectedObject, Vector2Int gridPos)
    {
        List<Vector2Int> gridPositionList = selectedObject.GetGridPositionList(gridPos, dir);
        bool CanBuild = true;

        if (selectedGrid.showObjectsInGrid && !isPointerOverUI)
        {
            try
            {
                foreach (var pos in gridPositionList)
                {
                    GridObject obj = grid.GetValue(pos);
                    if (!obj.IsEmpty())
                    {
                        CanBuild = false;
                    }
                }
            }
            catch
            {
                CanBuild = false;
            }
        }
        else
        {
            CanBuild = false;
        }
        
        return CanBuild;
    }
    public Quaternion GetPlacedObjectRotation() {
        PlaceableObjectSO selectedObject = selectedGrid.GetSelectedObject();
        
        if (selectedObject != null) {
            return Quaternion.Euler(0, selectedObject.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
    }

    public void UpdateGridBuilderButtonNavigationReferences()
    {
        Button selectedItemButton = buttonList_Items.list[0].GetComponent<Button>();
        
        for (int i = 0; i < buttonList_Grids.list.Count; i++)
        {
            Button ui = buttonList_Grids.list[i].GetComponent<Button>();
                
            Button lastUi;
            if (i == 0)
            {
                lastUi = buttonList_Grids.list[buttonList_Grids.list.Count-1].GetComponent<Button>();
            }
            else
            {
                lastUi = buttonList_Grids.list[i-1].GetComponent<Button>();
            }
                
            Button nextUi;
            if (i + 1 < buttonList_Grids.list.Count)
            {
                //There is another item to the right
                nextUi = buttonList_Grids.list[i + 1].GetComponent<Button>();
            }
            else
            {
                //Wrap around to first
                nextUi = buttonList_Grids.list[0].GetComponent<Button>();
            }
            
            Navigation customNav = new Navigation
            {
                mode = Navigation.Mode.Explicit, 
                selectOnRight = nextUi, 
                selectOnLeft = lastUi, 
                selectOnDown = selectedItemButton,
            };
            
            ui.navigation = customNav;
        }
    }

    public void ShowObjectsInGridFunc()
    {
        bool state = !selectedGrid.showObjectsInGrid;
        SetShowObjectsInGrid(state);
    }

    void SetShowObjectsInGrid(bool state)
    {
        viewObjectsInLayerToggle.SetIsOnWithoutNotify(state);
        selectedGrid.ShowObjectsInGrid(state);
        
        buttonList_Grids.list.Find(x => x.GetComponent<GridBuilderUI>().GetBuilder() == selectedGrid).GetComponent<GridBuilderUI>().SetDisabledOverlay(!state);
    }

    public void IsPointerOverUI(bool state)
    {
        isPointerOverUI = state;
    }

    public PlaceableObjectSO GetSObyName(string soName, string soGuid)
    {
        foreach (CustomGridBuilder builder in gridList)
        {
            foreach (PlaceableObjectSO so in builder.placeableObjects)
            {
                //try by name
                if (so.name == soName)
                {
                    return so;
                }
                
                //try by GUID
                /*AssetDatabase.TryGetGUIDAndLocalFileIdentifier(so.GetInstanceID(), out string guid, out long _);
                if (guid == soGuid)
                {
                    return so;
                }*/
            }
        }

        throw new Exception($"There was an error loading PlaceableObjectSO of name \"{soName}\"");
    }
    
    
    #region Saving System

        public static string BuilderSavenames = "BuildingSystemSave";
        public static string BuilderSavesPrefix = "BuildingSystemSave_";

        public static List<string> BuilderSaveList = new List<string>();
        public void Save() 
        {
            List<PlacedObjectSaveObjectArray> placedObjectSaveObjectArrayList = new List<PlacedObjectSaveObjectArray>();

            foreach (CustomGridBuilder gridBuilder in gridList)
            {
                CustomGrid<GridObject> grid = gridBuilder.GetGrid();
                List<CustomPlacedObject.SaveObject> saveObjectList = new List<CustomPlacedObject.SaveObject>();
                List<CustomPlacedObject> savedPlacedObjectList = new List<CustomPlacedObject>();

                for (int x = 0; x < grid.GetWidth(); x++) {
                    for (int y = 0; y < grid.GetHeight(); y++) {
                        CustomPlacedObject placedObject = grid.GetValue(new Vector2Int(x, y)).GetPlacedObject();
                        if (placedObject != null && !savedPlacedObjectList.Contains(placedObject)) {
                            // Save object
                            savedPlacedObjectList.Add(placedObject);
                            saveObjectList.Add(placedObject.GetSaveObject());
                        }
                    }
                }

                PlacedObjectSaveObjectArray placedObjectSaveObjectArray = new PlacedObjectSaveObjectArray
                {
                    placedObjectSaveObjectArray = saveObjectList.ToArray(),
                    gridName = gridBuilder.name
                };
                placedObjectSaveObjectArrayList.Add(placedObjectSaveObjectArray);
            }

            SaveObject saveObject = new SaveObject {
                placedObjectSaveObjectArrayArray = placedObjectSaveObjectArrayList.ToArray(),
            };

            string json = JsonUtility.ToJson(saveObject);

            string saveName = BuilderSavesPrefix + (saveInputField.text == "" ? "Unnamed" : saveInputField.text);
            
            if (!BuilderSaveList.Contains(saveName))
            {
                BuilderSaveList.Add(saveName);
                Debug.Log($"Added {saveName} to list");
            }

            string jsonList = JsonUtility.ToJson(new Savenames
            {
                saves = BuilderSaveList
            });
            
            PlayerPrefs.SetString(BuilderSavenames, jsonList);
            PlayerPrefs.SetString(saveName, json);

            Debug.Log($"Saved to slot: {saveName}");
        }

        public void Load(string save) 
        {
            string saveName = BuilderSavesPrefix + save;
            string json = PlayerPrefs.GetString(saveName);
            
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

            CustomGridBuilder_MultiActionAction loadingAction = new CustomGridBuilder_MultiActionAction();
            
            for (int i=0; i<gridList.Count; i++) {
                CustomGridBuilder gridBuilder = gridList[i];
                gridBuilder.ClearGrid(ref loadingAction);
                
                CustomGrid<GridObject> grid = gridBuilder.GetGrid();
                
                foreach (CustomPlacedObject.SaveObject placedObjectSaveObject in saveObject.placedObjectSaveObjectArrayArray[i].placedObjectSaveObjectArray)
                {
                    PlaceableObjectSO placedObjectTypeSO = GetSObyName(placedObjectSaveObject.soName, placedObjectSaveObject.soGuid);
                    loadingAction.Add(new CustomGridBuilder_PlaceObjectAction(grid, placedObjectTypeSO, placedObjectSaveObject.origin, placedObjectSaveObject.dir, gridBuilder));
                }
            }

            CommandManager.instance.ExecuteCommand(loadingAction);
            saveInputField.text = save;
            Debug.Log($"Loaded slot: {saveName}");
        }

        public void DeleteSave()
        {
            string selectedSave = BuilderSavesPrefix + loadDropdown.options[loadDropdown.value].text;
            PlayerPrefs.DeleteKey(selectedSave);
            BuilderSaveList.Remove(selectedSave);
            
            string jsonList = JsonUtility.ToJson(new Savenames
            {
                saves = BuilderSaveList
            });
            
            PlayerPrefs.SetString(BuilderSavenames, jsonList);

            if (BuilderSaveList.Count == 0)
            {
                HideLoadScreen();
            }
            else
            {
                ShowLoadScreen();
            }
            
            Debug.Log($"Removed slot: {selectedSave}");
        }

        public void ShowLoadScreen()
        {
            BuilderSaveList = JsonUtility.FromJson<Savenames>(PlayerPrefs.GetString(BuilderSavenames)).saves;
            if (BuilderSaveList.Count == 0)
            {
                return;
            }
            
            loadDropdown.ClearOptions();

            foreach (var savename in BuilderSaveList)
            {
                string save = savename.Replace(BuilderSavesPrefix, "");
                loadDropdown.options.Add(new TMP_Dropdown.OptionData(save));
            }

            loadScreen.SetActive(true);
        }
    

        public void ConfirmLoadScreen()
        {
            Load(loadDropdown.options[loadDropdown.value].text);
            HideLoadScreen();
        }
        public void HideLoadScreen()
        {
            loadScreen.SetActive(false);
        }
    
        [Serializable]
        public class SaveObject {

            public PlacedObjectSaveObjectArray[] placedObjectSaveObjectArrayArray;
        }

        [Serializable]
        public class PlacedObjectSaveObjectArray
        {
            public string gridName;
            public CustomPlacedObject.SaveObject[] placedObjectSaveObjectArray;
        }

        [Serializable]
        public class Savenames
        {
            public List<string> saves = new List<string>();
        }
    #endregion
}
