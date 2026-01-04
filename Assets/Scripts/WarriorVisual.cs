using UnityEngine;

public class WarriorVisual : MonoBehaviour
{
    [SerializeField] private GameObject aliveModel;
    [SerializeField] private GameObject deadModel;
    [SerializeField] private float size;
    
    
    [SerializeField] private float visualMoveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;
    
    protected Warrior Logic;
    protected GridVisual GridVisual;
    
    private Vector3 _targetPosition;
    
    
    public virtual void Bind(Warrior logic, GridVisual gridVisual)
    {
        Logic = logic;
        GridVisual = gridVisual;
        
        Logic.OnDeath += OnDeath;
        Logic.OnMove += OnMoveVisual;
        Logic.OnAttack += OnAttackBaseVisual;
        
        aliveModel.SetActive(true);
        deadModel.SetActive(false);
        
        // if (animator == null) animator = GetComponentInChildren<Animator>();
        
        _targetPosition = CalculateWorldPosition();
        transform.position = _targetPosition;
    }

    private void Update()
    {
        if (Logic == null) return;
        
        Vector3 targetPosition = CalculateWorldPosition();
        float distance = Vector3.Distance(transform.position, targetPosition);
        bool isMoving = distance > 0.05f;

        if (isMoving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, visualMoveSpeed * Time.deltaTime);
        }

        // if (animator != null)
        // {
        //     animator.SetBool("IsMoving", isMoving);
        // }
    }
    
    private void OnAttackBaseVisual(Building target)
    {
        Vector3 targetPos = GridVisual.transform.position + new Vector3(target.OriginCell.X, size, target.OriginCell.Y);
        Vector3 dir = (targetPos - transform.position).normalized;
        
        if (dir != Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(dir);

        
        // if (animator != null)
        // {
        //     animator.SetTrigger("Attack");
        // }
    }
    
    
    
    private void OnMoveVisual()
    {
        _targetPosition = CalculateWorldPosition();
    }
    
    private void OnDeath()
    {
        aliveModel.SetActive(false);
        deadModel.SetActive(true);
    }
    
    protected virtual void OnDestroy()
    {
        if (Logic != null)
        {
            Logic.OnDeath -= OnDeath;
            Logic.OnMove -= OnMoveVisual;
            Logic.OnAttack -= OnAttackBaseVisual;
        }
    }
    protected Vector3 CalculateWorldPosition()
    {
        return GridVisual.transform.position + new Vector3(Logic.OriginCell.X + 0.5f, size, Logic.OriginCell.Y + 0.5f);
    }
}