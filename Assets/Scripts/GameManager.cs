using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Logic References")]
    [SerializeField] private GridVisual gridVisual;
    [SerializeField] private BuildingSystem buildingSystem;
    
    [Header("UI References")]
    [SerializeField] private GameObject startUIPanel;
    [SerializeField] private GameObject battleUIPanel;
    [SerializeField] private GameObject endUIPanel;
    [SerializeField] private TMP_Text battleTimerText;
    [SerializeField] private TMP_Text battleScoreText;
    [SerializeField] private TMP_Text battleCostText;
    [SerializeField] private TMP_Text endTimerText;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private TMP_Text endCostText;
    [SerializeField] private TMP_Text epochText;
    

    [Header("EOA Parameters")] 
    [SerializeField] private int epochCount;
    [SerializeField] private int maxArmyCost;
    [SerializeField] private int populationSize;
    [SerializeField] private bool isRandom;
    [SerializeField] private bool isLocal;
    [SerializeField] private bool isEvolutionary;
    [SerializeField] private bool isMemetic;
    [SerializeField] private string perturbationType = "Mix";     // "Mix", "Move", "Add"
    [SerializeField] private string selectionType = "Tournament"; // "Tournament", "Roulette"
    [SerializeField] private string crossoverType = "Uniform";    // "Uniform", "OnePoint"
    [SerializeField] private string replacementType = "Elitism";  // "Elitism", "Generational"
    [SerializeField] private int lsEpochs;
    [SerializeField] private float lsProbability;
    [SerializeField] private bool improveInit;
    
    
    [Header("Archer Parameters")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private int archerHealth;
    [SerializeField] private int archerDamage;
    [SerializeField] private float archerSpeed; // smaller -> faster
    [SerializeField] private int archerFireRange;
    [SerializeField] private float archerFireRate; //smaller -> faster
    [SerializeField] private int archerCost; // 100
    
    
    [Header("Golem Parameters")]
    [SerializeField] private GameObject golemPrefab;
    [SerializeField] private int golemHealth;
    [SerializeField] private int golemDamage;
    [SerializeField] private float golemSpeed;
    [SerializeField] private int golemFireRange;
    [SerializeField] private float golemFireRate;
    [SerializeField] private int golemCost; // 200

    [Header("Bomber Parameters")]
    [SerializeField] private GameObject bomberPrefab;
    [SerializeField] private int bomberHealth;
    [SerializeField] private int bomberDamage;
    [SerializeField] private float bomberSpeed;
    [SerializeField] private int bomberFireRange;
    [SerializeField] private float bomberFireRate; 
    [SerializeField] private int bomberCost; // 50


    private enum WarriorTypes
    {
        Archer,
        Golem,
        Bomber,
        Count
    }

    private Dictionary<string, int> _unitCosts;
  
    
    
    private const float MaxBattleTime = 120f;
    private float _battleTimer;
    private float _mainGridMaxScore;
    private float _time;
    
    private const int GridWidth = 40;
    private const int GridHeight = 40;
    
    private Grid _mainGrid = new Grid(GridWidth, GridHeight);
    private Grid _gridCopy;
 
    private bool _isSimulationReady;
    
    private readonly System.Random _rand = new System.Random();
    
    private Perturbation _perturbation;
    private Selection _selection;
    private Crossover _crossover;
    private Replacement _replacement;
    

    private readonly DataManager _dataManager = new DataManager();
    
  

    private void Awake()
    {
        gridVisual.Init(_mainGrid);
        buildingSystem.Init(_mainGrid);
        
        _unitCosts = new Dictionary<string, int>
        {
            { "Archer", archerCost },
            { "Golem", golemCost },
            { "Bomber", bomberCost }
        };
        
        
        startUIPanel.SetActive(true);
        battleUIPanel.SetActive(false);
        endUIPanel.SetActive(false);
        
        _isSimulationReady = false;
    }

    private void Start()
    {
        _perturbation = new Perturbation(perturbationType, GridWidth,  GridHeight, maxArmyCost, _unitCosts.Values.Min(),  _unitCosts);
        _selection = new Selection(selectionType);
        _crossover = new Crossover(crossoverType);
        _replacement = new Replacement(replacementType);
    }
    
    private void Update()
    {
        _time = Time.deltaTime;
        if (!_isSimulationReady) return;
        _time = Time.deltaTime;
        _isSimulationReady = SimulateTick(_mainGrid,  _time);
        UpdateVisualScore();
        UpdateTimer(_time);
    }


   

    public void OnStartButtonClick()
    {
        startUIPanel.SetActive(false);
        battleUIPanel.SetActive(true);
        
     
        List<WarriorGenome> army = ChooseArmy();
        List<WarriorGenome> copyTestArmy = army.Select(w => w.Clone()).ToList();
        
        // float fitness = FitnessFunction(army);
        // Debug.Log($"Fitness Result: {fitness}% destroyed");
        
        
        int armyCost = 0;
        foreach (var gene in copyTestArmy)
        {
            GridCell cell = _mainGrid.GetCell(gene.X, gene.Y);
            if (cell.IsOccupied) continue;

            if (gene.Type == "Archer")
            {
                SpawnVisualArcher(gene.X, gene.Y);
                armyCost += archerCost;
            }
            if (gene.Type == "Golem")
            {
                SpawnVisualGolem(gene.X, gene.Y);
                armyCost += golemCost;
            }
            if (gene.Type == "Bomber")
            {
                SpawnVisualBomber(gene.X, gene.Y);
                armyCost += bomberCost;
            }
        }
        
        endCostText.text = endCostText.text = $"${armyCost}";
        battleCostText.text = endCostText.text;
        _battleTimer = MaxBattleTime;
       
        _mainGridMaxScore = 0;
        foreach (var b in _mainGrid.Buildings)
        {
            _mainGridMaxScore += b.ScoreValue;
        }
        UpdateVisualScore();
        _isSimulationReady = true;

        
    }

    private List<WarriorGenome> ChooseArmy()
    {
        List<WarriorGenome> army = new List<WarriorGenome>();
        
        if (isRandom)
        {
            army = GenerateRandomArmy();
        }
        else if (isLocal)
        {
            army = GenerateRandomArmy();
            LocalSearch ls = new LocalSearch(FitnessFunction, army, epochCount, _perturbation, CalculateArmyCost);
            army = ls.Run();
            // List<List<float>> statistic = new List<List<float>>();
            // for (int i = 0; i < 5; i++)
            // {
            //     army = ls.Run();
            //     statistic.Add(new List<float>(ls.BestFitnessHistory));
            // }
            // _dataManager.SaveStatisticsToCsv(statistic, "LocalSearch");
            // _dataManager.SaveStatisticsToCsv(new List<List<float>> { ls.BestFitnessHistory }, "LocalSearch_Fitness");
          
        }
        else if (isEvolutionary)
        {
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm(
                candidateGenerator: GenerateRandomArmy,
                fitnessFunction: FitnessFunction,
                selection: _selection,
                crossover: _crossover,
                perturbation: _perturbation,
                replacement: _replacement,
                populationSize: populationSize,
                epochsCount: epochCount,
                CalculateArmyCost
            );
            army = ea.Run();
            // List<List<float>> statistic = new List<List<float>>();
            // for (int i = 0; i < 5; i++)
            // {
            //     army = ea.Run();
            //     statistic.Add(new List<float>(ea.BestFitnessHistory));
            // }
            // _dataManager.SaveStatisticsToCsv(statistic, "EOA");
        }
        else if (isMemetic)
        {
            MemeticAlgorithm ma = new MemeticAlgorithm(
                candidateGenerator: GenerateRandomArmy,
                fitnessFunction: FitnessFunction,
                selection: _selection,
                crossover: _crossover,
                perturbation: _perturbation,
                replacement: _replacement,
                populationSize: populationSize,
                epochsCount: epochCount, 
                
                lsEpochs: lsEpochs,     
                CalculateArmyCost,
                lsProbability: lsProbability, 
                improveInit: improveInit 
            );

            
            army = ma.Run();
            // List<List<float>> statistic = new List<List<float>>();
            // for (int i = 0; i < 5; i++)
            // {
            //     army = ma.Run();
            //     statistic.Add(new List<float>(ma.BestFitnessHistory));
            //     Debug.Log($"[Memetic Stats] Total Epochs: {epochCount}, Total FFE: {ma.LcCalls}");
            // }
            // _dataManager.SaveStatisticsToCsv(statistic, "MemeticAlgorithm");
        }
        else
        {
            Debug.Log("Hasn't been chosen any algorithm");
        }
        
        return army;
    }

    private List<WarriorGenome> GenerateRandomArmy()
    {
        List<WarriorGenome> army = new List<WarriorGenome>();
        int localArmyCost = _rand.Next(50, maxArmyCost);
        int totalCost = 0;
        while (totalCost <= localArmyCost - 50)
        {
            int type = _rand.Next(0, (int)WarriorTypes.Count);
            
            if (type == (int)WarriorTypes.Archer && totalCost + archerCost <= localArmyCost)
            {
                totalCost += archerCost;
                (int x, int y) = _perturbation.GetRandomSpawnPoint();    
                army.Add(new WarriorGenome(x, y,  "Archer"));
                
            }else if (type == (int)WarriorTypes.Golem && totalCost + golemCost <= localArmyCost)
            {
                totalCost += golemCost;
                (int x, int y) = _perturbation.GetRandomSpawnPoint();    
                army.Add(new WarriorGenome(x, y,  "Golem"));
                
            }else if (type == (int)WarriorTypes.Bomber && totalCost + bomberCost <= localArmyCost)
            {
                totalCost += bomberCost;
                (int x, int y) = _perturbation.GetRandomSpawnPoint();    
                army.Add(new WarriorGenome(x, y,  "Bomber"));
            }
        }
        return army;
    }

   
    
    private void UpdateVisualScore()
    {
        if (_mainGridMaxScore <= 0) return;
    
        float currentScore = 0f;
        foreach (var b in _mainGrid.Buildings)
        {
            if (b.IsDestroyed) 
            {
                currentScore += b.ScoreValue;
            }
        }
    
        float percentage = (currentScore / _mainGridMaxScore) * 100f;
        
        if (battleScoreText)
            battleScoreText.text = $"{percentage:F0}%"; 
    }
    
    private void UpdateTimer(float dt)
    {
        _battleTimer -= dt;
        
        if (_battleTimer <= 0 || !_isSimulationReady)
        {
            float endTime = Mathf.Max(0, _battleTimer);
            _battleTimer = 0;
            _isSimulationReady = false;
            endUIPanel.SetActive(true);
            endTimerText.text =  $"{Mathf.FloorToInt(endTime / 60):00}:{ Mathf.FloorToInt(endTime % 60):00}";
            endScoreText.text = battleScoreText.text;
            battleUIPanel.SetActive(false);
           
        }
        
        int minutes = Mathf.FloorToInt(_battleTimer / 60);
        int seconds = Mathf.FloorToInt(_battleTimer % 60);
        
        battleTimerText.text = $"{minutes:00}:{seconds:00}";
    }



    private Grid CopyGrid(Grid grid)
    {
        Grid newGrid = new Grid(GridWidth, GridHeight);
        
        foreach (var oldBuilding in grid.Buildings)
        {
            if (oldBuilding is Tower t)
            {
                Tower newTower = new Tower(
                    t.SizeX,
                    t.SizeY,
                    t.Health,
                    t.ScoreValue,
                    0f, 
                    t.FireRange, 
                    t.FireRate,
                    t.Damage,
                    newGrid.GetCell(t.OriginCell.X, t.OriginCell.Y),
                    null
                );
            
                newGrid.AddBuilding(newTower);
            }
            else if (oldBuilding is CivilBuilding cb)
            {
                CivilBuilding newCivilBuilding = new CivilBuilding(
                    cb.SizeX,
                    cb.SizeY,
                    cb.Health,
                    newGrid.GetCell(cb.OriginCell.X, cb.OriginCell.Y),
                    cb.ScoreValue,
                    cb.IsWall
                );
                
                newGrid.AddBuilding(newCivilBuilding);
            }
        }
        
        return newGrid;
    }


    private float FitnessFunction(List<WarriorGenome> armyGenome)
    {
        
        _gridCopy = CopyGrid(_mainGrid);
        float maxPossibleScore = 0;
        float armyCost = 0;
        
        for (int i = 0; i < _gridCopy.Buildings.Count; i++)
        {
            maxPossibleScore += _gridCopy.Buildings[i].ScoreValue;
        }
      
        foreach (var gene in armyGenome)
        {
            GridCell cell = _gridCopy.GetCell(gene.X, gene.Y);
            if (cell.IsOccupied) continue;
            
            Warrior warrior = null;
            if (gene.Type == "Archer")
            {
                warrior = new Archer(archerHealth, archerDamage, archerSpeed, archerFireRange, archerFireRate, cell);
                armyCost += archerCost;
            }
            if (gene.Type == "Golem")
            {
                warrior = new Golem(golemHealth, golemDamage, golemSpeed, golemFireRange, golemFireRate, cell);
                armyCost += golemCost;
            }
            if (gene.Type == "Bomber")
            {
                warrior = new Bomber(bomberHealth, bomberDamage, bomberSpeed, bomberFireRange, bomberFireRate, cell);
                armyCost += bomberCost;
            }
            
            if (warrior != null) _gridCopy.AddArmy(warrior);
        }
        
        float simStep = 0.1f;
        int maxTicks = 1200; // 120 seconds
        int ticks = 0;
        bool battleOngoing = true;

        while (battleOngoing && ticks < maxTicks)
        {
            battleOngoing = SimulateTick(_gridCopy, simStep);
            ticks++;
        }
        
        float currentScore = 0f;
        foreach (var b in _gridCopy.Buildings)
        {
            if (b.IsDestroyed) 
            {
                currentScore += b.ScoreValue;
            }
        }

        if (maxPossibleScore <= 0) return 0;
        
        float destructionRatio = currentScore / maxPossibleScore;
        if (destructionRatio < 0.1) return 0;
        float score = destructionRatio * 100f;
        
        // float costRatio = armyCost / maxArmyCost;
        // float costPenalty = costRatio * 10f;
        float costPenalty = armyCost * 0.007f;
        
        return Mathf.Max(0, score - costPenalty);
    }
    
    
    
    
    private bool SimulateTick(Grid grid, float dt)
    {
        int warriorCount = grid.Warriors.Count;
        int buildingCount = grid.Buildings.Count;

        bool anyWarriorAlive = false;
        bool anyBuildingAlive = false;

        for (int i = 0; i < warriorCount; i++)
        {
            var w = grid.Warriors[i];
            w.WarriorUpdate(dt, grid.Buildings, grid);
            if (!w.IsDead) anyWarriorAlive = true;
        }

        for (int i = 0; i < buildingCount; i++)
        {
            var b = grid.Buildings[i];
            b.BuildingUpdate(dt, grid.Warriors);
            if (!b.IsDestroyed && !b.IsWall) anyBuildingAlive = true;
        }
        
        return anyWarriorAlive && anyBuildingAlive;
    }
    
    
    private void SpawnVisualArcher(int x, int y)
    {
        GridCell cell = _mainGrid.GetCell(x, y);
        Archer logic = new Archer(
            health: archerHealth,
            damage: archerDamage,
            speed: archerSpeed,
            fireRange: archerFireRange,
            fireRate: archerFireRate,
            origin: cell
        );

        _mainGrid.AddArmy(logic);
        GameObject go = Instantiate(archerPrefab);
        ArcherVisual view = go.GetComponent<ArcherVisual>();
        view.Bind(logic, gridVisual);
    }
    
    
    private void SpawnVisualGolem(int x, int y)
    {
        GridCell cell = _mainGrid.GetCell(x, y);
        Golem logic = new Golem(
            health: golemHealth,    
            damage: golemDamage,      
            speed: golemSpeed,     
            attackRange: golemFireRange,  
            attackRate: golemFireRate,
            origin: cell
        );
    
        _mainGrid.AddArmy(logic);
        GameObject go = Instantiate(golemPrefab); 
        GolemVisual view = go.GetComponent<GolemVisual>();
        view.Bind(logic, gridVisual);
    }
    
    private void SpawnVisualBomber(int x, int y)
    {
        GridCell cell = _mainGrid.GetCell(x, y);
        Bomber logic = new Bomber(
            health: bomberHealth,     
            damage: bomberDamage,    
            speed: bomberSpeed,    
            attackRange: bomberFireRange, 
            attackRate: bomberFireRate, 
            origin: cell
        );

        _mainGrid.AddArmy(logic);
        GameObject go = Instantiate(bomberPrefab);
        BomberVisual view = go.GetComponent<BomberVisual>();
        view.Bind(logic, gridVisual);
    }


    public void OnEndButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
    
    public int CalculateArmyCost(List<WarriorGenome> army)
    {
        int totalCost = 0;
        foreach (var unit in army)
        {
            switch (unit.Type)
            {
                case "Archer": totalCost += archerCost; break; 
                case "Bomber": totalCost += bomberCost; break;
                case "Golem":  totalCost += golemCost; break;
            }
        }
        return totalCost;
    }

    
    
}
