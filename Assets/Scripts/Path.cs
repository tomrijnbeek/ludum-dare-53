using System.Collections.Generic;
using System.Linq;
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

    public Path Clamped(int maxLength)
    {
        return Length <= maxLength ? this : new Path(Start, Directions.Take(maxLength).ToList().AsReadOnly());
    }
}
