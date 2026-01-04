using UnityEngine;

public class TowerVisual : BuildingVisual
{
    [SerializeField] private int fireRange;
    [SerializeField] private float fireRate;
    [SerializeField] private int damage;
    
    
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    
    private Tower _tower;
 
    public override Building CreateLogic(GridCell origin)
    {
        _tower = new Tower(
            sizeX,
            sizeY,
            health,
            score,
            0f,
            fireRange,
            fireRate,
            damage,
            origin,
            null
        );
        Logic = _tower;
        base.CreateLogic(origin);
        _tower.OnTowerShoot += OnTowerShoot;
        return Logic;
    }
    
    
    private void OnTowerShoot(Warrior target)
    {
        Vector3 targetPosition = GridToWorld(target.OriginCell.X, 0 , target.OriginCell.Y);
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Vector3 direction = (targetPosition - arrowSpawnPoint.position).normalized;
        arrow.GetComponent<Projectile>().targetDirection = direction;
    }

    private void OnDestroy()
    {
        if (Logic != null)
            _tower.OnTowerShoot -= OnTowerShoot;
    }
    
    

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(gameObject.transform.position, fireRange);
    }
    
    private Vector3 GridToWorld(int x, int z, int y)
    {
        return gridVisual.transform.position + new Vector3(x, z, y);
    }
}
