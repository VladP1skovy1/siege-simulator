using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GridVisual gridVisual;
    [SerializeField] private BuildingSystem buildingSystem;
    [SerializeField] private GameObject archerPrefab;


    private Grid _grid;
    private float _time;
    
    private bool _isRunning;

    private void Awake()
    {
        _grid = new Grid(40, 40);
        gridVisual.Init(_grid);
        buildingSystem.Init(_grid);
    }

    private void Start()
    {
        SpawnArcher(35, 1);
        SpawnArcher(1, 35);
        SpawnArcher(1, 5);

        _isRunning = true;
    }

    private void Update()
    {
        _time = Time.deltaTime;
        
        if (_isRunning)
        {
            SimulateTick(_time);
        }
        
    }

    private void SimulateTick(float dt)
    {
        int warriorCount = _grid.Warriors.Count;
        int buildingCount = _grid.Buildings.Count;

        bool allWarriorsAreDead = true;
        bool allBuildingsAreDestroyed = true;

        for (int i = 0; i < warriorCount; i++)
        {
            _grid.Warriors[i].WarriorUpdate(dt, _grid.Buildings, _grid);
            if (!_grid.Warriors[i].IsDead)
            {
                allWarriorsAreDead = false;
            }
        }

        if (allWarriorsAreDead)
        {
            _isRunning =  false;
        }
        
        for (int i = 0; i < buildingCount; i++)
        {
            _grid.Buildings[i].BuildingUpdate(dt, _grid.Warriors);
            if (!_grid.Buildings[i].IsDestroyed)
            {
                allBuildingsAreDestroyed = false;
            }
        }

        if (allBuildingsAreDestroyed)
        {
            _isRunning =  false;
        }
        
    }
    
    
    private void SpawnArcher(int x, int y)
    {
        GridCell cell = _grid.GetCell(x, y);

       
        Archer logic = new Archer(
            health: 50,
            damage: 5,
            speed: 0.2f,
            fireRange: 8,
            fireRate: 1,
            origin: cell
        );

        _grid.AddArmy(logic);

     
        GameObject go = Instantiate(archerPrefab);
        ArcherVisual view = go.GetComponent<ArcherVisual>();
        view.Bind(logic, gridVisual);
    }

    
    
}
