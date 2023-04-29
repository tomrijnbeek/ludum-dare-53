using System.Collections.Generic;
using UnityEngine;

public sealed class Truck : MonoBehaviour
{
    [SerializeField] private Vector3Int logicalTile;
    [SerializeField] private Grid grid;
    [SerializeField] private float tilesPerSecond = 2;

    private readonly Queue<Direction> path = new();
    private TileTransition? currentTransition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = grid.GetCellCenterWorld(logicalTile);
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
                grid.GetCellCenterWorld(fromTile),
                grid.GetCellCenterWorld(toTile),
                Time.time,
                Time.time + 1 / tilesPerSecond);
        }
    }

    public void SetDestination(Vector3Int destination)
    {
        if (path.Count > 0)
        {
            path.Clear();
        }

        foreach (var dir in makePath(destination))
        {
            path.Enqueue(dir);
        }
    }

    private IEnumerable<Direction> makePath(Vector3Int destination)
    {
        var (xDir, yDir) = Directions.FromDifference(destination - logicalTile);
        var x = logicalTile.x;
        var y = logicalTile.y;

        while (x != destination.x)
        {
            x += xDir.Numeric();
            yield return xDir;
        }
        while (y != destination.y)
        {
            y += yDir.Numeric();
            yield return yDir;
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
