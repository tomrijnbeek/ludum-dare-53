using System.Collections.Generic;
using UnityEngine;

public sealed class Vehicle : MonoBehaviour
{
    [SerializeField] private Vector3Int logicalTile;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private float tilesPerSecond = 2;

    public Vector3Int LogicalTile => logicalTile;

    private readonly Queue<Direction> path = new();
    private TileTransition? currentTransition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = cityMap.TileToCenterWorld(logicalTile);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTransition is { } transition)
        {
            if (Time.time >= transition.EndTime)
            {
                transform.position = transition.To;
                currentTransition = null;
            }
            else
            {
                var t = (Time.time - transition.StartTime) / transition.Duration;
                transform.position = transition.From + t * transition.Difference;
            }
        }

        if (path.Count > 0 && currentTransition is null)
        {
            var dir = path.Dequeue();
            var fromTile = logicalTile;
            logicalTile = logicalTile.Neighbour(dir);
            var toTile = logicalTile;
            transform.forward = dir.Forward();
            currentTransition = new TileTransition(
                cityMap.TileToCenterWorld(fromTile),
                cityMap.TileToCenterWorld(toTile),
                Time.time,
                Time.time + 1 / tilesPerSecond);
        }
    }

    public void FollowPath(Path newPath)
    {
        if (path.Count > 0)
        {
            path.Clear();
        }

        foreach (var dir in newPath.Directions)
        {
            path.Enqueue(dir);
        }
    }

    private readonly struct TileTransition
    {
        public Vector3 From { get; }
        public Vector3 To { get; }
        public float StartTime { get; }
        public float EndTime { get; }

        public Vector3 Difference => To - From;
        public float Duration => EndTime - StartTime;

        public TileTransition(Vector3 from, Vector3 to, float startTime, float endTime)
        {
            From = from;
            To = to;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
