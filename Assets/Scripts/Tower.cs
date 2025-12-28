using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    private int FireRange { get; set; }
    private float FireRate { get; set; }
    private int Damage { get; set; }
    
    private Warrior _currentTarget;
    private float _coolDown;
    
    public event System.Action<Warrior> OnTowerShoot;
    
    public Tower(int size, int health, GridCell origin, Warrior currentTarget, float coolDown, int fireRange, float fireRate, int damage) : base(size, health, origin)
    {
        _currentTarget = currentTarget;
        _coolDown = coolDown;
        FireRange = fireRange;
        FireRate = fireRate;
        Damage = damage;
    }

    
    
    
    
    public override void BuildingUpdate(float dt, List<Warrior> warriors)
    {
        if (IsDestroyed)
        {
            return;
        }
        
        _coolDown -= dt;
        FindTarget(warriors);
       
        
        if (_currentTarget != null && _coolDown <= 0)
        {
            Attack(_currentTarget);
            _coolDown += FireRate;
        }
    }


    private void Attack(Warrior target)
    {
        target.TakeDamage(Damage);
        OnTowerShoot?.Invoke(target);
    }

    private void FindTarget(List<Warrior> warriors)
    {
        if (_currentTarget != null)
        {
            if (_currentTarget.IsDead || (ComputeDistance(this.OriginCell.X, this.OriginCell.Y,
                    _currentTarget.OriginCell.X, _currentTarget.OriginCell.Y) > FireRange))
            {
                _currentTarget = null;
            }
        }

        if (_currentTarget == null)
        {
            int armyCount = warriors.Count;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < armyCount; i++)
            {
                if (warriors[i].IsDead)
                {
                    continue;
                }
                float distance = ComputeDistance(this.OriginCell.X, this.OriginCell.Y, warriors[i].OriginCell.X, warriors[i].OriginCell.Y);
                if (distance < nearestDistance && distance < FireRange )
                {
                    nearestDistance = distance;
                    _currentTarget = warriors[i];
                }
            }
        }
    }
    
    private float ComputeDistance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    

}
