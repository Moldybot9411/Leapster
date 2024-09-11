﻿using ImGuiNET;
using Leapster.LevelEditor;
using NativeFileDialogSharp;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;
using System.Reflection;

namespace Leapster.Screens;

public class LevelSelectScreen : IScreen
{
    private readonly List<LevelData> levels = [];

    private class LevelData
    {
        public string Name;
        public string Path;
        public string CreationTime;
    }

    public unsafe void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 21, 10, 8);

        if (IsLevelsFolderValid())
        {
            RescanLevelFolder();
        }
    }

    public void Hide()
    {
    }

    public void RenderImGui()
    {
        ImGui.Begin("LevelSelect",
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoDocking |
            ImGuiWindowFlags.NoSavedSettings);

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);
        ImGui.PushFont(Game.Instance.BigFont);

        ImGui.PushStyleColor(ImGuiCol.Text, Color.FromArgb(255, 247, 239, 238).ToImGuiColor());

        ImGui.PushStyleColor(ImGuiCol.Button, Color.FromArgb(255, 225, 67, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.FromArgb(255, 255, 100, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.FromArgb(255, 133, 42, 29).ToImGuiColor());

        if (ImGui.Button(FontAwesome6.FolderOpen))
        {
            string currentPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            DialogResult result = Dialog.FolderPicker(currentPath);

            if (result.IsOk)
            {
                Game.Instance.Configuration.LevelsFolder = result.Path;
                RescanLevelFolder();
            }
        }
        ImGui.SetItemTooltip("Open Levels folder");

        ImGui.SameLine();
        ImGui.InputText("Levels folder", ref Game.Instance.Configuration.LevelsFolder, 1000);

        ImGui.SameLine();
        if (ImGui.Button(FontAwesome6.Retweet))
        {
            RescanLevelFolder();
        }
        ImGui.SetItemTooltip("Rescan Folder");

        ImGui.SameLine();
        if (ImGui.Button(FontAwesome6.Bars))
        {
            Game.Instance.ShowScreen(Game.Instance.MainmenuScreen);
        }
        ImGui.SetItemTooltip("Back to Main Menu");

        if (ImGui.BeginTable("Levels", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Date of creation");
            ImGui.TableSetupColumn("##bruh", ImGuiTableColumnFlags.None, 0.1f);

            ImGui.TableHeadersRow();

            for (int i = 0; i < levels.Count; i++)
            {
                LevelData level = levels[i];

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.Text(level.Name);

                ImGui.TableNextColumn();

                ImGui.Text(level.CreationTime);

                ImGui.TableNextColumn();

                if (ImGui.Button($"Play##{i}"))
                {
                    Game.Instance.GameScreen.LoadLevel(level.Path);
                    Game.Instance.ShowScreen(Game.Instance.GameScreen);
                }
            }

            ImGui.EndTable();
        }

        if (ImGui.BeginPopupModal("ERROR", ImGuiWindowFlags.NoResize))
        {
            ImGui.Text("Specified level path is not valid!");

            if (ImGui.Button("Close"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.PopFont();

        ImGui.PopStyleColor(4);
        ImGui.PopStyleVar();

        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetWindowPos(Vector2.Zero);

        ImGui.End();
    }

    private static bool IsLevelsFolderValid() => Directory.Exists(Game.Instance.Configuration.LevelsFolder);

	private void RescanLevelFolder()
    {
        if (!IsLevelsFolderValid())
        {
            Console.WriteLine("Specified level path is not valid!");
            ImGui.OpenPopup("ERROR");
            return;
        }

        levels.Clear();

        string[] files = Directory.GetFiles(Game.Instance.Configuration.LevelsFolder);

        foreach (string file in files)
        {
            try
            {
                EditorLevel level = JsonConvert.DeserializeObject<EditorLevel>(File.ReadAllText(file));

                if (level == null)
                    continue;

                levels.Add(new LevelData()
                {
                    Name = level.Name,
                    CreationTime = File.GetCreationTime(file).ToString(),
                    Path = file
                });
            } catch(Exception)
            {
                // Ignored, just dont display level no one cares
            }
        }
    }

}
