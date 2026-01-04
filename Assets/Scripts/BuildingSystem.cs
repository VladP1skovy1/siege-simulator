using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{

    
    [SerializeField] private GameObject gridVisual;
    [SerializeField] private List<GameObject> towers = new();
    [SerializeField] private List<GameObject> walls = new();
    [SerializeField] private List<GameObject> goldMines = new();
    [SerializeField] private List<GameObject> townhalls = new();
    
    private Vector3 _gridOriginPosition;
    private float _gridCellSize;
    
    private Grid _grid;

    public void Init(Grid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        _gridOriginPosition = gridVisual.transform.position;
        
        foreach (GameObject building in towers)
        {
            AddBuildingToGrid(building);
        }
        
        foreach (GameObject building in walls)
        {
            AddBuildingToGrid(building);
        }
        
        foreach (GameObject building in goldMines)
        {
            AddBuildingToGrid(building);
        }
        
        foreach (GameObject building in townhalls)
        {
            AddBuildingToGrid(building);
        }
    }


    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - _gridOriginPosition;

        int x = Mathf.FloorToInt(local.x / 1f);
        int y = Mathf.FloorToInt(local.z / 1f);

        return new Vector2Int(x, y);
    }


    private void AddBuildingToGrid(GameObject buildingObject)
    {
        Vector2Int buildingGridPosition = WorldToGrid(buildingObject.transform.position);
        BuildingVisual visual = buildingObject.GetComponent<BuildingVisual>();
        
        if (!_grid.CanPlace(buildingGridPosition.x, buildingGridPosition.y, visual.SizeX, visual.SizeY))
        {
            Debug.LogWarning("Cells are already occupied, cannot add building");
        }
        else
        {   
            GridCell cell = _grid.GetCell(
                buildingGridPosition.x,
                buildingGridPosition.y
            );
            Building building = visual.CreateLogic(cell);
            building.OriginCell.X = buildingGridPosition.x;
            building.OriginCell.Y = buildingGridPosition.y;
            _grid.AddBuilding(building);
            Debug.Log("Building added");
        }
    }
    
}
