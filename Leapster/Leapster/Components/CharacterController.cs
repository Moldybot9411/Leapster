using ImGuiNET;
using Leapster.Audio;
using Leapster.ObjectSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Components;

public class CharacterController : Component
{
    //This script is basically a copy of the Rigidbody component, as it seemed impossible to add Controls in another component due to timing issues.
    //This was the cleanest I could think of and I'm sorry :(

    public float Speed = 250f;
    public float JumpForce = -350f;
    public bool IsGrounded;
    public Vector2 Velocity;

    private bool jumpQueued;
    private int jumpBuffer = 170;
    private RectangleF rect;

    public override void Start()
    {
        rect = AssignedObject.Rect;
        base.Start();
    }

    public override void Update()
    {
        CheckCollisions();
        //Gravity
        Velocity.Y += Game.Instance.GameScreen.Gravity * ImGui.GetIO().DeltaTime * Game.Instance.GameScreen.TimeScale;

        //Movement
        if (ImGui.IsKeyDown(ImGuiKey.A) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickLeft))
        {
            Velocity.X = -Speed * Game.Instance.GameScreen.TimeScale;
        }

        if (ImGui.IsKeyDown(ImGuiKey.D) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickRight))
        {
            Velocity.X = Speed * Game.Instance.GameScreen.TimeScale;
        }

        //Lerping Speed to 0
        Velocity.X = float.Lerp(Velocity.X, 0, 8.5f * ImGui.GetIO().DeltaTime);


        //Jumping
        if ((ImGui.IsKeyPressed(ImGuiKey.Space) || ImGui.IsKeyPressed(ImGuiKey.GamepadFaceDown)))
        {
            jumpQueued = true;
            JumpPressed();
        }

        if (jumpQueued && IsGrounded)
        {
            AudioEngine.Instance.PlayResourceSound("jump.wav");
            Velocity.Y = JumpForce;
            Screenshake.Shake(30f, 10f);

            jumpQueued = false;
        }


        rect.Y += Velocity.Y * ImGui.GetIO().DeltaTime;
        rect.X += Velocity.X * ImGui.GetIO().DeltaTime;

        Vector2 tl = new(rect.X, rect.Y);
        Vector2 br = new(rect.X + rect.Width, rect.Y + rect.Height);

        AssignedObject.Rect = rect;
        ImGui.GetBackgroundDrawList().AddRectFilled(tl + Screenshake.ShakeOffset, br + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new(1, 0, 0, 1)));
    }

    private void CheckCollisions()
    {
        //Set to true when Player stood on any surface during the Frame
        IsGrounded = false;

        foreach (GameObject obj in Game.Instance.GameScreen.gameObjects.Where(obj => obj.HasComponent<Box>() && obj.GetComponent<Box>().Collidable))
        {
            RectangleF box = obj.Rect;

            //if (Colliding(box))
            if (box.IntersectsWith(rect))
            {
                Vector2 playerMiddle = new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

                Vector2 boxMiddle = box.Location.ToVector2() + box.Size.ToVector2() / 2;
                float angleBoxToPlayer = MathF.Atan2(playerMiddle.Y - boxMiddle.Y, playerMiddle.X - boxMiddle.X);

                float angleTopLeft = MathF.Atan2(box.Y - boxMiddle.Y, box.X - boxMiddle.X);
                float angleBottomLeft = MathF.Atan2((box.Y + box.Height) - boxMiddle.Y, box.X - boxMiddle.X);
                float angleTopRight = MathF.Atan2(box.Y - boxMiddle.Y, (box.X + box.Width) - boxMiddle.X);
                float angleBottomRight = MathF.Atan2((box.Y + box.Height) - boxMiddle.Y, (box.X + box.Width) - boxMiddle.X);

#if DEBUG
                if (Game.Instance.Configuration.DebugMode)
                {
                    ImDrawListPtr draw = ImGui.GetForegroundDrawList();

                    draw.AddLine(playerMiddle, boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1)));
                    draw.AddLine(new Vector2(box.X, box.Y) + Screenshake.ShakeOffset, boxMiddle + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    draw.AddLine(new Vector2(box.X, box.Y + box.Height) + Screenshake.ShakeOffset, boxMiddle + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    draw.AddLine(new Vector2(box.X + box.Width, box.Y) + Screenshake.ShakeOffset, boxMiddle + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
					draw.AddLine(new Vector2(box.X + box.Width, box.Y + box.Height) + Screenshake.ShakeOffset, boxMiddle + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                }
#endif

                //Left
                if ((angleBoxToPlayer < angleTopLeft && angleBoxToPlayer > -Math.PI) || (angleBoxToPlayer > angleBottomLeft && angleBoxToPlayer < Math.PI))
                {
                    Velocity.X = 0;
                    rect.X = box.X - rect.Width;
                }

                //Top
                if (angleBoxToPlayer > angleTopLeft && angleBoxToPlayer < angleTopRight)
                {
                    Velocity.Y = 0;
                    rect.Y = box.Y - rect.Height;
                    IsGrounded = true;
                }

                //Right
                if ((angleBoxToPlayer > angleTopRight && angleBoxToPlayer < 0f) || (angleBoxToPlayer > 0f && angleBoxToPlayer < angleBottomRight))
                {
                    Velocity.X = 0;
                    rect.X = box.X + box.Width;
                }

                //Bottom
                if (angleBoxToPlayer < angleBottomLeft && angleBoxToPlayer > angleBottomRight)
                {
                    Velocity.Y = 0;
                    rect.Y = box.Y + box.Height;
                }
            }
        }
    }

    private bool Colliding(Vector4 box)
    {
        Vector4 rightEdge = new(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
        Vector4 leftEdge = new(rect.X, rect.Y, rect.X, rect.Y + rect.Height);
        Vector4 topEdge = new(rect.X, rect.Y, rect.X + rect.Width, rect.Y);
        Vector4 bottomEdge = new(rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height);

        bool colliding = rightEdge.X > box.X && leftEdge.X < box.X + box.Z && bottomEdge.Y > box.Y && topEdge.Y < box.Y + box.W;

        return colliding;
    }

    private async void JumpPressed()
    {
        await Task.Delay(jumpBuffer);
        jumpQueued = false;
    }
}
