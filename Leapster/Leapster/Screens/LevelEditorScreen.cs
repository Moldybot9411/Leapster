using ImGuiNET;
using System.Numerics;

namespace Leapster.Screens;

public class LevelEditorScreen : Screen
{
    private Level dummyLevel;

    public override void Show()
    {
        dummyLevel = new();
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

        ImGui.SetWindowSize(Vector2.Zero);

        ImGui.End();
    }

    private Vector4 box;

    public override void RenderImGui()
    {
        RenderLevelEditorWindow();

        //dummyLevel.OnRender();
    }

}
