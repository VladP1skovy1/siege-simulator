using System.Collections.Generic;

public abstract class Building
{
   private int Health { get; set; }
   public int Size { get; private set; }
   public bool IsDestroyed { get; private set; }
   public GridCell OriginCell { get; private set; }
   
   public event System.Action OnDestroyed;
   
   protected Building(int size, int health, GridCell origin)
   {
      Size = size;
      Health = health;
      OriginCell = origin;
      IsDestroyed =  false;
   }
   public virtual void BuildingUpdate(float dt, List<Warrior> warriors)
   {
   }
   
   public void TakeDamage(int damage)
   {
      Health -= damage;
      if (Health > 0) return;
      IsDestroyed = true;
      OnDestroyed?.Invoke();

   }
   
}
