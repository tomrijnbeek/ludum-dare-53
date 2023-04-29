using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class VehicleLocations : MonoBehaviour
{
    private IList<Vehicle> emptyList = new List<Vehicle>(0).AsReadOnly();

    private readonly Dictionary<Vector3Int, List<Vehicle>> vehicles = new();
    private readonly List<(Vehicle Vehicle, Vector3Int From, Vector3Int To)> transitionQueue = new();

    public void RegisterVehicle(Vector3Int tile, Vehicle vehicle)
    {
        addVehicle(tile, vehicle);
    }

    public void QueueTransition(Vehicle vehicle, Vector3Int from, Vector3Int to)
    {
        transitionQueue.Add((vehicle, from, to));
    }

    public void CommitTransitions()
    {
        checkTransitionQueueForHeadOnCollisions();
        var candidateTiles = new List<Vector3Int>();

        foreach (var transition in transitionQueue)
        {
            removeVehicle(transition.From, transition.Vehicle);
            addVehicle(transition.To, transition.Vehicle);
            candidateTiles.Add(transition.From);
            candidateTiles.Add(transition.To);
        }

        checkTilesForCollisions(candidateTiles);

        transitionQueue.Clear();
    }

    private void addVehicle(Vector3Int tile, Vehicle vehicle)
    {
        if (!vehicles.TryGetValue(tile, out var list))
        {
            list = new List<Vehicle>();
            vehicles.Add(tile, list);
        }

        list.Add(vehicle);
    }

    private void removeVehicle(Vector3Int tile, Vehicle vehicle)
    {
        vehicles[tile].Remove(vehicle);
    }

    private void checkTransitionQueueForHeadOnCollisions()
    {
        var transitionsByOrigin = transitionQueue.ToLookup(transition => transition.From);
        foreach (var (_, from, to) in transitionQueue)
        {
            var departingFromTo = transitionsByOrigin[to];
            if (departingFromTo.Any(otherTransition => otherTransition.To == from))
            {
                lose();
                return;
            }
        }
    }

    private void checkTilesForCollisions(IEnumerable<Vector3Int> tiles)
    {
        foreach (var tile in tiles)
        {
            var vs = vehiclesOnTile(tile);
            if (vs.Count > 1 && vs.Any(v => v.LoseOnCollision))
            {
                lose();
                return;
            }
        }
    }

    private void lose()
    {
        Debug.LogWarning("You lose!");
    }

    private IList<Vehicle> vehiclesOnTile(Vector3Int tile)
    {
        emptyList = new List<Vehicle>();
        return vehicles.TryGetValue(tile, out var list) ? list : emptyList;
    }
}
