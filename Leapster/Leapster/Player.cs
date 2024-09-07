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
    private int jumpBuffer = 150;

    private bool jumpQueued = false;
    private bool isGrounded = false;

    public Player()
    {
        Game.Instance.GameScreen.OnRender += OnRenderFrame;
    }

    private void OnRenderFrame()
    {
        UpdatePosition();

		Vector2 topLeft = position;
		Vector2 bottomRight = topLeft + size;
		Vector2 center = (topLeft + bottomRight) * 0.5f;

		ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
		draw.AddRectFilled(topLeft + Screenshake.ShakeOffset, bottomRight + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1)));

		// Draw the centered "H"
		ImGui.PushFont(Game.Instance.BigFont); // Replace 'yourFont' with the font you want to use
		Vector2 textSize = ImGui.CalcTextSize("H");
		Vector2 textPosition = center - textSize * 0.5f + Screenshake.ShakeOffset;
		draw.AddText(textPosition, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), "H");
		ImGui.PopFont();
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
        Velocity.X = float.Lerp(0, Velocity.X, t);


        //Jumping
        if ((ImGui.IsKeyPressed(ImGuiKey.Space) || ImGui.IsKeyPressed(ImGuiKey.GamepadFaceDown)))
        {
            jumpQueued = true;
            JumpPressed();
        }

        if(Velocity.Y <= 0.15f && Velocity.Y >= 0 && jumpQueued && isGrounded)
        {
            Velocity.Y = JumpForce;
            Screenshake.Shake(30f, 0.01f);
            jumpQueued = false;
        }


        //Updating Positions based on Velocity
        position.Y += Velocity.Y;
        position.X += Velocity.X;
    }

    private void CheckCollisions()
    {
        //Set to true when Player stood on any surface during the Frame. Is used to determine isGrounded
        isGrounded = false;

        foreach (Vector4 Box in Game.Instance.CurrentLevel.Boxes)
        {
            if(Colliding(Box))
            {
                Vector2 playerMiddle = new(position.X + (size.X / 2), position.Y + (size.Y / 2));
                Vector2 boxMiddle = new(Box.X + (Box.Z / 2), Box.Y + (Box.W / 2));

                float angleBoxToPlayer = (float)Math.Atan2(playerMiddle.Y - boxMiddle.Y, playerMiddle.X - boxMiddle.X);

                float angleTopLeft = (float)Math.Atan2(Box.Y - boxMiddle.Y, Box.X - boxMiddle.X);
                float angleBottomLeft = (float)Math.Atan2((Box.Y + Box.W) - boxMiddle.Y, Box.X - boxMiddle.X);
                float angleTopRight = (float)Math.Atan2(Box.Y - boxMiddle.Y, (Box.X + Box.Z) - boxMiddle.X);
                float angleBottomRight = (float)Math.Atan2((Box.Y + Box.Z) - boxMiddle.Y, (Box.X + Box.Z) - boxMiddle.X);

#if DEBUG
                if (Config.DebugMode)
                {
                    ImGui.GetForegroundDrawList().AddLine(playerMiddle, boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X, Box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X, Box.Y + Box.W), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X + Box.Z, Box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(Box.X + Box.Z, Box.Y + Box.W), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                }
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
                    isGrounded = true;
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

    private async void JumpPressed()
    {
        await Task.Delay(jumpBuffer);
        jumpQueued = false;
    }

    private bool Colliding(Vector4 box)
    {
        Vector4 rightEdge = new(position.X + size.X, position.Y, position.X + size.X, position.Y + size.Y);
        Vector4 leftEdge = new(position.X, position.Y, position.X, position.Y + size.Y);
        Vector4 topEdge = new(position.X, position.Y, position.X + size.X, position.Y);
        Vector4 bottomEdge = new(position.X, position.Y + size.Y, position.X + size.X, position.Y + size.Y);

        if (rightEdge.X > box.X && leftEdge.X < box.X + box.Z && bottomEdge.Y > box.Y && topEdge.Y < box.Y + box.W)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
