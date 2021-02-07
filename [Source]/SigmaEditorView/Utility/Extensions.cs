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

        internal static bool TryParse(this string s, out Vector2 v2)
        {
            v2 = Vector2.one;

            string[] data = s?.Split(',');

            if (data?.Length == 2)
            {
                if (float.TryParse(data[0], out float x) && float.TryParse(data[1], out float y))
                {
                    v2.x = x;
                    v2.y = y;
                    return true;
                }
            }

            return false;
        }
    }
}
