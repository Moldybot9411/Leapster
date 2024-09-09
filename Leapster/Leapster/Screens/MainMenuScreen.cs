using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class MainmenuScreen : IScreen
{
    private bool inOptionsWindow = false;

    private Vector2 childSize = Vector2.Zero;
    private int[] resolutionInput = [0, 0];

    public unsafe void Show()
    {
        Game.Instance.SdlInstance.GetWindowSize(Game.Instance.ApplicationWindow, ref resolutionInput[0], ref resolutionInput[1]);

        Game.Instance.clearColor = Color.FromArgb(240, 40, 15, 15);
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

        ImGui.InputInt2("Resolution", ref resolutionInput[0]);

        if (ImGui.Button("Apply"))
        {
            unsafe
            {
                Game.Instance.SdlInstance.SetWindowSize(Game.Instance.ApplicationWindow, resolutionInput[0], resolutionInput[1]);
            }
        }

        ImGui.Checkbox("FPS Overlay", ref Game.Instance.GameScreen.FPSOverlay);

        ImGui.End();
    }

    public void RenderImGui()
    {
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

            if (ImGui.Button("Start", buttonSize))
            {
                Game.Instance.ShowScreen(Game.Instance.GameScreen);
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

        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetWindowPos(Vector2.Zero);

        ImGui.End();

        if (inOptionsWindow)
        {
            RenderOptionsMenu();
        }
    }

}
