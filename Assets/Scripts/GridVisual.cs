using UnityEngine;
using Color = UnityEngine.Color;


public class GridVisual : MonoBehaviour
{
    private Grid _grid;

    public void Init(Grid grid)
    {
        _grid = grid;
    }
    
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.gray;
        
        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 40; x++)
            {
                Vector3 transformPosition = transform.position + transform.forward.normalized * 1f * y;
                Gizmos.DrawLine(transformPosition, transformPosition + transform.right.normalized * 1f * 40);
                transformPosition = transform.position + transform.right.normalized * 1f * x;
                Gizmos.DrawLine(transformPosition, transformPosition + transform.forward.normalized * 1f * 40);
            }
        }
    }
}
