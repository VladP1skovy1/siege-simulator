using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    public Vector3 targetDirection;
    
    
    private void Start()
    {
        Destroy(gameObject, 1.0f); 
    }
    
    private void Update()
    {
        transform.position += targetDirection * (speed * Time.deltaTime);
    }
 
}
