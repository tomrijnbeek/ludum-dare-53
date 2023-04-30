using System;
using UnityEngine;

public sealed class CityMap : Singleton<CityMap>
{
    [SerializeField] private Grid grid;

    private readonly TileType[,] map = {
        { TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Empty, TileType.Empty },
        { TileType.Road, TileType.Empty, TileType.Building, TileType.Road, TileType.Building, TileType.Empty, TileType.Road, TileType.Road, TileType.Road, TileType.Building },
        { TileType.Road, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Empty, TileType.Road, TileType.Road },
        { TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Empty, TileType.Building, TileType.Road, TileType.Building, TileType.Building, TileType.Road },
        { TileType.Road, TileType.Empty, TileType.Road, TileType.Road, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road },
        { TileType.Road, TileType.Road, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Empty, TileType.Road, TileType.Empty },
        { TileType.Building, TileType.Road, TileType.Empty, TileType.Road, TileType.Building, TileType.Road, TileType.Empty, TileType.Building, TileType.Road, TileType.Building },
        { TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road },
        { TileType.Road, TileType.Building, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Building, TileType.Building, TileType.Road },
        { TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Building, TileType.Road, TileType.Road, TileType.Road, TileType.Road, TileType.Road },
    };

    public int Width => map.GetLength(0);
    public int Height => map.GetLength(1);

    public bool IsValid(Vector3Int tile) => tile.x >= 0 && tile.x < Width && tile.y >= 0 && tile.y < Height;

    public TileType TileAt(Vector3Int tile)
    {
        if (!IsValid(tile))
        {
            throw new ArgumentOutOfRangeException(nameof(tile));
        }

        return map[tile.x, tile.y];
    }

    public bool TryWorldToValidTile(Vector3 pos, out Vector3Int tile)
    {
        tile = WorldToTile(pos);
        return IsValid(tile);
    }

    public Vector3 TileToCenterWorld(Vector3Int tile) => grid.GetCellCenterWorld(tile);
    public Vector3Int WorldToTile(Vector3 pos) => grid.WorldToCell(pos);
}

public enum TileType
{
    Empty,
    Road,
    Building,
}

public static class TileTypeExtensions
{
    public static bool IsRoad(this TileType tileType) => tileType == TileType.Road;
    public static bool IsBuilding(this TileType tileType) => tileType == TileType.Building;
}
