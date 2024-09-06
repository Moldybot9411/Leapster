using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : Screen
{
    public event Action OnRender = delegate { };

    public override void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    public override void Hide()
    {
    }

    public override void RenderImGui()
    {
        OnRender();
    }
}
