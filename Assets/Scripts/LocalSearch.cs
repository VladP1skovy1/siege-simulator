using System;
using System.Collections.Generic;
using System.Linq;

public class LocalSearch
{
    private readonly Func<List<WarriorGenome>, float> _fitnessFunction;
    private readonly List<WarriorGenome> _armyGenome;
    private readonly int _epochsCount;
    private readonly Perturbation _perturb;
    private readonly Func<List<WarriorGenome>, int> _costFunc;
    public List<float> BestFitnessHistory { get; private set; }
    public List<int> BestCostHistory { get; private set; }
    
    public LocalSearch(Func<List<WarriorGenome>, float> fitnessFunction, List<WarriorGenome> armyGenome, int epochsCount, Perturbation  perturb, Func<List<WarriorGenome>, int> costFunc)
    {
        _fitnessFunction = fitnessFunction;
        _armyGenome = armyGenome;
        _epochsCount = epochsCount;
        _perturb = perturb;
        _costFunc = costFunc;
        BestFitnessHistory = new List<float>();
        BestCostHistory = new List<int>();
    }

    public List<WarriorGenome> Run()
    {
        List<WarriorGenome> bestArmyGenome = _armyGenome.Select(w => w.Clone()).ToList();
        float bestFitnessValue = _fitnessFunction(bestArmyGenome);
        BestFitnessHistory.Clear();
        BestFitnessHistory.Add(bestFitnessValue);
        BestCostHistory.Add(_costFunc(bestArmyGenome));
        for (int i = 0; i < _epochsCount; i++)
        {
            List<WarriorGenome> newArmyGenome = bestArmyGenome.Select(w => w.Clone()).ToList();
            _perturb.Run(newArmyGenome);
            float newFitnessValue = _fitnessFunction(newArmyGenome);
            if (newFitnessValue > bestFitnessValue)
            {
                bestArmyGenome = newArmyGenome;
                bestFitnessValue = newFitnessValue;
            }
            BestFitnessHistory.Add(bestFitnessValue);
            BestCostHistory.Add(_costFunc(bestArmyGenome));
        }
        
        return bestArmyGenome;
    }
}
