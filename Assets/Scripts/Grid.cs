using System.Collections.Generic;
using UnityEngine;


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
        
        building.OnDestroyed += () => SetCellsOccupied(building, false);
    }
    
    public void RemoveBuilding(Building building)
    {
        SetCellsOccupied(building, false);
    }
  
    
    private void SetCellsOccupied(Building building, bool isOccupied)
    {
        int halfX = building.SizeX / 2;
        int halfY = building.SizeY / 2;

        for (int dx = -halfX; dx < -halfX + building.SizeX; dx++)
        {
            for (int dy = -halfY; dy < -halfY + building.SizeY; dy++)
            {
                int cx = building.OriginCell.X + dx;
                int cy = building.OriginCell.Y + dy;
            
                if (IsInside(cx, cy))
                {
                    _grid[cx, cy].IsOccupied = isOccupied;
                    _grid[cx, cy].BuildingRef = isOccupied ? building : null;
                }
            }
        }
    }
    
    public bool CanPlace(int x, int y, int sizeX, int sizeY)
    {
        int halfX = sizeX / 2;
        int halfY = sizeY / 2;

        for (int dx = -halfX; dx < -halfX + sizeX; dx++)
        {
            for (int dy = -halfY; dy < -halfY + sizeY; dy++)
            {
                int cx = x + dx;
                int cy = y + dy;

                if (!IsInside(cx, cy)) return false;
                if (_grid[cx, cy].IsOccupied) return false;
            }
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
    
    
    
    public bool HasLineOfSight(int x0, int y0, int x1, int y1)
    {
        // Алгоритм Брезенхема для проверки линии на сетке
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // 1. Если мы дошли до клетки цели — значит путь чист
            if (x0 == x1 && y0 == y1) 
            {
                return true;
            }

            // 2. Проверка текущей клетки на наличие СТЕНЫ
            // (Мы пропускаем проверку, если это стартовая клетка, чтобы юнит не блокировал сам себя, 
            // хотя юниты обычно не стоят ВНУТРИ стен)
            GridCell cell = GetCell(x0, y0);
        
            if (cell.IsOccupied && cell.BuildingRef != null)
            {
                // Если на пути стена — обзор перекрыт
                if (cell.BuildingRef.IsWall)
                {
                    return false;
                }
                // Опционально: Если вы хотите, чтобы ОБЫЧНЫЕ здания тоже блокировали стрельбу,
                // уберите проверку .IsWall. Но обычно стреляют через здания, но не через стены.
            }

            // 3. Переход к следующей клетке
            int e2 = 2 * err;
            if (e2 > -dy) 
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) 
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    
    

  
  
    
   

}
