using ImGuiNET;
using System.Numerics;

namespace Leapster;

internal static class Screenshake
{
    public static Vector2 ShakeOffset;

    private static float currentAmount;
    private static float duration;

    private static Random rand = new();

    static Screenshake()
    {
        Game.Instance.GameScreen.OnRender += Update;
    }

    public static void Shake(float Amount, float Duration)
    {
        currentAmount = Amount;
        duration = Duration;
    }

    private static void Update()
    {
        currentAmount = float.Lerp(currentAmount, 0, duration * ImGui.GetIO().DeltaTime);

        ShakeOffset = new Vector2((float)rand.NextDouble() * currentAmount, (float)rand.NextDouble() * currentAmount);
    }
}
