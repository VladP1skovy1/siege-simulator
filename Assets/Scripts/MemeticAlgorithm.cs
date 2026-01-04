using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EvaluatedGenome = System.Tuple<System.Collections.Generic.List<WarriorGenome>, float>;

public class MemeticAlgorithm
{
    private readonly Func<List<WarriorGenome>> _candidateGenerator;
    private readonly Func<List<WarriorGenome>, float> _fitnessFunction;
    
    private readonly Selection _selection;
    private readonly Crossover _crossover;
    private readonly Perturbation _perturbation;
    private readonly Replacement _replacement;
    
    private readonly int _populationSize;
    private readonly int _epochsCount;
    
    private readonly int _lsEpochs;
    private readonly float _lsProbability;         
    private readonly bool _improveInit;           
    
    private readonly Func<List<WarriorGenome>, int> _costFunc;
    public List<int> BestCostHistory { get; private set; }

    
    public List<float> BestFitnessHistory { get; private set; }
    public int LcCalls { get; private set; } = 0;

    public MemeticAlgorithm(
        Func<List<WarriorGenome>> candidateGenerator,
        Func<List<WarriorGenome>, float> fitnessFunction,
        Selection selection,
        Crossover crossover,
        Perturbation perturbation, 
        Replacement replacement,
        int populationSize,
        int epochsCount,
        int lsEpochs,
        Func<List<WarriorGenome>, int> costFunc,
        float lsProbability = 0.2f,
        bool improveInit = false)
    {
        _candidateGenerator = candidateGenerator;
        _fitnessFunction = fitnessFunction;
        _selection = selection;
        _crossover = crossover;
        _perturbation = perturbation;
        _replacement = replacement;
        _populationSize = populationSize;
        _epochsCount = epochsCount;

        _lsEpochs = lsEpochs;
        _lsProbability = lsProbability;
        _improveInit = improveInit;
        
        _costFunc = costFunc;
        BestCostHistory = new List<int>();

        BestFitnessHistory = new List<float>();
    }
    
    public List<WarriorGenome> Run()
    {
        LcCalls = 0;
        List<List<WarriorGenome>> population = new List<List<WarriorGenome>>();
        for (int i = 0; i < _populationSize; i++) population.Add(_candidateGenerator());

        if (_improveInit)
        {
            for (int i = 0; i < population.Count; i++)
            {
                LocalSearch ls = new LocalSearch(_fitnessFunction, population[i], _lsEpochs, _perturbation, _costFunc);
                population[i] = ls.Run();
                LcCalls += 1;
            }
        }

        List<WarriorGenome> globalBestGenome = null;
        float globalBestFitness = float.MinValue;
        BestFitnessHistory.Clear();
        BestCostHistory.Clear();

        for (int i = 0; i < _epochsCount; i++)
        {
            var evaluatedPop = population.Select(army => new EvaluatedGenome(army, _fitnessFunction(army))).ToList();
            
            float currentBest = evaluatedPop.Max(x => x.Item2);
            BestFitnessHistory.Add(currentBest);
            var bestInd = evaluatedPop.OrderByDescending(x => x.Item2).First();
            BestCostHistory.Add(_costFunc(bestInd.Item1));
            
            if (currentBest > globalBestFitness)
            {
                globalBestFitness = currentBest;
                var bestArmy = evaluatedPop.First(x => Mathf.Approximately(x.Item2, currentBest)).Item1;
                globalBestGenome = bestArmy.Select(w => w.Clone()).ToList();
            }
            
            List<List<WarriorGenome>> offspring = new List<List<WarriorGenome>>();
            
            while (offspring.Count < _populationSize)
            {
                var parents = _selection.Run(evaluatedPop, 2);
                var child = _crossover.Run(parents[0], parents[1]);
                _perturbation.EnforceBudget(child);
                _perturbation.Run(child);
                
                if (UnityEngine.Random.value < _lsProbability)
                {
                    LocalSearch ls = new LocalSearch(_fitnessFunction, child, _lsEpochs, _perturbation, _costFunc);
                    child = ls.Run();
                    LcCalls += 1;
                }
                
                offspring.Add(child);
            }
            
            population = _replacement.Run(evaluatedPop, offspring, _populationSize);
        }

        return globalBestGenome;
    }
}