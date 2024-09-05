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
    public float Speed = 300.0f;
    public float JumpForce = -3.5f;

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

        //Movement
        if (ImGui.IsKeyDown(ImGuiKey.A))
        {
            Velocity.X = -Speed * ImGui.GetIO().DeltaTime * Config.TimeScale;
        }

        if (ImGui.IsKeyDown(ImGuiKey.D))
        {
            Velocity.X = Speed * ImGui.GetIO().DeltaTime * Config.TimeScale;
        }

        //Lerping Speed to 0
        float t = 0.85f;
        Velocity.X = Velocity.X * t + 0.0f * (1 - t);

        //Jumping
        if (ImGui.IsKeyPressed(ImGuiKey.Space) && Velocity.Y == 0)
        {
            Velocity.Y = JumpForce;
        }

        //Updating Positions based on Velocity
        position.Y += Velocity.Y;
        position.X += Velocity.X;
    }
}
