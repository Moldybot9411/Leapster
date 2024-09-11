﻿using ImGuiNET;
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

    private Vector4 spawnColor = new(1f, 1f, 1f, 1f);
    private Vector2 objectSpawnSize = new(100, 100);

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

    private void SpawnObject(Vector2 position)
    {
        if (currentObjectType == (int)EditorObjectType.PlayerSpawn)
        {
            if (level.Objects.Where(obj => obj.Type == EditorObjectType.PlayerSpawn).Any())
                return;
        }

        level.Objects.Add(new EditorObject()
        {
            ViewRect = new RectangleF(new PointF(position), new SizeF(objectSpawnSize)),
            Color = spawnColor,
            Type = (EditorObjectType)currentObjectType
        });
    }

    private void DuplicateObject(EditorObject obj)
    {
        EditorObject clonedObject = (EditorObject)obj.Clone();

        clonedObject.ViewRect.Location += clonedObject.ViewRect.Size;
        level.Objects.Add(clonedObject);
    }

    private void RenderLevelEditorWindow()
    {
        if (!ImGui.Begin("Level Editor"))
        {
            ImGui.End();
            return;
        }

        if (ImGui.CollapsingHeader("Scene"))
        {
            for (int i = 0; i < level.Objects.Count; i++)
            {
                EditorObject obj = level.Objects[i];

                if (!ImGui.TreeNode($"({i}) {Enum.GetName(obj.Type)}"))
                    continue;

                // TODO: Add position sliders

                string popupName = $"ColorEdit##{i}";
                if (ImGui.Button("Edit color"))
                {
                    ImGui.OpenPopup(popupName);
                }

                if (ImGui.BeginPopup(popupName))
                {
                    ImGui.ColorPicker4("Color", ref obj.Color);
                    ImGui.EndPopup();
                }

                if (ImGui.Button("Delete"))
                {
                    level.Objects.RemoveAt(i);
                }

                if (ImGui.Button("Duplicate"))
                {
                    DuplicateObject(obj);
                }

                ImGui.TreePop();
            }
        }

        if (ImGui.CollapsingHeader("Spawn##Header"))
        {
            ImGui.Combo("Type", ref currentObjectType, objectTypeNames, objectTypeNames.Length);

            string popupName = "SpawnColorPicker";
            if (ImGui.Button("Edit Color"))
            {
                ImGui.OpenPopup(popupName);
            }

            if (ImGui.BeginPopup(popupName))
            {
                ImGui.ColorPicker4("Color", ref spawnColor);
                ImGui.EndPopup();
            }

            if (ImGui.Button("Spawn"))
            {
                SpawnObject(new Vector2(10, 10));
            }

            ImGui.SameLine();
            ImGuiExtensions.HelpMarker("You can also double click to spawn a box.");
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
                    Game.Instance.Configuration.LevelsFolder = result.Path;
                }
            }
            ImGui.SetItemTooltip("Open Levels folder");

            ImGui.SameLine();
            ImGui.InputText("Levels folder", ref Game.Instance.Configuration.LevelsFolder, 1000);

            // Save Level
            if (ImGui.Button(FontAwesome6.FloppyDisk))
            {
                string legalName = string.Join("_", level.Name.Split(Path.GetInvalidFileNameChars()));
                string fileName = Path.Combine(Game.Instance.Configuration.LevelsFolder, legalName + ".json");

                File.WriteAllText(fileName, JsonConvert.SerializeObject(level, Formatting.Indented));
            }
            ImGui.SetItemTooltip("Save the current level");

            ImGui.SameLine();

            // Load Level
            if (ImGui.Button(FontAwesome6.FolderPlus))
            {
                DialogResult result = Dialog.FileOpen("json", Game.Instance.Configuration.LevelsFolder);

                if (result.IsOk)
                {
                    Game.Instance.Configuration.LevelsFolder = new FileInfo(result.Path).Directory.FullName;

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

    private void RenderButtons()
    {
        ImGui.Begin("Buttons",
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoSavedSettings);

        ImGui.PushStyleColor(ImGuiCol.Button, Color.FromArgb(255, 225, 67, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.FromArgb(255, 255, 100, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.FromArgb(255, 133, 42, 29).ToImGuiColor());

        if (ImGui.Button(FontAwesome6.Bars))
        {
            Game.Instance.ShowScreen(Game.Instance.MainmenuScreen);
        }

        ImGui.PopStyleColor(3);

        ImGui.SetWindowPos(new Vector2(10, 10));
        ImGui.SetWindowSize(Vector2.Zero);

        ImGui.End();
    }

    public void RenderImGui()
    {
        RenderButtons();

        RenderLevelEditorWindow();

        if (level == null)
            return;

        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.Transparent.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Border, Color.Black.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, Color.Red.ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.Button, Color.Blue.ToImGuiColor());

        PointF cursorPos = new(ImGui.GetIO().MousePos);

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            Vector2 spawnPos = ImGui.GetIO().MousePos - objectSpawnSize / 2;
            SpawnObject(spawnPos);
        }

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

                case EditorObjectType.Goal:
                {
                    draw.AddRectFilled(topLeft, bottomRight, Color.Yellow.ToImGuiColor());

                    break;
                }

                case EditorObjectType.PlayerSpawn:
                    draw.AddCircle((obj.ViewRect.Location + obj.ViewRect.Size / 2).ToVector2(), 20, Color.Red.ToImGuiColor());
                    break;
            }

            string popupName = $"BoxColor##{i}";

            if (obj.ViewRect.Contains(cursorPos) || ImGui.IsPopupOpen(popupName))
            {
                if (ImGui.Button($"{FontAwesome6.PaintRoller}##{i}"))
                {
                    ImGui.OpenPopup(popupName);
                }

                ImGui.SameLine();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.1f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.05f, 0.05f, 1.0f));

                if (ImGui.Button($"{FontAwesome6.TrashCan}##{i}"))
                {
                    level.Objects.RemoveAt(i);

                    // Generate new random so imgui doesnt move our windows around
                    currentLevelRandom = new Random().Next();

                    ImGui.PopStyleColor(3);
                    ImGui.End();
                    continue;
                }

                ImGui.SameLine();

                if (ImGui.Button($"{FontAwesome6.Copy}##{i}") && obj.Type != EditorObjectType.PlayerSpawn)
                {
                    DuplicateObject(obj);
                }

                ImGui.PopStyleColor(3);
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
            obj.CollisionRect = obj.ViewRect;

            level.Objects[i] = obj;

            ImGui.End();
        }

        ImGui.PopStyleColor(4);
    }

}
