public class GridCell
{
    public int X;
    public int Y;
    public bool IsOccupied;
    public Building BuildingRef;
    
    public GridCell(int x, int y, bool occupied) 
    { 
        X = x; Y = y; IsOccupied = occupied; 
        BuildingRef = null; 
    }
}