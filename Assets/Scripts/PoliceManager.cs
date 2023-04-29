using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PoliceManager : MonoBehaviour
{
    [SerializeField] private CityMap cityMap;
    [SerializeField] private int movementPerTurn = 3;
    [SerializeField] private float timeBetweenTurns = 10;

    private readonly List<PoliceVehicle> vehicles = new();
    private float nextTurn;

    // Start is called before the first frame update
    void Start()
    {
        nextTurn = Time.time + timeBetweenTurns;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTurn)
        {
            DoTurn();
            nextTurn = Time.time + timeBetweenTurns;
        }
    }

    public void RegisterVehicle(PoliceVehicle vehicle)
    {
        vehicles.Add(vehicle);
    }

    public void DoTurn()
    {
        foreach (var vehicle in vehicles)
        {
            var tile = vehicle.Vehicle.LogicalTile;
            var path = generateRandomPath(tile);
            vehicle.Vehicle.FollowPath(path);
        }
    }

    private Path generateRandomPath(Vector3Int startTile)
    {
        var dirs = new Direction[movementPerTurn];
        var currentTile = startTile;
        Direction? previousDirection = null;
        for (var i = 0; i < movementPerTurn; i++)
        {
            var dir = generateRandomDirection(currentTile, previousDirection);
            dirs[i] = dir;
            previousDirection = dir;
            currentTile = currentTile.Neighbour(dir);
        }

        return new Path(startTile, dirs);
    }

    private Direction generateRandomDirection(Vector3Int tile, Direction? previousDir)
    {
        var validDirs = DirectionHelpers.EnumerateDirections()
            .Where(dir => cityMap.IsValid(tile.Neighbour(dir)) && cityMap.TileAt(tile.Neighbour(dir)).IsRoad())
            .ToList();
        if (validDirs.Count > 1 && previousDir is { } actualDir)
        {
            validDirs.Remove(actualDir.Opposite());
        }

        if (validDirs.Count == 0)
        {
            return Direction.PositiveX;
        }

        return validDirs[Random.Range(0, validDirs.Count)];
    }
}
