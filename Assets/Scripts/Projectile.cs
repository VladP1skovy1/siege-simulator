using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    public Vector3 targetDirection;
    
    private void Update()
    {
        transform.position += targetDirection * (speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
