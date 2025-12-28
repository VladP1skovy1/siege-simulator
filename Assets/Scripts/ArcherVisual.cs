using UnityEngine;

public class ArcherVisual : WarriorVisual
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
  
    public override void Bind(Warrior logic, GridVisual gridVisual)
    {
        base.Bind(logic, gridVisual); 
     
        if (Logic != null) 
        {
            Logic.OnAttack += OnArcherAttackVisual;
        }
    }

    private void OnArcherAttackVisual(Building target)
    {
        Vector3 targetPosition = GridToWorld(target.OriginCell.X, 0 , target.OriginCell.Y);
        
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Vector3 direction = (targetPosition - arrowSpawnPoint.position).normalized;
        arrow.GetComponent<Projectile>().targetDirection = direction; 
    }
    
    protected override void OnDestroy() 
    {
        if (Logic != null)
        {
            Logic.OnAttack -= OnArcherAttackVisual;
        }
        
        base.OnDestroy(); 
    }
    
    
    private Vector3 GridToWorld(int x, int z, int y)
    {
        return GridVisual.transform.position + new Vector3(x, z, y);
    }
}
