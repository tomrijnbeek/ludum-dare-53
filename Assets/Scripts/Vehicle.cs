using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class Vehicle : MonoBehaviour
{
    private const float lineHeight = 0.05f;

    [SerializeField] private Vector3Int logicalTile;
    [SerializeField] private Direction orientation = Direction.PositiveX;
    [SerializeField] private CityMap cityMap;
    [SerializeField] private VehicleLocations vehicleLocations;
    [SerializeField] private int rangePerTurn = 3;
    [SerializeField] private bool loseOnCollision = false;

    public Vector3Int LogicalTile => logicalTile;
    public Direction Orientation => orientation;
    public int RangePerTurn => rangePerTurn;
    public bool LoseOnCollision => loseOnCollision;
    public Path PreparedPath { get; private set; }

    private LineRenderer lineRenderer;

    private TileTransition? currentTransition;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        transform.position = cityMap.TileToCenterWorld(logicalTile);
        transform.forward = orientation.Forward();
        setLineRendererVertices();
        vehicleLocations.RegisterVehicle(logicalTile, this);
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
    }

    public void PreparePath(Path path)
    {
        PreparedPath = path;
        setLineRendererVertices();
    }

    private void setLineRendererVertices()
    {
        if (PreparedPath.Length == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        var vertices = new Vector3[PreparedPath.Length + 1];
        vertices[0] = cityMap.TileToCenterWorld(PreparedPath.Start).WithY(lineHeight);
        var curr = PreparedPath.Start;
        for (var i = 0; i < PreparedPath.Length; i++)
        {
            curr = curr.Neighbour(PreparedPath.Directions[i]);
            vertices[i + 1] = cityMap.TileToCenterWorld(curr).WithY(lineHeight);
        }

        lineRenderer.positionCount = vertices.Length;
        lineRenderer.SetPositions(vertices);
    }

    public VehicleMovement CommitVehicleMovement()
    {
        var vm = new VehicleMovement(this, PreparedPath);
        PreparedPath = Path.Empty;
        return vm;
    }

    public void MoveInDirection(Direction dir, float tickDuration)
    {
        orientation = dir;
        var fromTile = logicalTile;
        logicalTile = logicalTile.Neighbour(dir);
        var toTile = logicalTile;
        transform.forward = dir.Forward();
        currentTransition = new TileTransition(
            cityMap.TileToCenterWorld(fromTile),
            cityMap.TileToCenterWorld(toTile),
            Time.time,
            Time.time + tickDuration);
        vehicleLocations.QueueTransition(this, fromTile, toTile);
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
