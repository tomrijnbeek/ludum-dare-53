using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct Path
{
    public static Path Empty { get; } = new(Vector3Int.zero, Array.Empty<Direction>());

    public Vector3Int Start { get; }
    public IReadOnlyList<Direction> Directions { get; }

    public int Length => Directions?.Count ?? 0;

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
