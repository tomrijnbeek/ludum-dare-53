using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Pathfinder
{
    private readonly CityMap map;

    public Pathfinder(CityMap map)
    {
        this.map = map;
    }

    public bool TryFindPath(Vector3Int from, Vector3Int to, out Path path)
    {
        if (!map.TileAt(from).IsRoad() || !map.TileAt(to).IsRoad())
        {
            path = default;
            return false;
        }

        var q = new Queue<Vector3Int>();
        var seen = new Dictionary<Vector3Int, Direction>();
        queueUnseenNeighbours(from);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            if (current == to)
            {
                var dirs = reconstructPath();
                path = new Path(from, dirs.ToList());
                return true;
            }
            queueUnseenNeighbours(current);
        }

        path = default;
        return false;

        void queueUnseenNeighbours(Vector3Int tile)
        {
            foreach (var dir in DirectionHelpers.EnumerateDirections())
            {
                var neighbour = tile.Neighbour(dir);
                if (neighbour == from || !map.IsValid(neighbour) || seen.ContainsKey(neighbour) || !map.TileAt(neighbour).IsRoad())
                {
                    continue;
                }
                seen.Add(neighbour, dir);
                q.Enqueue(neighbour);
            }
        }

        IEnumerable<Direction> reconstructPath()
        {
            var list = new List<Direction>();
            var current = to;
            while (current != from)
            {
                list.Add(seen[current]);
                current = current.Neighbour(seen[current].Opposite());
            }

            list.Reverse();
            return list;
        }
    }
}
