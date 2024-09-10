﻿using ImGuiNET;
using Leapster.ObjectSystem;
using Leapster.Screens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.Components;

internal class Trigger : Component
{
    public RectangleF Bounds;
    public string Tag = "";

    private Vector2 customOffset = new(10,10);

    private GameObject playerObj;

    public Trigger(string tag)
    {
        Tag = tag;
    }

    public override void Start()
    {
        Bounds = AssignedObject.Rect;

        base.Start();
    }

    public override void Update()
    {
        if(playerObj == null)
        {
            playerObj = Game.Instance.GameScreen.PlayerObj;
            return;
        }

        RectangleF updatedBound = new RectangleF(Bounds.X + customOffset.X, Bounds.Y + customOffset.Y, Bounds.Width, Bounds.Height);

        if (updatedBound.IntersectsWith(playerObj.Rect))
        {
            Console.WriteLine($"Collided with {Tag}!");
        }

        Vector2 tlObj = AssignedObject.Rect.Location.ToVector2();
        Vector2 brObj = tlObj + AssignedObject.Rect.Size.ToVector2();

        Vector2 tlCol = Bounds.Location.ToVector2() + customOffset;
        Vector2 brCol = tlCol + Bounds.Size.ToVector2();

        ImGui.GetBackgroundDrawList().AddRectFilled(tlObj + Screenshake.ShakeOffset, brObj + Screenshake.ShakeOffset, Color.Yellow.ToImGuiColor());
#if DEBUG
        ImGui.GetBackgroundDrawList().AddRect(tlCol, brCol, Color.Red.ToImGuiColor());
#endif
    }

    public void SetCustomBounds(Vector2 offsetFromOrigin, Vector2 size)
    {
        customOffset = offsetFromOrigin;
        Bounds.Size = new SizeF(size.X, size.Y);
    }
}
