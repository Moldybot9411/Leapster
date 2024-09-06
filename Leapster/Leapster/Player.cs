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
        Game.Instance.GameScreen.OnRender += OnRenderFrame;
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
        CheckCollisions();
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


        //Jumping
        if ((ImGui.IsKeyPressed(ImGuiKey.Space) || ImGui.IsKeyPressed(ImGuiKey.GamepadFaceDown)) && Velocity.Y <= 0.15f && Velocity.Y >= 0)
        {
            Velocity.Y = JumpForce;
        }


        //Updating Positions based on Velocity
        position.Y += Velocity.Y;
        position.X += Velocity.X;
    }

    private void CheckCollisions()
    {
        /*Vector2 bottomRight = position + size;
        Vector2 topRight = new Vector2(position.X + size.X, position.Y);*/

        Vector4 rightEdge = new(position.X + size.X, position.Y, position.X + size.X, position.Y + size.Y);
        Vector4 leftEdge = new(position.X, position.Y, position.X, position.Y + size.Y);
        Vector4 topEdge = new(position.X, position.Y, position.X + size.X, position.Y);
        Vector4 bottomEdge = new(position.X, position.Y + size.Y, position.X + size.X, position.Y + size.Y);

        foreach (Vector4 Box in Game.Instance.CurrentLevel.Boxes)
        {
            if(rightEdge.X + Velocity.X > Box.X && leftEdge.X + Velocity.X <  Box.X + Box.Z && bottomEdge.Y + Velocity.Y > Box.Y && topEdge.Y + Velocity.Y < Box.Y + Box.W)
            {
                Vector2 playerMiddle = new(position.X + Velocity.X + (size.X / 2), position.Y + Velocity.Y + (size.Y / 2));
                Vector2 boxMiddle = new(Box.X + (Box.Z / 2), Box.Y + (Box.W / 2));

                float angleBoxToPlayer = (float)Math.Atan2(playerMiddle.Y - boxMiddle.Y, playerMiddle.X - boxMiddle.X);

                float angleTopLeft = (float)Math.Atan2(Box.Y - boxMiddle.Y, Box.X - boxMiddle.X);
                float angleBottomLeft = (float)Math.Atan2((Box.Y + Box.W) - boxMiddle.Y, Box.X - boxMiddle.X);
                float angleTopRight = (float)Math.Atan2(Box.Y - boxMiddle.Y, (Box.X + Box.Z) - boxMiddle.X);
                float angleBottomRight = (float)Math.Atan2((Box.Y + Box.Z) - boxMiddle.Y, (Box.X + Box.Z) - boxMiddle.X);
#if DEBUG
                ImGui.GetForegroundDrawList().AddLine(playerMiddle, boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1)));
                ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X, Box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X, Box.Y + Box.W), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X + Box.Z, Box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X + Box.Z, Box.Y + Box.W), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
#endif

                //Left
                if ((angleBoxToPlayer < angleTopLeft && angleBoxToPlayer > -Math.PI) || (angleBoxToPlayer > angleBottomLeft && angleBoxToPlayer < Math.PI))
                {
                    Velocity.X = 0;
                    position.X = Box.X - size.X;
                }

                //Top
                if (angleBoxToPlayer > angleTopLeft && angleBoxToPlayer < angleTopRight)
                {
                    Velocity.Y = 0;
                    position.Y = Box.Y - size.Y;
                }


                //Right
                if ((angleBoxToPlayer > angleTopRight && angleBoxToPlayer < 0) || (angleBoxToPlayer > 0 && angleBoxToPlayer < angleBottomRight))
                {
                    Velocity.X = 0;
                    position.X = Box.X + Box.Z;
                }

                //Bottom
                if (angleBoxToPlayer < angleBottomLeft && angleBoxToPlayer > angleBottomRight)
                {
                    Velocity.Y = 0;
                    position.Y = Box.Y + Box.W + 0.1f;
                }
            }
        }
    }
}
