using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class RoadSpawner : MonoBehaviour
{
    [SerializeField] private CityMap cityMap;

    [Header("Prefabs")]
    [SerializeField] private GameObject emptyTile;

    [SerializeField] private GameObject straight;
    [SerializeField] private GameObject curve;
    [SerializeField] private GameObject tIntersection;
    [SerializeField] private GameObject xIntersection;

    [SerializeField] private GameObject[] buildings;

    private static readonly Directions[] deadEndDirections =
    {
        Directions.PositiveX,
        Directions.PositiveY,
        Directions.NegativeX,
        Directions.NegativeY,
    };

    private static readonly Directions[] straightDirections =
    {
        Directions.PositiveX | Directions.NegativeX,
        Directions.PositiveY | Directions.NegativeY,
    };

    private static readonly Directions[] curveDirections =
    {
        Directions.PositiveX | Directions.PositiveY,
        Directions.NegativeY | Directions.PositiveX,
        Directions.NegativeX | Directions.NegativeY,
        Directions.PositiveY | Directions.NegativeX,
    };

    private static readonly Directions[] tIntersectionDirections =
    {
        Directions.PositiveX | Directions.PositiveY | Directions.NegativeX,
        Directions.NegativeY | Directions.PositiveX | Directions.PositiveY,
        Directions.NegativeX | Directions.NegativeY | Directions.PositiveX,
        Directions.PositiveY | Directions.NegativeX | Directions.NegativeY,
    };

    private static readonly Directions[] xIntersectionDirections =
    {
        Directions.PositiveX | Directions.PositiveY | Directions.NegativeX | Directions.NegativeY,
        Directions.None,
    };

    private readonly Dictionary<Directions, (GameObject Object, float Rotation)> objectMap = new();

    private void Start()
    {
        cacheObjectMap(deadEndDirections, straight);
        cacheObjectMap(straightDirections, straight);
        cacheObjectMap(curveDirections, curve);
        cacheObjectMap(tIntersectionDirections, tIntersection);
        cacheObjectMap(xIntersectionDirections, xIntersection);

        populateTile();
    }

    private void cacheObjectMap(Directions[] arr, GameObject obj)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            objectMap.Add(arr[i], (obj, i * 90));
        }
    }

    private void populateTile()
    {
        for (var y = 0; y < cityMap.Height; y++)
        {
            for (var x = 0; x < cityMap.Width; x++)
            {
                var tile = new Vector3Int(x, y);
                spawnFloor(tile);
                if (isRoad(tile))
                {
                    spawnRoad(tile);
                }
                if (isBuilding(tile))
                {
                    spawnBuilding(tile);
                }
            }
        }
    }

    private void spawnFloor(Vector3Int tile)
    {
        var floor = Instantiate(emptyTile, transform, true);
        floor.transform.position = cityMap.TileToCenterWorld(tile);
    }

    private void spawnRoad(Vector3Int tile)
    {
        var roadNeighbours = Directions.None;
        foreach (var dir in DirectionHelpers.EnumerateDirections())
        {
            if (isRoad(tile.Neighbour(dir)))
            {
                roadNeighbours |= dir.ToFlags();
            }
        }

        var (obj, orientation) = objectMap[roadNeighbours];
        var road = Instantiate(obj, transform, true);
        road.transform.position = cityMap.TileToCenterWorld(tile);
        road.transform.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
    }

    private void spawnBuilding(Vector3Int tile)
    {
        if (buildings.Length == 0)
        {
            Debug.LogWarning("Attempting to spawn building, but no prefabs found.");
            return;
        }

        var building = buildings[Random.Range(0, buildings.Length)];
        var surroundingRoads = DirectionHelpers.EnumerateDirections()
            .Where(dir => isRoad(tile.Neighbour(dir))).ToArray();
        var forward = surroundingRoads.Length == 0
            ? Vector3.forward
            : surroundingRoads[Random.Range(0, surroundingRoads.Length)].Forward();

        var buildingObj = Instantiate(building, transform, true);
        buildingObj.transform.position = cityMap.TileToCenterWorld(tile);
        building.transform.forward = forward;
    }

    private bool isRoad(Vector3Int tile) => cityMap.IsValid(tile) && cityMap.TileAt(tile).IsRoad();
    private bool isBuilding(Vector3Int tile) => cityMap.IsValid(tile) && cityMap.TileAt(tile).IsBuilding();
}
