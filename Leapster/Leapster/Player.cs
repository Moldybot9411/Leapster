using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leapster;

public class Player
{
    public Vector2 Velocity;
    public float Speed = 350.0f;

    private Vector2 position = new(50.0f, 50.0f);
    private Vector2 size = new(20.0f, 40.0f);

    public Player()
    {
        Game.Instance.OnRender += OnRenderFrame;
    }

    private void OnRenderFrame()
    {
        UpdatePosition();

        Vector2 topLeft = position;
        Vector2 bottomRight = topLeft + size;
        ImGui.GetBackgroundDrawList().AddRectFilled(topLeft, bottomRight, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1)));
    }

    private void UpdatePosition()
    {
        //Gravity
        if (position.Y <= 500)
        {
            Velocity.Y += Config.Gravity * Config.TimeScale * ImGui.GetIO().DeltaTime;
        }
        else
        {
            Velocity.Y = 0.0f;
        }

        if (ImGui.IsKeyPressed(ImGuiKey.A))
        {
            Velocity.X = -Speed * ImGui.GetIO().DeltaTime * Config.TimeScale;
        }

        if (ImGui.IsKeyPressed(ImGuiKey.D))
        {
            Velocity.X = Speed * ImGui.GetIO().DeltaTime;
        }

        position.Y += Velocity.Y;
        position.X += Velocity.X;
    }
}
