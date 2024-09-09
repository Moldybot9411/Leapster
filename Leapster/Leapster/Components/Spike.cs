using ImGuiNET;
using Leapster.ObjectSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Components;

public class Spike : Component
{
    public Vector4 Color;

    public override void Update()
    {
        RectangleF rect = AssignedObject.Rect;
        Vector2 p0 = new(rect.X + rect.Width / 2, rect.Y); // Top-center point of the rectangle
        Vector2 p1 = new(rect.X, rect.Y + rect.Height); // Bottom-left point of the rectangle
        Vector2 p2 = new(rect.X + rect.Width, rect.Y + rect.Height); // Bottom-right point of the rectangle

        ImGui.GetBackgroundDrawList().AddTriangleFilled(p0, p1, p2, Color.ToImguiColor());
    }
}
