using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class Extensions
    {
        internal static Vector3 X(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        internal static Vector3 dX(this Vector3 v, float dx)
        {
            return new Vector3(v.x + dx, v.y, v.z);
        }

        internal static Vector3 Y(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        internal static Vector3 dY(this Vector3 v, float dy)
        {
            return new Vector3(v.x, v.y + dy, v.z);
        }

        internal static Vector3 Z(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        internal static Vector3 dZ(this Vector3 v, float dz)
        {
            return new Vector3(v.x, v.y, v.z + dz);
        }
    }
}
