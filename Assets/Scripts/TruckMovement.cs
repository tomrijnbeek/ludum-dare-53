using JetBrains.Annotations;
using UnityEngine;

public sealed class TruckMovement : MonoBehaviour
{
    [SerializeField] private Vehicle truck;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private new Camera camera;

    [CanBeNull] public event VoidEventHandler PathSelected;

    private Pathfinder pathfinder;

    [CanBeNull] private VehicleMovement currentMovement;

    private void Start()
    {
        pathfinder = new Pathfinder(cityMap);
    }

    private void Update()
    {
        if (currentMovement != null)
        {
            if (!currentMovement.Done)
            {
                return;
            }
            currentMovement = null;
        }

        if (tryFindTile(out var tile) && pathfinder.TryFindPath(truck.LogicalTile, tile, out var path))
        {
            truck.PreparePath(path.Clamped(truck.RangePerTurn));
        }

        if (Input.GetMouseButtonDown(0))
        {
            PathSelected?.Invoke();
        }
    }

    public VehicleMovement ExecuteTurn()
    {
        currentMovement = truck.TraversePreparedPath();
        return currentMovement;
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
}
