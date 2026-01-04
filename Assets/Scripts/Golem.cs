using System.Collections.Generic;
using UnityEngine;

public class Golem : Warrior
{
    public Golem(int health, int damage, float speed, int attackRange, float attackRate, GridCell origin) 
        : base(health, damage, speed, attackRange, attackRate, origin)
    {
    }
    
    protected override void Think(List<Building> buildings, Grid grid)
    {
        if (CurrentTarget != null && CurrentTarget.IsDestroyed) CurrentTarget = null;

        if (CurrentTarget == null)
        {
            CurrentTarget = FindBuilding(grid, TargetPriority.Defense);
    
            if (CurrentTarget == null)
            {
                Building dreamTarget = FindClosestDefenseGlobal(buildings);

                if (dreamTarget != null)
                {
                    CurrentTarget = FindBestObstacleToBreak(buildings, dreamTarget);
                }
            }
          
            if (CurrentTarget == null)
            {
                CurrentTarget = FindBuilding(grid, TargetPriority.Any);
            }

            if (CurrentTarget == null)
            {
                CurrentTarget = FindClosestBuildingGlobal(buildings);
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
    

    private Building FindClosestDefenseGlobal(List<Building> buildings)
    {
        Building best = null;
        int minDistance = int.MaxValue;

        foreach (var b in buildings)
        {
            if (b.IsDestroyed) continue;
            if (!b.IsDefense) continue; 

            int dist = ComputeDistance(OriginCell.X, OriginCell.Y, b.OriginCell.X, b.OriginCell.Y);
            if (dist < minDistance)
            {
                minDistance = dist;
                best = b;
            }
        }
        return best;
    }

    private Building FindBestObstacleToBreak(List<Building> buildings, Building finalTarget)
    {
        Building bestObstacle = null;
        float minScore = float.MaxValue;

        foreach (var b in buildings)
        {
            if (b.IsDestroyed) continue;
          
            if (b == finalTarget) continue; 
            
            int distToMe = ComputeDistance(OriginCell.X, OriginCell.Y, b.OriginCell.X, b.OriginCell.Y);
            if (distToMe <= AttackRange * AttackRange)
            {
                 return b; 
            }
            float distMeToObj = Mathf.Sqrt(distToMe);
            float distObjToTarget = Mathf.Sqrt(ComputeDistance(b.OriginCell.X, b.OriginCell.Y, finalTarget.OriginCell.X, finalTarget.OriginCell.Y));
            
            float score = distMeToObj + distObjToTarget;

            if (score < minScore)
            {
                minScore = score;
                bestObstacle = b;
            }
        }
        return bestObstacle;
    }
    
    private Building FindClosestBuildingGlobal(List<Building> buildings)
    {
        Building best = null;
        int minDistance = int.MaxValue;

        foreach (var b in buildings)
        {
            if (b.IsDestroyed) continue;

            int dist = ComputeDistance(OriginCell.X, OriginCell.Y, b.OriginCell.X, b.OriginCell.Y);
            if (dist < minDistance)
            {
                minDistance = dist;
                best = b;
            }
        }
        return best;
    }
}