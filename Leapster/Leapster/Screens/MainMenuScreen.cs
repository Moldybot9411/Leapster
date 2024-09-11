using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class MainmenuScreen : IScreen
{
    private bool inOptionsWindow = false;

    private Vector2 childSize = Vector2.Zero;
    private readonly int[] resolutionInput = [0, 0];

    private static Config Configuration => Game.Instance.Configuration;

    public unsafe void Show()
    {
        Game.Instance.SdlInstance.GetWindowSize(Game.Instance.ApplicationWindow, ref resolutionInput[0], ref resolutionInput[1]);

        Game.Instance.clearColor = Color.FromArgb(255, 21, 10, 8);
    }

    public void Hide()
    {
    }

    private void RenderOptionsMenu()
    {
        if (!ImGui.Begin("Options", ref inOptionsWindow))
        {
            ImGui.End();
            return;
        }

        ImGui.TextColored(new Vector4(1, 0, 0, 1), "Changing the resolution is definetly not recommended in this game");

        ImGui.InputInt2("Resolution", ref resolutionInput[0]);

        if (ImGui.Button("Apply"))
        {
			Configuration.Resolution = new Size(resolutionInput[0], resolutionInput[1]);
            Configuration.SaveConfig();
            unsafe
            {
                Game.Instance.SdlInstance.SetWindowSize(Game.Instance.ApplicationWindow, resolutionInput[0], resolutionInput[1]);
            }
        }

        if (ImGui.Checkbox("FPS Overlay", ref Configuration.FpsOverlay))
        {
            Configuration.FpsOverlay = Configuration.FpsOverlay;
            Configuration.SaveConfig();
        }

#if DEBUG
        if (ImGui.Checkbox("Debug mode", ref Configuration.DebugMode))
        {
            Configuration.SaveConfig();
        }
#endif

        ImGui.End();
    }

    public void RenderImGui()
    {
        if (ImGui.IsKeyDown(ImGuiKey.ModCtrl) &&
            ImGui.IsKeyDown(ImGuiKey._6) &&
            ImGui.IsKeyPressed(ImGuiKey._9))
        {
            Configuration.HMode = !Configuration.HMode;
        }

        ImGui.Begin("test",
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoInputs | 
            ImGuiWindowFlags.NoDocking);

        Vector2 parentSize = ImGui.GetWindowSize();

        ImGui.SetNextWindowPos((parentSize - childSize) / 2);

        //Colors
        ImGui.PushStyleColor(ImGuiCol.Text, Color.FromArgb(255, 247, 239, 238).ToImGuiColor());

        ImGui.PushStyleColor(ImGuiCol.Button, Color.FromArgb(255, 225, 67, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.FromArgb(255, 255, 100, 44).ToImGuiColor());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.FromArgb(255, 133, 42, 29).ToImGuiColor());

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10);
        ImGui.PushFont(Game.Instance.BigFont);

        if (ImGui.BeginChild("mainMenu", Vector2.Zero, ImGuiChildFlags.AutoResizeX | ImGuiChildFlags.AutoResizeY))
        {
            Vector2 buttonSize = new Vector2(200, 100);

            Vector2 cursorPos = ImGui.GetCursorPos();

            ImGui.SetCursorPosX((childSize.X - ImGui.CalcTextSize("LEAPSTER").X) / 2f);
            ImGui.Text("LEAPSTER");

            ImGui.SetCursorPosX(cursorPos.X);

            ImGui.Dummy(new Vector2(0.0f, 100.0f));

            if (ImGui.Button("Level Select", buttonSize))
            {
                Game.Instance.ShowScreen(Game.Instance.LevelSelectScreen);
            }

            if (ImGui.Button("Options", buttonSize))
            {
                inOptionsWindow = !inOptionsWindow;
            }

            if (ImGui.Button("Level Editor", buttonSize))
            {
                Game.Instance.ShowScreen(new LevelEditorScreen());
            }

            childSize = ImGui.GetWindowSize();

            ImGui.EndChild();
        }

        ImGui.PopFont();

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(4);

        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetWindowPos(Vector2.Zero);

        ImGui.End();

        if (inOptionsWindow)
        {
            RenderOptionsMenu();
        }
    }

}
