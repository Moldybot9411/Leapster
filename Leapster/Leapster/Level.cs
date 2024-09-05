using ImGuiNET;
using System.Globalization;
using System.Numerics;

namespace Leapster;

public class Level
{
    public List<Vector4> Boxes { get; private set; } = new()
    {
        new Vector4(22f, 382f, 300f, 32.093f),
        new Vector4(431f, 479f, 300f, 32.093f),
        new Vector4(755f, 374f, 300f, 32.093f),
        new Vector4(720f, 452f, 93.023f, 32.093f),
        new Vector4(749f, 427f, 93.023f, 32.093f),
        new Vector4(756f, 404f, 93.023f, 32.093f),
        new Vector4(23f, 179f, 100f, 38.983f)
    };

    public Vector4 BoxColor = new(0, 0, 0, 1);

#if DEBUG
    private bool levelEditorOpen = true;
    private bool spawnBox;
    private Vector2 spawnBoxSize = new(10, 10);

    private string generatedText = "";
#endif

    public Level()
    {
        Game.Instance.OnRender += OnRender;
    }

    private void OnRender()
    {
#if DEBUG
        if (levelEditorOpen && ImGui.Begin("Level Editor", ref levelEditorOpen))
        {
            ImGui.ColorPicker4("Box color", ref BoxColor);


            ImGui.Checkbox("Spawn box (SPACE)", ref spawnBox);

            ImGui.SliderFloat2("Box size", ref spawnBoxSize, 0, 100);

            if (ImGui.Button("Generate level data"))
            {
                string FloatToString(float f)
                {
                    return f.ToString(CultureInfo.InvariantCulture) + "f";
                }

                generatedText = string.Join(",\n", Boxes.Select(box => $"new Vector4({FloatToString(box.X)}, {FloatToString(box.Y)}, {FloatToString(box.Z)}, {FloatToString(box.W)})"));
            }

            ImGui.InputTextMultiline("Generated", ref generatedText, (uint)generatedText.Length + 1, Vector2.Zero);

            if (ImGui.CollapsingHeader("Boxes"))
            {
                Vector2 displaySize = ImGui.GetIO().DisplaySize;

                for (int i = 0; i < Boxes.Count; i++)
                {
                    if (!ImGui.TreeNode($"Box #{i}"))
                        continue;

                    bool changed = false;

                    void DoChange(Func<bool> func)
                    {
                        if (func())
                            changed = true;
                    }

                    Vector4 box = Boxes[i];

                    DoChange(() => ImGui.SliderFloat("X", ref box.X, 0, displaySize.X));
                    DoChange(() => ImGui.SliderFloat("y", ref box.Y, 0, displaySize.Y));
                    DoChange(() => ImGui.SliderFloat("Width", ref box.Z, 0, 500));
                    DoChange(() => ImGui.SliderFloat("Height", ref box.W, 0, 500));

                    if (changed)
                        Boxes[i] = box;

                    if (ImGui.Button("Delete"))
                    {
                        Boxes.RemoveAt(i);
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.SetWindowSize(Vector2.Zero, ImGuiCond.Once);
            ImGui.End();
        }

        if (ImGui.IsKeyPressed(ImGuiKey.Space, false) && spawnBox)
        {
            Vector2 mouseLocation = ImGui.GetIO().MousePos;
            Vector4 rect = new(mouseLocation.X, mouseLocation.Y, spawnBoxSize.X, spawnBoxSize.Y);

            Boxes.Add(rect);
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
#endif

        foreach (Vector4 rect in Boxes)
        {
            Vector2 topLeft = new Vector2(rect.X, rect.Y);
            Vector2 topRight = topLeft + new Vector2(rect.Z, rect.W);
            draw.AddRectFilled(topLeft, topRight, ImGui.ColorConvertFloat4ToU32(BoxColor));
        }

    }

}
