using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public sealed class TruckMovement : MonoBehaviour
{
    [SerializeField] private Vehicle truck;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject indicator;

    [CanBeNull] public event VoidEventHandler PathSelected;

    private Pathfinder pathfinder;
    private readonly List<GameObject> spawnedIndicators = new();

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

        if (spawnedIndicators.Count == 0)
        {
            spawnIndicators();
        }

        if (tryFindTile(out var tile)
            && pathfinder.TryFindPath(truck.LogicalTile, tile, out var path)
            && path.Length > 0)
        {
            truck.PreparePath(path.Clamped(truck.RangePerTurn));
        }

        if (Input.GetMouseButtonDown(0) && truck.PreparedPath.Length > 0)
        {
            PathSelected?.Invoke();
        }
    }

    public VehicleMovement CommitMovement()
    {
        currentMovement = truck.CommitVehicleMovement();
        destroyIndicators();
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

    private void spawnIndicators()
    {
        var front = new List<Vector3Int> { truck.LogicalTile };
        var seen = new HashSet<Vector3Int> { truck.LogicalTile };
        for (var i = 0; i <= truck.RangePerTurn; i++)
        {
            var newFront = new List<Vector3Int>();
            foreach (var tile in front)
            {
                if (tile != truck.LogicalTile)
                {
                    spawnIndicator(tile);
                }

                foreach (var nb in DirectionHelpers.EnumerateDirections().Select(d => tile.Neighbour(d)))
                {
                    if (cityMap.IsValid(nb) && cityMap.TileAt(nb).IsRoad() && !seen.Contains(nb))
                    {
                        newFront.Add(nb);
                        seen.Add(nb);
                    }
                }
            }
            front = newFront;
        }
    }

    private void spawnIndicator(Vector3Int tile)
    {
        var ind = Instantiate(indicator, transform, true);
        ind.transform.position = cityMap.TileToCenterWorld(tile);
        spawnedIndicators.Add(ind);
    }

    private void destroyIndicators()
    {
        foreach (var go in spawnedIndicators)
        {
            Destroy(go);
        }
        spawnedIndicators.Clear();
    }
}
