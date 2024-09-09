using ImGuiNET;
using Leapster.LevelEditor;
using NativeFileDialogSharp;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;
using System.Reflection;

namespace Leapster.Screens;

public class LevelEditorScreen : IScreen
{
    private EditorLevel level;

    private int currentObjectType = 0;
    private string[] objectTypeNames;

    private Vector4 spawnColor = new(0, 0.5f, 0, 0.3f);

    private string levelsFolder = "";

    private float gravity;

    private int currentLevelRandom;

    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
        objectTypeNames = Enum.GetNames<EditorObjectType>();

        LoadLevel(new());
    }

    public void Hide()
    {
    }

    private void LoadLevel(EditorLevel level)
    {
        this.level = null;
        gravity = level.Gravity / 100;
        this.level = level;

        currentLevelRandom = new Random().Next();
    }

    private void RenderLevelEditorWindow()
    {
        if (!ImGui.Begin("Level Editor"))
        {
            ImGui.End();
            return;
        }

        if (ImGui.CollapsingHeader("Spawn##Header"))
        {
            ImGui.Combo("Type", ref currentObjectType, objectTypeNames, objectTypeNames.Length);
            ImGui.ColorPicker4("Color", ref spawnColor);

            if (ImGui.Button("Spawn"))
            {
                level.Objects.Add(new EditorObject()
                {
                    ViewRect = new RectangleF(10, 10, 100, 100),
                    Color = spawnColor,
                    Type = (EditorObjectType)currentObjectType
                });
            }
        }

        if (ImGui.CollapsingHeader("Level Settings"))
        {
            ImGui.InputText("Name", ref level.Name, 100);

            if (ImGui.SliderFloat("Gravity", ref gravity, 0, 20))
            {
                level.Gravity = gravity * 100;
            }

            ImGui.SliderFloat("Time scale", ref level.TimeScale, 0, 4);
        }

        if (ImGui.CollapsingHeader("Save/Load"))
        {

            // Open folder save path
            if (ImGui.Button(FontAwesome6.FolderOpen))
            {
                string currentPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                DialogResult result = Dialog.FolderPicker(currentPath);

                if (result.IsOk)
                {
                    levelsFolder = result.Path;
                }
            }
            ImGui.SetItemTooltip("Open Levels folder");

            ImGui.SameLine();
            ImGui.InputText("Levels folder", ref levelsFolder, 1000);

            // Save Level
            if (ImGui.Button(FontAwesome6.FloppyDisk))
            {
                string legalName = string.Join("_", level.Name.Split(Path.GetInvalidFileNameChars()));
                string fileName = Path.Combine(levelsFolder, legalName + ".json");

                File.WriteAllText(fileName, JsonConvert.SerializeObject(level, Formatting.Indented));
            }
            ImGui.SetItemTooltip("Save the current level");

            ImGui.SameLine();

            // Load Level
            if (ImGui.Button(FontAwesome6.FolderPlus))
            {
                DialogResult result = Dialog.FileOpen("json", levelsFolder);

                if (result.IsOk)
                {
                    string contents = File.ReadAllText(result.Path);

                    EditorLevel loadedLevel = JsonConvert.DeserializeObject<EditorLevel>(contents);
                    LoadLevel(loadedLevel);
                }

            }
            ImGui.SetItemTooltip("Load a level");
        }

        ImGui.SetWindowSize(Vector2.Zero, ImGuiCond.Once);

        ImGui.End();
    }

    public void RenderImGui()
    {
        RenderLevelEditorWindow();

        if (level == null)
            return;

        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.Transparent.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Border, Color.Black.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, Color.Red.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Button, Color.Blue.ToImGuiColor());

        PointF cursorPos = new(ImGui.GetIO().MousePos);

        for (int i = 0; i < level.Objects.Count; i++)
        {
            EditorObject obj = level.Objects[i];

            Vector2 topLeft = obj.ViewRect.Location.ToVector2();
            Vector2 bottomRight = topLeft + obj.ViewRect.Size.ToVector2();

            if (!ImGui.Begin($"Box##{currentLevelRandom}##{i}", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoSavedSettings))
            {
                ImGui.End();
                continue;
            }

            ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

            switch (obj.Type)
            {
                case EditorObjectType.Box:
                    draw.AddRectFilled(topLeft, bottomRight, obj.Color.ToImguiColor());
                    break;

                case EditorObjectType.Spike:
                {
                    RectangleF rect = obj.ViewRect;
                    Vector2 p0 = new(rect.X + rect.Width / 2, rect.Y); // Top-center point of the rectangle
                    Vector2 p1 = new(rect.X, rect.Y + rect.Height); // Bottom-left point of the rectangle
                    Vector2 p2 = new(rect.X + rect.Width, rect.Y + rect.Height); // Bottom-right point of the rectangle
                    
                    draw.AddTriangleFilled(p0, p1, p2, obj.Color.ToImguiColor());
                    break;
                }
            }

            string popupName = $"BoxColor##{i}";

            if (obj.ViewRect.Contains(cursorPos) || ImGui.IsPopupOpen(popupName))
            {
                if (ImGui.Button($"{FontAwesome6.PaintRoller}##{i}"))
                {
                    ImGui.OpenPopup(popupName);
                }
            }

            if (ImGui.BeginPopup(popupName))
            {
                ImGui.ColorPicker4("Box color", ref obj.Color);

                ImGui.EndPopup();
            }

            ImGui.SetWindowPos(topLeft, ImGuiCond.Once);
            ImGui.SetWindowSize(obj.ViewRect.Size.ToVector2(), ImGuiCond.Once);

            obj.ViewRect.Size = new SizeF(ImGui.GetWindowSize());
            obj.ViewRect.Location = new PointF(ImGui.GetWindowPos());
            level.Objects[i] = obj;

            ImGui.End();
        }

        ImGui.PopStyleColor(4);
    }

}
