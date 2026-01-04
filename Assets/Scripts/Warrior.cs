using System.Collections.Generic;
using UnityEngine;

public enum TargetPriority
{
    Any, 
    Defense, 
    Walls  
}


public abstract class Warrior
{
    protected int Health { get; set; }
    private int Damage { get; set; }
    protected float Speed { get; private set; }
    protected float AttackRange { get; private set; } 
    protected float AttackRate { get; private set; } 
    
    public bool IsDead { get; private set; }
    public GridCell OriginCell { get; private set; }

   
    protected Building CurrentTarget;
    protected float AttackCoolDown;
    protected float MoveCoolDown;

   
    public event System.Action OnDeath;
    public event System.Action<Building> OnAttack; 
    public event System.Action OnMove;

    protected Warrior(int health, int damage, float speed, float attackRange, float attackRate, GridCell origin)
    {
        Health = health;
        Damage = damage;
        Speed = speed;
        AttackRange = attackRange;
        AttackRate = attackRate;
        OriginCell = origin;
        IsDead = false;
    }

   
    public virtual void WarriorUpdate(float dt, List<Building> buildings, Grid grid)
    {
        if (IsDead) return;

        AttackCoolDown -= dt;
        MoveCoolDown -= dt;

        Think(buildings, grid); 
    }
    
    protected virtual void Think(List<Building> buildings, Grid grid)
    {
        FindTarget(buildings,  grid);

        if (CurrentTarget == null) return;

        int distSq = ComputeDistance(OriginCell.X, OriginCell.Y, CurrentTarget.OriginCell.X, CurrentTarget.OriginCell.Y);
        float rangeSq = AttackRange * AttackRange;

       
        if (distSq <= rangeSq)
        {
            if (AttackCoolDown <= 0)
            {
                Attack(CurrentTarget);
                AttackCoolDown = AttackRate;
            }
        }
        else 
        {
            if (MoveCoolDown <= 0)
            {
                Move(CurrentTarget, grid);
                MoveCoolDown = Speed;
            }
        }
    }

    protected virtual void Attack(Building target)
    {
        target.TakeDamage(Damage);
        OnAttack?.Invoke(target); 
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health > 0) return;
        IsDead = true;
        OnDeath?.Invoke();
    }


    private void FindTarget(List<Building> buildings, Grid grid)
    {
        if (CurrentTarget != null && CurrentTarget.IsDestroyed) CurrentTarget = null;
        if (CurrentTarget != null) return;
        
        CurrentTarget = FindBuilding(grid, TargetPriority.Any);
        
        if (CurrentTarget == null)
        {
            CurrentTarget = FindWall(buildings);
        }
    }


    protected Building FindBuilding(Grid grid, TargetPriority priority)
    {
        Queue<GridCell> queue = new Queue<GridCell>();
        HashSet<GridCell> visited = new HashSet<GridCell>();
        
        queue.Enqueue(OriginCell);
        visited.Add(OriginCell);
        int safetyCounter = 0; 

        while (queue.Count > 0 && safetyCounter < 4000)
        {
            GridCell current = queue.Dequeue();
            safetyCounter++;

            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = current.X + dx[i];
                int ny = current.Y + dy[i];

                if (!grid.IsInside(nx, ny)) continue;
                GridCell nextCell = grid.GetCell(nx, ny);

                if (visited.Contains(nextCell)) continue;
                visited.Add(nextCell);
                
                if (nextCell.IsOccupied && nextCell.BuildingRef != null)
                {
                    Building b = nextCell.BuildingRef;
                    if (b.IsDestroyed) continue;
                    
                    if (priority == TargetPriority.Walls)
                    {
                        if (b.IsWall) return b; 
                       
                    }
                    else if (priority == TargetPriority.Defense)
                    {
                        if (b.IsWall) continue;
                        if (b.IsDefense) return b;
                    }
                    else 
                    {
                        if (b.IsWall) continue; 
                        return b; 
                    }
                }
                else 
                {
                    queue.Enqueue(nextCell);
                }
            }
        }
        return null; 
    }

    private Building FindWall(List<Building> buildings)
    {
        Building best = null;
        float minDist = float.MaxValue;

        foreach (var b in buildings)
        {
            if (b.IsDestroyed) continue;
            
            float d = ComputeDistance(OriginCell.X, OriginCell.Y, b.OriginCell.X, b.OriginCell.Y);
            if (!(d < minDist)) continue;
            minDist = d;
            best = b;
        }
        return best;
    }
    

    protected virtual void Move(Building target, Grid grid)
    {
        int dx = target.OriginCell.X - OriginCell.X;
        int dy = target.OriginCell.Y - OriginCell.Y;
    
        int stepX = dx == 0 ? 0 : dx > 0 ? 1 : -1;
        int stepY = dy == 0 ? 0 : dy > 0 ? 1 : -1;
        
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            if (TryMove(OriginCell.X + stepX, OriginCell.Y, grid)) return;
            
            TryMove(OriginCell.X, OriginCell.Y + stepY, grid);
        }
        else
        {
            if (TryMove(OriginCell.X, OriginCell.Y + stepY, grid)) return;
            TryMove(OriginCell.X + stepX, OriginCell.Y, grid);
        }
    }

    private bool TryMove(int newX, int newY, Grid grid)
    {
        // if (!grid.IsInside(newX, newY)) return false;
        //
        // GridCell targetCell = grid.GetCell(newX, newY);
        //
        // if (targetCell.IsOccupied) 
        // {
        //     
        //     if (targetCell.BuildingRef != null && !targetCell.BuildingRef.IsDestroyed)
        //     {
        //         CurrentTarget = targetCell.BuildingRef;
        //     }
        //     return false;
        // }
        //
        // OriginCell = targetCell;
        // OnMove?.Invoke();
        // return true;
        
        if (!grid.IsInside(newX, newY)) return false; 

        GridCell targetCell = grid.GetCell(newX, newY);

        // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
        // Если клетка занята, НО здание на ней уже уничтожено (или null) — считаем путь свободным
        bool isBlocked = targetCell.IsOccupied;
        if (isBlocked && targetCell.BuildingRef != null && targetCell.BuildingRef.IsDestroyed)
        {
            isBlocked = false; // "Призрак" не блокирует путь
        }

        if (isBlocked) 
        {
            // Если мы врезались в ЖИВОЕ здание
            if (targetCell.BuildingRef != null && !targetCell.BuildingRef.IsDestroyed)
            {
                if (CurrentTarget == targetCell.BuildingRef) return false;
                CurrentTarget = targetCell.BuildingRef;
            }
            return false; 
        }
    
        // Путь свободен
        OriginCell = targetCell;
        OnMove?.Invoke();
        return true;
    }

    // protected bool TryMove(int newX, int newY, Grid grid)
    // {
    //     if (!grid.IsInside(newX, newY)) return false;
    //     GridCell targetCell = grid.GetCell(newX, newY);
    //     if (targetCell.IsOccupied) return false;
    //     
    //     OriginCell = targetCell;
    //
    //     OnMove?.Invoke();
    //         
    //     return true;
    // }

    protected int ComputeDistance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;
        return dx * dx + dy * dy;
    }
}