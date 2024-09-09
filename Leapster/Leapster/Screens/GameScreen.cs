using ImGuiNET;
using Leapster.Components;
using Leapster.LevelEditor;
using Leapster.ObjectSystem;
using Leapster.ParticleSystem;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : IScreen
{
    public event Action OnRender = delegate { };

    public bool FPSOverlay = false;

    public List<GameObject> gameObjects = [];


    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);

        GameObject player = new(new RectangleF(200, 50, 20, 40), "Player");
        player.AddComponent(new CharacterController());
        player.AddComponent(new H());

        gameObjects.Add(player);
    }

    public void Hide()
    {
    }

    public void LoadLevel(string levelFile)
    {
        string content = File.ReadAllText(levelFile);
        EditorLevel level = JsonConvert.DeserializeObject<EditorLevel>(content);
        LoadLevel(level);
    }

    public void LoadLevel(EditorLevel level)
    {
        for (int i = 0; i < level.Objects.Count; i++)
        {
            EditorObject obj = level.Objects[i];
            GameObject gameObject = new GameObject(obj.ViewRect, $"({i}) {Enum.GetName(obj.Type)}");

            switch (obj.Type)
            {
                case EditorObjectType.Box:
                    gameObject.AddComponent(new Box()
                    {
                        Color = obj.Color
                    });
                    //gameObject.AddComponent(new H());
                    break;

                case EditorObjectType.Spike:
                    gameObject.AddComponent(new Spike()
                    {
                        Color = obj.Color
                    });
                    gameObject.AddComponent(new H());
                    break;
            }

            gameObjects.Add(gameObject);
        }
    }

    public void RenderImGui()
    {
        if (FPSOverlay)
        {
            ImGui.GetBackgroundDrawList().AddText(new Vector2(10, 10), Color.Lime.ToImGuiColor(), $"FPS: {ImGui.GetIO().Framerate:F2}");
        }

        OnRender();
    }
}
