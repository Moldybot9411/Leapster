using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leapster
{
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

        public static async void Shake(float Amount, float Duration)
        {
            currentAmount = Amount;
            duration = Duration;
        }

        private static void Update()
        {
            currentAmount = Lerp(currentAmount, 0, 0.1f);

            ShakeOffset = new Vector2((float)rand.NextDouble() * currentAmount, (float)rand.NextDouble() * currentAmount);
        }

        static float Lerp(float a, float b, float f)
        {
            return a * (1.0f - f) + (b * f);
        }

    }
}
