using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : IScreen
{
    public event Action OnRender = delegate { };

    public bool FPSOverlay = false;

    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    public void Hide()
    {
    }

    public void RenderImGui()
    {
        if (FPSOverlay)
        {
            ImGui.GetBackgroundDrawList().AddText(new Vector2(10, 10), Color.Lime.ToImGuiColor(), $"FPS: {ImGui.GetIO().Framerate:F2}");
        }

        OnRender();
    }
}
