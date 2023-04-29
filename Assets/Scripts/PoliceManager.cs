using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PoliceManager : MonoBehaviour
{
    [SerializeField] private CityMap cityMap;
    [SerializeField] private int movementPerTurn = 3;

    private bool turnPrepared;
    private readonly List<PoliceVehicle> vehicles = new();

    public void RegisterVehicle(PoliceVehicle vehicle)
    {
        vehicles.Add(vehicle);
        if (turnPrepared)
        {
            prepareTurn(vehicle.Vehicle);
        }
    }

    public void PrepareTurn()
    {
        foreach (var vehicle in vehicles)
        {
            prepareTurn(vehicle.Vehicle);
        }
        turnPrepared = true;
    }

    private void prepareTurn(Vehicle vehicle)
    {
        var tile = vehicle.LogicalTile;
        var path = generateRandomPath(tile);
        vehicle.PreparePath(path);
    }

    public VehicleMovement ExecuteTurn()
    {
        turnPrepared = false;
        return VehicleMovement.Composite(vehicles.Select(v => v.Vehicle.TraversePreparedPath()).ToList());
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
