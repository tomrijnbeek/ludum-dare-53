using UnityEngine;

public sealed class Building : MonoBehaviour
{
    [Readonly] private CityMap map;
    [Readonly] private Vector3Int logicalTile;
    [Readonly] private Vector3Int roadTile;

    public Vector3Int LogicalTile => logicalTile;
    public Vector3Int RoadTile => roadTile;

    private void Start()
    {
        DeliveryScheduler.Instance.RegisterBuilding(this);
    }

    public void UpdateLocation(CityMap map, Vector3Int logicalTile, Vector3Int roadTile)
    {
        this.map = map;
        this.logicalTile = logicalTile;
        this.roadTile = roadTile;
    }
}
