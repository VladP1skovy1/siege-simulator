using System.Collections.Generic;
using System.Linq;
using EvaluatedGenome = System.Tuple<System.Collections.Generic.List<WarriorGenome>, float>;

public class Replacement
{
    private readonly string _type;

    public Replacement(string type)
    {
        _type = type;
    }

    public List<List<WarriorGenome>> Run(List<EvaluatedGenome> oldPop, List<List<WarriorGenome>> offspring, int popSize)
    {
        switch (_type)
        {
            case "Elitism":
                return ElitismReplacement(oldPop, offspring, popSize);
            case "Generational":
                return GenerationalReplacement(oldPop, offspring, popSize);
            default:
                return ElitismReplacement(oldPop, offspring, popSize);
        }
    }

    private List<List<WarriorGenome>> ElitismReplacement(List<EvaluatedGenome> oldPop, List<List<WarriorGenome>> offspring, int popSize)
    {
        List<List<WarriorGenome>> newPop = new List<List<WarriorGenome>>();
        var sortedOld = oldPop.OrderByDescending(x => x.Item2).ToList();
        newPop.Add(sortedOld[0].Item1.Select(w => w.Clone()).ToList());
        
        foreach (var child in offspring)
        {
            if (newPop.Count < popSize)
                newPop.Add(child);
            else
                break;
        }
        
        int oldIndex = 1;
        while(newPop.Count < popSize && oldIndex < sortedOld.Count)
        {
            newPop.Add(sortedOld[oldIndex].Item1.Select(w => w.Clone()).ToList());
            oldIndex++;
        }

        return newPop;
    }

    private List<List<WarriorGenome>> GenerationalReplacement(List<EvaluatedGenome> oldPop, List<List<WarriorGenome>> offspring, int popSize)
    {
        return offspring.Take(popSize).ToList();
    }
}