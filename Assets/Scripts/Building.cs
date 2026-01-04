using System.Collections.Generic;

public abstract class Building
{
   public int Health { get; private set; }
   public int SizeX { get; private set; }
   public int SizeY { get; private set; }
   public int ScoreValue { get; private set; }
   public bool IsDestroyed { get; private set; }
   public bool IsWall { get; private set; }
   public GridCell OriginCell { get; private set; }
   
   public virtual bool IsDefense => false;
   
   public event System.Action OnDestroyed;
   
   protected Building(int sizeX, int sizeY, int health, int scoreValue, bool isWall, GridCell origin)
   {
      SizeX= sizeX;
      SizeY= sizeY;
      Health = health;
      OriginCell = origin;
      ScoreValue = scoreValue;
      IsWall = isWall;
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
