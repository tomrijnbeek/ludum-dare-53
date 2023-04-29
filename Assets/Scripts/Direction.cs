using System;
using UnityEngine;

public enum Direction
{
    PositiveX,
    PositiveY,
    NegativeX,
    NegativeY,
}

public static class Directions
{
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
