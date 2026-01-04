using System;
using System.Collections.Generic;
using System.Linq;

public class Perturbation
{
    private readonly int _gridWidth;
    private readonly int _gridHeight;
    private readonly int _maxCost;
    private readonly int _minCost;
    private readonly string _type;
    
    private readonly Dictionary<string, int> _unitCosts;
    private readonly string[] _unitTypes;
    private readonly Random _rand = new Random();

    public Perturbation(string type, int width, int height, int maxCost, int minCost, Dictionary<string, int> unitCosts)
    {
        _type = type;
        _gridWidth = width;
        _gridHeight = height;
        _maxCost = maxCost;
        _minCost = minCost;
        _unitCosts = unitCosts;
        _unitTypes = unitCosts.Keys.ToArray();
    }

    public void Run(List<WarriorGenome> armyGenome)
    {
        switch (_type)
        {
            case "Add":
                AddRandomWarrior(armyGenome);
                break;
            case "Remove":
                RemoveRandomWarrior(armyGenome);
                break;
            case "Swap":
                SwapRandomWarriorType(armyGenome);
                break;
            case "Move":
                ChangeRandomWarriorPosition(armyGenome);
                break;
            case "Mix":
                Mix(armyGenome);
                break;
        }
                
    }

    private int CalculateTotalCost(List<WarriorGenome> army)
    {
        int total = 0;
        foreach (var w in army)
        {
            if (_unitCosts.TryGetValue(w.Type, out int cost))
                total += cost;
        }
        return total;
    }

    private void AddRandomWarrior(List<WarriorGenome> armyGenome)
    {
        string typeToAdd = _unitTypes[_rand.Next(_unitTypes.Length)];
        int costToAdd = _unitCosts[typeToAdd];
        
        int currentCost = CalculateTotalCost(armyGenome);
        
        if (currentCost + costToAdd <= _maxCost)
        {
            (int x, int y) = GetRandomSpawnPoint();
            
            armyGenome.Add(new WarriorGenome(x, y, typeToAdd));
        }
    }

    private void RemoveRandomWarrior(List<WarriorGenome> armyGenome)
    {
        if (armyGenome.Count == 0) return;
        int index = _rand.Next(armyGenome.Count);
        WarriorGenome warriorToRemove = armyGenome[index];
        int costToRemove = _unitCosts[warriorToRemove.Type];
        int currentCost = CalculateTotalCost(armyGenome);
        if (currentCost - costToRemove >= _minCost)
        {
            armyGenome.RemoveAt(index);
        }
    }

    private void SwapRandomWarriorType(List<WarriorGenome> armyGenome)
    {
        if (armyGenome.Count == 0) return;

        int index = _rand.Next(armyGenome.Count);
        WarriorGenome oldWarrior = armyGenome[index];
        
        string newType = _unitTypes[_rand.Next(_unitTypes.Length)];
        while (oldWarrior.Type == newType)
        {
            newType = _unitTypes[_rand.Next(_unitTypes.Length)];
        }
   
        int oldCost = _unitCosts[oldWarrior.Type];
        int newCost = _unitCosts[newType];
        int currentCost = CalculateTotalCost(armyGenome);
        
        if (currentCost - oldCost + newCost <= _maxCost)
        {
           
            armyGenome[index].Type = newType;
        }
    }

    private void ChangeRandomWarriorPosition(List<WarriorGenome> armyGenome)
    {
        if (armyGenome.Count == 0) return;

        int index = _rand.Next(armyGenome.Count);
  
        (int newX, int newY) = GetRandomSpawnPoint();
        
        armyGenome[index].X = newX;
        armyGenome[index].Y = newY;
    }

    private void Mix(List<WarriorGenome> armyGenome)
    {
        int action = _rand.Next(0, 4);
        if (armyGenome.Count == 0) action = 0; 

        switch (action)
        {
            case 0: AddRandomWarrior(armyGenome); break;
            case 1: RemoveRandomWarrior(armyGenome); break;
            case 2: SwapRandomWarriorType(armyGenome); break;
            case 3: ChangeRandomWarriorPosition(armyGenome); break;
        }
    }
    
    public (int, int) GetRandomSpawnPoint()
    {
        int x = 0, y = 0;
        int mapSide = _rand.Next(0, 4); // North, South, West, East
        switch (mapSide)
        {
            case 0: // North
                y = 0;
                x = _rand.Next(0, _gridWidth);
                break;
            case 1: // South
                y = _gridHeight - 1;
                x = _rand.Next(0, _gridWidth);
                break;
            case 2: // West
                x = 0;
                y = _rand.Next(0, _gridHeight);
                break;
            case 3: // East
                x = _gridWidth - 1;
                y = _rand.Next(0, _gridHeight);
                break;
        }
        return (x, y);
    }
    
    public void EnforceBudget(List<WarriorGenome> armyGenome)
    {
        while (CalculateTotalCost(armyGenome) > _maxCost && armyGenome.Count > 0)
        {
            int indexToRemove = _rand.Next(armyGenome.Count);
            armyGenome.RemoveAt(indexToRemove);
        }
    }

    
}