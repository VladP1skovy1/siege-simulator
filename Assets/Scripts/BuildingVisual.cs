using UnityEngine;

public abstract class BuildingVisual : MonoBehaviour
{
    [SerializeField] protected int size;
    [SerializeField] protected int health ;
    [SerializeField] private GameObject intactModel;
    [SerializeField] private GameObject destroyedModel;
    [SerializeField] protected GridVisual gridVisual;
    
    private protected Building Logic;
    public int Size => size;

    public virtual Building CreateLogic(GridCell origin)
    {
        Logic.OnDestroyed += OnDestroyed;
        intactModel.SetActive(true);
        destroyedModel.SetActive(false);
        return Logic;
    }
    
    
    private void OnDestroyed()
    {
        intactModel.SetActive(false);
        destroyedModel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Logic != null)
            Logic.OnDestroyed -= OnDestroyed;
    }
}
