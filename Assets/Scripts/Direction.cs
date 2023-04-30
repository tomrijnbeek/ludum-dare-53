using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction : byte
{
    PositiveX = 1,
    PositiveY = 2,
    NegativeX = 3,
    NegativeY = 4,
}

[Flags]
public enum Directions : byte
{
    None = 0,
    PositiveX = 1,
    PositiveY = 2,
    NegativeX = 4,
    NegativeY = 8,
}

public static class DirectionHelpers
{
    public static Directions ToFlags(this Direction direction) => (Directions) (1 << ((int) direction - 1));

    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.PositiveX => Direction.NegativeX,
        Direction.PositiveY => Direction.NegativeY,
        Direction.NegativeX => Direction.PositiveX,
        Direction.NegativeY => Direction.PositiveY,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static IEnumerable<Direction> EnumerateDirections()
    {
        return new[] { Direction.PositiveX, Direction.PositiveY, Direction.NegativeX, Direction.NegativeY };
    }

    public static IEnumerable<Vector3Int> EnumerateNeighbours(Vector3Int tile)
    {
        return EnumerateDirections().Select(dir => tile.Neighbour(dir));
    }

    public static (Direction X, Direction Y) FromDifference(Vector3Int difference)
    {
        var x = difference.x >= 0 ? Direction.PositiveX : Direction.NegativeX;
        var y = difference.y >= 0 ? Direction.PositiveY : Direction.NegativeY;
        return (x, y);
    }

    public static int Numeric(this Direction direction) => direction switch
    {
        Direction.PositiveX or Direction.PositiveY => 1,
        Direction.NegativeX or Direction.NegativeY => -1,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static Vector3Int Diff(this Direction dir) => dir switch
    {
        Direction.PositiveX => new Vector3Int(1, 0),
        Direction.PositiveY => new Vector3Int(0, 1),
        Direction.NegativeX => new Vector3Int(-1, 0),
        Direction.NegativeY => new Vector3Int(0, -1),
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };

    public static Vector3Int Neighbour(this Vector3Int xyz, Direction dir) => xyz + dir.Diff();

    public static Vector3 Forward(this Direction dir) => dir switch
    {
        Direction.PositiveX => Vector3.forward,
        Direction.PositiveY => Vector3.left,
        Direction.NegativeX => Vector3.back,
        Direction.NegativeY => Vector3.right,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };
}
