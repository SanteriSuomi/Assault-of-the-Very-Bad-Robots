using UnityEngine;

namespace AOTVBR
{
    public static class Utility
    {
        public static bool PosEqual(Vector3 a, Vector3 b)
        {
            return FloatEqual(a.x, b.x)
                   && FloatEqual(a.y, b.y)
                   && FloatEqual(a.z, b.z);
        }

        public static bool FloatEqual(float x, float y) 
            => FastAbs(x - y) < 0.01f;

        public static bool SafeFloatEqual(float x, float y) 
            => FastAbs(x - y) <= (FastAbs(x) + FastAbs(y) + 1) * float.Epsilon;

        public static float FastAbs(float x) 
            => x > 0 ? x : -x;
    }
}