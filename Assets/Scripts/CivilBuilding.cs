
public class CivilBuilding : Building
{
    public CivilBuilding(int sizeX, int sizeY, int health, GridCell origin, int scoreValue, bool isWall) 
        : base(sizeX, sizeY, health, scoreValue, isWall, origin)
    {
    }
}