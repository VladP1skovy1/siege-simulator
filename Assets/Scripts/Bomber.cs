using System.Collections.Generic;


public class Bomber : Warrior
{
    public Bomber(int health, int damage, float speed, int attackRange, float attackRate, GridCell origin) 
        : base(health, damage, speed, attackRange, attackRate, origin)
    {
    }

    protected override void Think(List<Building> buildings, Grid grid)
    {
        if (CurrentTarget == null || CurrentTarget.IsDestroyed)
        {
            CurrentTarget = null;
            CurrentTarget = FindBuilding(grid, TargetPriority.Walls);

            if (CurrentTarget == null)
            {
                CurrentTarget = FindClosestWallGlobal(buildings);
            }

            if (CurrentTarget == null)
            {
                CurrentTarget = FindBuilding(grid, TargetPriority.Any);
            }
        }

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

    protected override void Attack(Building target)
    {
        base.Attack(target);
        TakeDamage(Health); 
    }

    private Building FindClosestWallGlobal(List<Building> buildings)
    {
        Building best = null;
        int minDistance = int.MaxValue;

        foreach (var b in buildings)
        {
            if (b.IsDestroyed) continue;

            if (!b.IsWall) continue;
            int dist = ComputeDistance(OriginCell.X, OriginCell.Y, b.OriginCell.X, b.OriginCell.Y);
            if (dist >= minDistance) continue;
            minDistance = dist;
            best = b;
        }
        return best;
    }
}