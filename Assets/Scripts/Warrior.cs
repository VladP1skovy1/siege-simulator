using System.Collections.Generic;
using UnityEngine;

public abstract class Warrior
{
    protected int Health { get; set; }
    protected int Damage { get; private set; }
    protected float Speed { get; private set; }
    protected int AttackRange { get; private set; } 
    protected float AttackRate { get; private set; } 
    
    public bool IsDead { get; private set; }
    public GridCell OriginCell { get; private set; }

   
    protected Building CurrentTarget;
    protected float AttackCoolDown;
    protected float MoveCoolDown;

   
    public event System.Action OnDeath;
    public event System.Action<Building> OnAttack; 
    public event System.Action OnMove;

    protected Warrior(int health, int damage, float speed, int attackRange, float attackRate, GridCell origin)
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
        FindTarget(buildings);

        if (CurrentTarget == null) return;

        int distSq = ComputeDistance(OriginCell.X, OriginCell.Y, CurrentTarget.OriginCell.X, CurrentTarget.OriginCell.Y);
        int rangeSq = AttackRange * AttackRange;

       
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

   

    protected void FindTarget(List<Building> buildings)
    {
        if (CurrentTarget != null && CurrentTarget.IsDestroyed)
            CurrentTarget = null;

        if (CurrentTarget != null) return;

        float nearestDistance = float.MaxValue;
        foreach (var building in buildings)
        {
            if (building.IsDestroyed) continue;

            float distance = ComputeDistance(OriginCell.X, OriginCell.Y, building.OriginCell.X, building.OriginCell.Y);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                CurrentTarget = building;
            }
        }
    }

    protected void Move(Building target, Grid grid)
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

    protected bool TryMove(int newX, int newY, Grid grid)
    {
        if (!grid.IsInside(newX, newY)) return false;
        GridCell targetCell = grid.GetCell(newX, newY);
        if (targetCell.IsOccupied) return false;

        OriginCell.X = targetCell.X;
        OriginCell.Y = targetCell.Y;

        OnMove?.Invoke();
            
        return true;
    }

    protected int ComputeDistance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;
        return dx * dx + dy * dy;
    }
}