using System.Collections.Generic;
using UnityEngine;

public readonly struct Path
{
    public Vector3Int Start { get; }
    public IReadOnlyList<Direction> Directions { get; }

    public int Length => Directions.Count;

    public Path(Vector3Int start, IReadOnlyList<Direction> directions)
    {
        Start = start;
        Directions = directions;
    }
}
