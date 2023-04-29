using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class Vehicle : MonoBehaviour
{
    private const float lineHeight = 0.05f;

    [SerializeField] private Vector3Int logicalTile;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private float tilesPerSecond = 6;
    [SerializeField] private int rangePerTurn = 3;

    public Vector3Int LogicalTile => logicalTile;
    public Direction Orientation { get; private set; } = Direction.PositiveX;
    public int RangePerTurn => rangePerTurn;

    private LineRenderer lineRenderer;

    private readonly Queue<Direction> pathQueue = new();
    private TileTransition? currentTransition;
    private bool isMoving;

    private void Start()
    {
        transform.position = cityMap.TileToCenterWorld(logicalTile);
        lineRenderer = GetComponent<LineRenderer>();
        if (pathQueue.Count == 0)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void Update()
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

        if (isMoving && pathQueue.Count > 0 && currentTransition is null)
        {
            var dir = pathQueue.Dequeue();
            Orientation = dir;
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

        if (pathQueue.Count == 0 && isMoving)
        {
            isMoving = false;
            lineRenderer.positionCount = 0;
        }
    }

    public void PreparePath(Path path)
    {
        if (pathQueue.Count > 0)
        {
            pathQueue.Clear();
        }

        foreach (var dir in path.Directions)
        {
            pathQueue.Enqueue(dir);
        }
        setLineRendererVertices(path);
    }

    private void setLineRendererVertices(Path path)
    {
        var vertices = new Vector3[path.Length + 1];
        vertices[0] = cityMap.TileToCenterWorld(path.Start).WithY(lineHeight);
        var curr = path.Start;
        for (var i = 0; i < path.Length; i++)
        {
            curr = curr.Neighbour(path.Directions[i]);
            vertices[i + 1] = cityMap.TileToCenterWorld(curr).WithY(lineHeight);
        }

        lineRenderer.positionCount = vertices.Length;
        lineRenderer.SetPositions(vertices);
    }

    public VehicleMovement TraversePreparedPath()
    {
        isMoving = true;
        return new VehicleMovement(() => isMoving == false);
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
