using System.Collections.Generic;
using UnityEngine;

public sealed class Truck : MonoBehaviour
{
    [SerializeField] private Vector3Int logicalTile;
    [SerializeField] private Grid grid;
    [SerializeField] private float tilesPerSecond = 2;

    private readonly Queue<Direction> path = new();
    private float nextMove;

    // Start is called before the first frame update
    void Start()
    {
        syncPositionWithLogical();
        // SetDestination(new Vector3Int(2, 2));
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count > 0 && nextMove <= Time.time)
        {
            var dir = path.Dequeue();
            logicalTile = logicalTile.Neighbour(dir);
            syncPositionWithLogical();
            transform.forward = dir.Forward();
            nextMove = Time.time + 1 / tilesPerSecond;
        }
    }

    private void syncPositionWithLogical()
    {
        transform.position = grid.GetCellCenterWorld(logicalTile);
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
}
