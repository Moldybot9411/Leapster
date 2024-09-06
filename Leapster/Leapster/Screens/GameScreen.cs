using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : Screen
{
    public event Action OnRender = delegate { };

    public bool FPSOverlay = false;

    public override void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    public override void Hide()
    {
    }

    public override void RenderImGui()
    {
        if (FPSOverlay)
        {
            ImGui.GetBackgroundDrawList().AddText(new Vector2(10, 10), Color.Lime.ToImGuiColor(), $"FPS: {ImGui.GetIO().Framerate:F2}");
        }

        OnRender();
    }
}
