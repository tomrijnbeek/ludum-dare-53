using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PoliceManager : MonoBehaviour
{
    [SerializeField] private CityMap cityMap;

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
        var path = generateRandomPath(tile, vehicle.RangePerTurn, vehicle.Orientation);
        vehicle.PreparePath(path);
    }

    public VehicleMovement ExecuteTurn()
    {
        turnPrepared = false;
        return VehicleMovement.Composite(vehicles.Select(v => v.Vehicle.TraversePreparedPath()).ToList());
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
            .Where(dir => cityMap.IsValid(tile.Neighbour(dir)) && cityMap.TileAt(tile.Neighbour(dir)).IsRoad())
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
}
