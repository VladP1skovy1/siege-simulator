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
        if (_grid == null)
            return;
        
        Gizmos.color = Color.gray;
        
        for (int y = 0; y < _grid.Height; y++)
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                Vector3 transformPosition = transform.position + transform.forward.normalized * 1f * y;
                Gizmos.DrawLine(transformPosition, transformPosition + transform.right.normalized * 1f * _grid.Height);
                transformPosition = transform.position + transform.right.normalized * 1f * x;
                Gizmos.DrawLine(transformPosition, transformPosition + transform.forward.normalized * 1f * _grid.Width);
            }
        }
    }
}
