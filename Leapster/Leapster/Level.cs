using ImGuiNET;
using Silk.NET.Maths;
using System.Drawing;
using System.Numerics;

namespace Leapster;

public class Level
{
    public Vector2 PlayerSpawn { get; private set; }

    public List<RectangleF> Boxes { get; private set; } = new();

    public Vector4 BoxColor = new(0, 0, 0, 1);

    public Level()
    {
    }

    public Level(Vector2 playerSpawn, params RectangleF[] boxes)
    {
        PlayerSpawn = playerSpawn;
        Boxes.AddRange(boxes);
    }

    public void OnLoad()
    {
        Game.Instance.GameScreen.OnRender += OnRender;
    }

    public void OnUnload()
    {
        Game.Instance.GameScreen.OnRender -= OnRender;
    }

    internal void OnRender()
    {
        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        foreach (RectangleF rect in Boxes)
        {
            Vector2 topLeft = new Vector2(rect.X, rect.Y);
            Vector2 topRight = topLeft + new Vector2(rect.Width, rect.Height);

            draw.AddRectFilled(topLeft + Screenshake.ShakeOffset, topRight + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(BoxColor));
        }

    }

}
