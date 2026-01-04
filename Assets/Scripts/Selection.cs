using System;
using System.Collections.Generic;
using System.Linq;

using EvaluatedGenome = System.Tuple<System.Collections.Generic.List<WarriorGenome>, float>;

public class Selection
{
    private readonly string _type;
    private readonly Random _rand = new Random();

    public Selection(string type)
    {
        _type = type;
    }

    public List<List<WarriorGenome>> Run(List<EvaluatedGenome> evaluatedPop, int count)
    {
        switch (_type)
        {
            case "Tournament":
                return TournamentSelection(evaluatedPop, count);
            case "Roulette":
                return RouletteSelection(evaluatedPop, count);
            case "Random":
                return RandomSelection(evaluatedPop, count);
            default:
                return TournamentSelection(evaluatedPop, count);
        }
    }

 
    private List<List<WarriorGenome>> TournamentSelection(List<EvaluatedGenome> population, int count)
    {
        List<List<WarriorGenome>> parents = new List<List<WarriorGenome>>();
        int tournamentSize = 3;

        for (int i = 0; i < count; i++)
        {
            var best = population[_rand.Next(population.Count)];
            for (int j = 0; j < tournamentSize - 1; j++)
            {
                var challenger = population[_rand.Next(population.Count)];
                if (challenger.Item2 > best.Item2) best = challenger;
            }
            parents.Add(best.Item1);
        }
        return parents;
    }

    private List<List<WarriorGenome>> RouletteSelection(List<EvaluatedGenome> population, int count)
    {
        List<List<WarriorGenome>> parents = new List<List<WarriorGenome>>();
        float totalFitness = population.Sum(x => Math.Max(0, x.Item2));

        for (int i = 0; i < count; i++)
        {
            float randomPoint = (float)_rand.NextDouble() * totalFitness;
            float currentSum = 0;
            foreach (var ind in population)
            {
                currentSum += Math.Max(0, ind.Item2);
                if (currentSum >= randomPoint)
                {
                    parents.Add(ind.Item1);
                    break;
                }
            }
        }
        return parents;
    }
    
    private List<List<WarriorGenome>> RandomSelection(List<EvaluatedGenome> population, int count)
    {
        List<List<WarriorGenome>> parents = new List<List<WarriorGenome>>();
        for (int i = 0; i < count; i++)
        {
            parents.Add(population[_rand.Next(population.Count)].Item1);
        }
        return parents;
    }
}