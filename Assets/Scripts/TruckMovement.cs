using UnityEngine;

public sealed class TruckMovement : MonoBehaviour
{
    [SerializeField] private Truck truck;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private new Camera camera;

    private Pathfinder pathfinder;

    private void Start()
    {
        pathfinder = new Pathfinder(cityMap);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && tryFindTile(out var tile))
        {
            attemptTruckMove(tile);
        }
    }

    private bool tryFindTile(out Vector3Int tile)
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, 0);
        if (plane.Raycast(ray, out var t))
        {
            var point = ray.GetPoint(t);
            return cityMap.TryWorldToValidTile(point, out tile) && cityMap.TileAt(tile) == TileType.Road;
        }

        tile = default;
        return false;
    }

    private void attemptTruckMove(Vector3Int tile)
    {
        if (!pathfinder.TryFindPath(truck.LogicalTile, tile, out var path))
        {
            return;
        }

        truck.FollowPath(path);
    }
}
