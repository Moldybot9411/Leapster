using ImGuiNET;
using Leapster.ObjectSystem;
using System.Numerics;



namespace Leapster.Components;

internal class H : Component
{
    public override void Update()
    {
        Vector2 topLeft = new(AssignedObject.Rect.X, AssignedObject.Rect.Y);
        Vector2 bottomRight = new(AssignedObject.Rect.X + AssignedObject.Rect.Width, AssignedObject.Rect.Y + AssignedObject.Rect.Height);
        Vector2 center = (topLeft + bottomRight) * 0.5f;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        draw.AddRectFilled(topLeft + Screenshake.ShakeOffset, bottomRight + Screenshake.ShakeOffset, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1)));

        // Draw the centered "H"
        ImGui.PushFont(Game.Instance.BigFont);
        Vector2 textSize = ImGui.CalcTextSize("H");
        Vector2 textPosition = center - textSize * 0.5f + Screenshake.ShakeOffset;
        draw.AddText(textPosition, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), "H");
        ImGui.PopFont();
    }
}
