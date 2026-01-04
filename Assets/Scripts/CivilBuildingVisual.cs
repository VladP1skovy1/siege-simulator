using UnityEngine;

public class CivilBuildingVisual : BuildingVisual
{
    [SerializeField] private bool isWall = false;
    private CivilBuildingVisual _civilBuildingVisual;
 
    public override Building CreateLogic(GridCell origin)
    {
        Logic = new CivilBuilding(sizeX, sizeY, health, origin, Score, isWall);
        
        base.CreateLogic(origin);
        
        return Logic;   
    }
}