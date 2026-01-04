using UnityEngine;

public class GolemVisual : WarriorVisual
{
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";

    public override void Bind(Warrior logic, GridVisual gridVisual)
    {
        base.Bind(logic, gridVisual);

        if (Logic != null)
        {
            Logic.OnAttack += OnGolemAttack;
        }
    }

    private void OnGolemAttack(Building target)
    {
        if (animator != null)
        {
            animator.SetTrigger(attackTriggerName);
        }
        
        // Тут можно также добавить звуки или эффекты удара
    }

    protected override void OnDestroy()
    {
        if (Logic != null)
        {
            Logic.OnAttack -= OnGolemAttack;
        }
        
        base.OnDestroy();
    }
}