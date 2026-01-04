using System;
using System.Collections.Generic;

public class Crossover
{
    private readonly string _type;
    private readonly Random _rand = new Random();

    public Crossover(string type)
    {
        _type = type;
    }

    public List<WarriorGenome> Run(List<WarriorGenome> parent1, List<WarriorGenome> parent2)
    {
        switch (_type)
        {
            case "Uniform":
                return UniformCrossover(parent1, parent2);
            case "OnePoint":
                return OnePointCrossover(parent1, parent2);
            default:
                return UniformCrossover(parent1, parent2);
        }
    }
    
    private List<WarriorGenome> UniformCrossover(List<WarriorGenome> p1, List<WarriorGenome> p2)
    {
        List<WarriorGenome> child = new List<WarriorGenome>();
        int maxLength = Math.Max(p1.Count, p2.Count);

        for (int i = 0; i < maxLength; i++)
        {
            bool hasP1 = i < p1.Count;
            bool hasP2 = i < p2.Count;

            if (hasP1 && hasP2)
            {
                if (_rand.NextDouble() > 0.5)
                    child.Add(p1[i].Clone());
                else
                    child.Add(p2[i].Clone());
            }
            else if (hasP1)
            {
                if (_rand.NextDouble() > 0.5) 
                    child.Add(p1[i].Clone());
            }
            else if (hasP2)
            {
                if (_rand.NextDouble() > 0.5) 
                    child.Add(p2[i].Clone());
            }
        }
        return child;
    }
    
    private List<WarriorGenome> OnePointCrossover(List<WarriorGenome> p1, List<WarriorGenome> p2)
    {
        List<WarriorGenome> child = new List<WarriorGenome>();
        int length = Math.Min(p1.Count, p2.Count);
        int splitPoint = _rand.Next(0, length);

        for (int i = 0; i < length; i++)
        {
            if (i < splitPoint) child.Add(p1[i].Clone());
            else child.Add(p2[i].Clone());
        }
        return child;
    }
}