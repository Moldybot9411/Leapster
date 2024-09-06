using ImGuiNET;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Leapster.Screens;

public class LevelEditorScreen : Screen
{

    private class Box
    {
        public RectangleF Rect;
        public Vector4 Color;

        public Box(RectangleF rect, Vector4 color)
        {
            Rect = rect;
            Color = color;
        }
    }

    private List<Box> boxes = [];

    public override void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    public override void Hide()
    {
    }

    private void RenderLevelEditorWindow()
    {
        if (!ImGui.Begin("Level Editor"))
        {
            ImGui.End();
            return;
        }

        if (ImGui.Button("Spawn"))
        {
            boxes.Add(new(new RectangleF(10, 10, 100, 100), new Vector4(1, 0, 0, 1)));
        }

        ImGui.SetWindowSize(Vector2.Zero, ImGuiCond.Once);

        ImGui.End();
    }
    private bool dragging = false;

    public override void RenderImGui()
    {
        RenderLevelEditorWindow();

        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.Transparent.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Border, Color.Black.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, Color.Red.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Button, Color.Blue.ToImGuiColor());

        for (int i = 0; i < boxes.Count; i++)
        {
            Box box = boxes[i];

            Vector2 topLeft = box.Rect.Location.ToVector2();
            Vector2 bottomRight = topLeft + box.Rect.Size.ToVector2();

            ImGui.Begin($"Box##{i}", ImGuiWindowFlags.NoTitleBar);

            ImGui.GetWindowDrawList().AddRectFilled(topLeft, bottomRight, box.Color.ToImguiColor());

            if (ImGui.IsWindowHovered() && ImGui.Button(FontAwesome6.PaintRoller))
            {
                ImGui.OpenPopup($"BoxColor##{i}");
            }

            if (ImGui.BeginPopup($"BoxColor##{i}"))
            {
                ImGui.ColorPicker4("Box color", ref box.Color);

                ImGui.EndPopup();
            }

            ImGui.SetWindowPos(topLeft, ImGuiCond.Once);
            ImGui.SetWindowSize(box.Rect.Size.ToVector2(), ImGuiCond.Once);

            box.Rect.Size = new SizeF(ImGui.GetWindowSize());
            box.Rect.Location = new PointF(ImGui.GetWindowPos());
            boxes[i] = box;

            dragging = (ImGui.IsWindowFocused() || ImGui.IsWindowHovered()) && ImGui.IsMouseDragging(ImGuiMouseButton.Left) && ImGui.IsMousePosValid();

            ImGui.End();
        }

        ImGui.PopStyleColor(4);
    }

}
