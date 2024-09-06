using ImGuiNET;
using System.Numerics;

namespace Leapster;

public class Player
{
    public Vector2 Velocity;
    public float Speed = 300.0f;
    public float JumpForce = -3.5f;
    public Vector2 position = new(50.0f, 50.0f);

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
        Velocity.Y += Config.Gravity * Config.TimeScale * ImGui.GetIO().DeltaTime;

        //Movement
        if (ImGui.IsKeyDown(ImGuiKey.A) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickLeft))
        {
            Velocity.X = -Speed * ImGui.GetIO().DeltaTime * Config.TimeScale;
        }

        if (ImGui.IsKeyDown(ImGuiKey.D) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickRight))
        {
            Velocity.X = Speed * ImGui.GetIO().DeltaTime * Config.TimeScale;
        }

        //Lerping Speed to 0
        float t = 0.85f;
        Velocity.X = Velocity.X * t + 0.0f * (1 - t);

        CheckCollisions();

        //Jumping
        if ((ImGui.IsKeyPressed(ImGuiKey.Space) || ImGui.IsKeyPressed(ImGuiKey.GamepadFaceDown)) && Velocity.Y == 0)
        {
            Velocity.Y = JumpForce;
        }


        //Updating Positions based on Velocity
        position.Y += Velocity.Y;
        position.X += Velocity.X;
    }

    private void CheckCollisions()
    {
        Vector2 bottomRight = position + size;

        foreach (Vector4 Box in Game.Instance.CurrentLevel.Boxes)
        {
            if(bottomRight.Y > Box.Y && bottomRight.Y < Box.Y + Box.W &&bottomRight.X > Box.X && bottomRight.X < Box.X + Box.Z)
            {
                position.Y = Box.Y - size.Y;
                Velocity.Y = 0;
            }
        }
    }
}
