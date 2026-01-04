public class WarriorGenome
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type {get; set;}

    public WarriorGenome(int x, int y, string type)
    {
        X = x;
        Y = y;
        Type = type;
    }
    
    public WarriorGenome Clone()
    {
        return new WarriorGenome(X, Y, Type);
    }

}
