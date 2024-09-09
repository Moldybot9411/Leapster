﻿using ImGuiNET;
using Leapster.ObjectSystem;
using Silk.NET.SDL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
    }

    public override void Update()
    {
        rect = AssignedObject.Rect;
        CheckCollisions();


        //Gravity
        Velocity.Y += Config.Gravity * ImGui.GetIO().DeltaTime * Config.TimeScale;

        //Movement
        if (ImGui.IsKeyDown(ImGuiKey.A) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickLeft))
        {
            Velocity.X = -Speed * Config.TimeScale;
        }

        if (ImGui.IsKeyDown(ImGuiKey.D) || ImGui.IsKeyDown(ImGuiKey.GamepadLStickRight))
        {
            Velocity.X = Speed * Config.TimeScale;
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
            Velocity.Y = JumpForce;
            //Screenshake.Shake(30f, 10f);

            //particles = new(new Vector2(position.X + (size.X / 2), position.Y + size.Y));

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
                    Velocity.X = 0;
                    rect.X = Box.X - rect.Width;
                }

                //Top
                if (angleBoxToPlayer > angleTopLeft && angleBoxToPlayer < angleTopRight)
                {
                    Velocity.Y = 0;
                    rect.Y = Box.Y - rect.Height;
                    IsGrounded = true;
                }

                //Right
                if ((angleBoxToPlayer > angleTopRight && angleBoxToPlayer < 0) || (angleBoxToPlayer > 0 && angleBoxToPlayer < angleBottomRight))
                {
                    Velocity.X = 0;
                    rect.X = Box.X + Box.Z;
                }

                //Bottom
                if (angleBoxToPlayer < angleBottomLeft && angleBoxToPlayer > angleBottomRight)
                {
                    Velocity.Y = 0;
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

    private async void JumpPressed()
    {
        await Task.Delay(jumpBuffer);
        jumpQueued = false;
    }
}
