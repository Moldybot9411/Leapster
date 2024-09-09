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

    private RectangleF rect;
    private Vector2 velocity = new();

    public override void Start()
    {
        rect = AssignedObject.Rect;
    }

    public override void Update()
    {
        rect = AssignedObject.Rect;
        CheckCollisions();

        //Gravity
        velocity.Y += Config.Gravity * ImGui.GetIO().DeltaTime * Config.TimeScale;

        rect.Y += velocity.Y * ImGui.GetIO().DeltaTime;

        Vector2 tl = new(rect.X, rect.Y);
        Vector2 br = new(rect.X + rect.Width, rect.Y + rect.Height);

        AssignedObject.Rect = rect;
        ImGui.GetBackgroundDrawList().AddRectFilled(tl + Screenshake.ShakeOffset, br + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new(1, 0, 0, 1)));
    }

    private void CheckCollisions()
    {
        //Set to true when Player stood on any surface during the Frame
        IsGrounded = false;

        foreach (Vector4 Box in Game.Instance.CurrentLevel.Boxes)
        {
            if (Colliding(Box))
            {
                Vector2 playerMiddle = new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
                Vector2 boxMiddle = new(Box.X + (Box.Z / 2), Box.Y + (Box.W / 2));

                float angleBoxToPlayer = MathF.Atan2(playerMiddle.Y - boxMiddle.Y, playerMiddle.X - boxMiddle.X);

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
                    velocity.X = 0;
                    rect.X = Box.X - rect.Width;
                }

                //Top
                if (angleBoxToPlayer > angleTopLeft && angleBoxToPlayer < angleTopRight)
                {
                    velocity.Y = 0;
                    rect.Y = Box.Y - rect.Height;
                    IsGrounded = true;
                }

                //Right
                if ((angleBoxToPlayer > angleTopRight && angleBoxToPlayer < 0) || (angleBoxToPlayer > 0 && angleBoxToPlayer < angleBottomRight))
                {
                    velocity.X = 0;
                    rect.X = Box.X + Box.Z;
                }

                //Bottom
                if (angleBoxToPlayer < angleBottomLeft && angleBoxToPlayer > angleBottomRight)
                {
                    velocity.Y = 0;
                    rect.Y = Box.Y + Box.W + 0.1f;
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
