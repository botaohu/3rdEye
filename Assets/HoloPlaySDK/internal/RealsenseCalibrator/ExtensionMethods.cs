using UnityEngine;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Transform FindInactive(this Transform parent, string name)
    {
        foreach (Transform t in parent)
            if (t.name == name) return t;
        return null;
    }
}