using UnityEngine;

public abstract class BuildingVisual : MonoBehaviour
{
    [SerializeField] protected int sizeX;
    [SerializeField] protected int sizeY;
    [SerializeField] protected int health ;
    [SerializeField] protected int score;
    [SerializeField] private GameObject intactModel;
    [SerializeField] private GameObject destroyedModel;
    [SerializeField] protected GridVisual gridVisual;
    
    private protected Building Logic;
    public int SizeX => sizeX;
    public int SizeY => sizeY;
    public int Score => score;

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
