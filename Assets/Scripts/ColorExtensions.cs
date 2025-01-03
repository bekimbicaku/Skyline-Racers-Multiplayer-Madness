using UnityEngine;

public static class ColorExtensions
{
    public static string ToHex(this Color color)
    {
        Color32 color32 = color;
        return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
    }
}
