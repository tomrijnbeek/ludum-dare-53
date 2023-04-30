using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class PoliceManager : MonoBehaviour
{
    [SerializeField] private Vector3Int[] spawnTiles;
    [SerializeField] private GameObject prefab;

    private readonly List<PoliceVehicle> vehicles = new();

    public void SpawnPoliceCar(Vector3Int currentPlayerPos)
    {
        var tile = determineSpawnTile(currentPlayerPos);
        var dir = determineSpawnDirection(tile);
        var obj = Instantiate(prefab);
        var pv = obj.GetComponent<PoliceVehicle>();
        vehicles.Add(pv);
        pv.Vehicle.Teleport(tile, dir);
        obj.name = $"Pineapple police {vehicles.Count}";
    }

    private Vector3Int determineSpawnTile(Vector3Int currentPlayerPos)
    {
        if (spawnTiles.Length == 0)
        {
            return Vector3Int.zero;
        }
        var furthestSpawn = spawnTiles.OrderByDescending(t => distance(t, currentPlayerPos)).First();
        return furthestSpawn;
    }

    private Direction determineSpawnDirection(Vector3Int tile)
    {
        var validDirections = DirectionHelpers.EnumerateDirections().Where(dir =>
        {
            var nb = tile.Neighbour(dir);
            return CityMap.Instance.IsValid(nb) && CityMap.Instance.TileAt(nb).IsRoad();
        }).ToArray();
        return validDirections.Length == 0
            ? Direction.PositiveX
            : validDirections[Random.Range(0, validDirections.Length)];
    }

    public void PrepareTurn()
    {
        foreach (var vehicle in vehicles)
        {
            prepareTurn(vehicle.Vehicle);
        }
    }

    private void prepareTurn(Vehicle vehicle)
    {
        var tile = vehicle.LogicalTile;
        var path = generateRandomPath(tile, vehicle.RangePerTurn, vehicle.Orientation);
        vehicle.PreparePath(path);
    }

    public IEnumerable<VehicleMovement> CommitMovement()
    {
        return vehicles.Select(v => v.Vehicle.CommitVehicleMovement()).ToList();
    }

    private Path generateRandomPath(Vector3Int startTile, int length, Direction orientation)
    {
        var dirs = new Direction[length];
        var currentTile = startTile;
        var currentDir = orientation;
        for (var i = 0; i < length; i++)
        {
            var dir = generateRandomDirection(currentTile, currentDir);
            dirs[i] = dir;
            currentDir = dir;
            currentTile = currentTile.Neighbour(dir);
        }

        return new Path(startTile, dirs);
    }

    private Direction generateRandomDirection(Vector3Int tile, Direction previousDir)
    {
        var validDirs = DirectionHelpers.EnumerateDirections()
            .Where(dir => CityMap.Instance.IsValid(tile.Neighbour(dir)) && CityMap.Instance.TileAt(tile.Neighbour(dir)).IsRoad())
            .ToList();
        if (validDirs.Count > 1)
        {
            validDirs.Remove(previousDir.Opposite());
        }

        if (validDirs.Count == 0)
        {
            return Direction.PositiveX;
        }

        return validDirs[Random.Range(0, validDirs.Count)];
    }

    private static int distance(Vector3Int x, Vector3Int y) => (x - y).sqrMagnitude;
}
