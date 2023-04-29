using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 WithY(this Vector3 xyz, float y) => new(xyz.x, y, xyz.z);
}
