using ImGuiNET;
using Leapster.ObjectSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.Components;

public class RigidBody : Component
{
    public bool IsGrounded { get; private set; }
    public Vector2 Velocity = new();

    private RectangleF rect;

    public override void Start()
    {
        rect = AssignedObject.Rect;
    }

    public override void Update()
    {
        rect = AssignedObject.Rect;
        CheckCollisions();


        //Gravity
        Velocity.Y += Config.Gravity * ImGui.GetIO().DeltaTime * Config.TimeScale;


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

        foreach (GameObject obj in Game.Instance.GameScreen.gameObjects)
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
                float angleBottomRight = MathF.Atan2((box.Y + box.Width) - boxMiddle.Y, (box.X + box.Width) - boxMiddle.X);

#if DEBUG
                if (Config.DebugMode)
                {
                    ImGui.GetForegroundDrawList().AddLine(playerMiddle, boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(box.X, box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(box.X, box.Y + box.Height), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(box.X + box.Width, box.Y), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
                    ImGui.GetForegroundDrawList().AddLine(new Vector2(box.X + box.Width, box.Y + box.Height), boxMiddle, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 0, 1)));
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
                if ((angleBoxToPlayer > angleTopRight && angleBoxToPlayer < 0) || (angleBoxToPlayer > 0 && angleBoxToPlayer < angleBottomRight))
                {
                    Velocity.X = 0;
                    rect.X = box.X + box.Width;
                }

                //Bottom
                if (angleBoxToPlayer < angleBottomLeft && angleBoxToPlayer > angleBottomRight)
                {
                    Velocity.Y = 0;
                    rect.Y = box.Y + box.Height + 0.1f;
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
}
