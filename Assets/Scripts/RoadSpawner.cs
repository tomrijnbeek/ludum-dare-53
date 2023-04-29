using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private CityMap cityMap;

    [Header("Prefabs")]
    [SerializeField] private GameObject emptyTile;

    [SerializeField] private GameObject straight;
    [SerializeField] private GameObject curve;
    [SerializeField] private GameObject tIntersection;
    [SerializeField] private GameObject xIntersection;

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

        spawnRoads();
    }

    private void cacheObjectMap(Directions[] arr, GameObject obj)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            objectMap.Add(arr[i], (obj, i * 90));
        }
    }

    private void spawnRoads()
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

    private bool isRoad(Vector3Int tile) => cityMap.IsValid(tile) && cityMap.TileAt(tile) == TileType.Road;
}
