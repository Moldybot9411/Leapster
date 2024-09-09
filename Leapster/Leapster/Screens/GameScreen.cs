using ImGuiNET;
using Leapster.Components;
using Leapster.ObjectSystem;
using Leapster.ParticleSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : IScreen
{
    public event Action OnRender = delegate { };

    public bool FPSOverlay = false;

    public List<GameObject> gameObjects = [];

    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);

        GameObject player = new(new RectangleF(200, 50, 20, 40), "Player");
        //player.AddComponent(new RigidBody());
        player.AddComponent(new CharacterController());
        player.AddComponent(new H());
        player.AddComponent(new Particly(new(50, 50)));

        gameObjects.Add(player);
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
