using ImGuiNET;
using Leapster.ObjectSystem;
using System.Numerics;

namespace Leapster.Components;

public class Box : Component
{

    public Vector4 Color;

    public override void Update()
    {
        Vector2 topLeft = new(AssignedObject.Rect.X, AssignedObject.Rect.Y);
        Vector2 bottomRight = new(AssignedObject.Rect.X + AssignedObject.Rect.Width, AssignedObject.Rect.Y + AssignedObject.Rect.Height);
        
        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        draw.AddRectFilled(topLeft, bottomRight, Color.ToImguiColor());
    }

}
