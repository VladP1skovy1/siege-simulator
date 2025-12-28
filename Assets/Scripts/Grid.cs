using System.Collections.Generic;


public class Grid
{
    public int Width { get;} 
    public int Height { get;} 
    public List<Building> Buildings { get;}
    public List<Warrior> Warriors { get;}
    
    private GridCell[,] _grid;

    
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        
        Buildings = new List<Building>();
        Warriors = new List<Warrior>();
        
        _grid = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[x, y] = new GridCell(x, y, false);
            }
        }
    }
    

    public void AddBuilding(Building building)
    {
        Buildings.Add(building);
        SetCellsOccupied(building, true);
    }
    
    public void RemoveBuilding(Building building)
    {
        Buildings.Remove(building);
        SetCellsOccupied(building, false);
    }
  
    
    private void SetCellsOccupied(Building building, bool isOccupied)
    {
        int half = building.Size / 2;

        for (int dx = -half; dx <= half; dx++)
        for (int dy = -half; dy <= half; dy++)
        {
            int cx = building.OriginCell.X + dx;
            int cy = building.OriginCell.Y + dy;
            
            
            if (!IsInside(cx, cy))
                continue;

            _grid[cx, cy].IsOccupied = isOccupied;
        }
    }
    
    public bool CanPlace(int x, int y, int size)
    {
        int half = size / 2;

        for (int dx = -half; dx <= half; dx++)
        for (int dy = -half; dy <= half; dy++)
        {
            int cx = x + dx;
            int cy = y + dy;

            if (!IsInside(cx, cy)) return false;
            if (_grid[cx, cy].IsOccupied) return false;
        }

        return true;
    }
    
    public bool IsInside(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

  


    public void RemoveWarrior(Warrior warrior)
    {
        Warriors.Remove(warrior);
    }
    

    public void AddArmy(Warrior warrior)
    {
        Warriors.Add(warrior);
    }
    
    
    public GridCell GetCell(int x, int y)
    {
        return _grid[x, y];
    }

    
    

  
  
    
   

}
