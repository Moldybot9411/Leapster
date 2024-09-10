using ImGuiNET;
using Leapster.Components;
using Leapster.LevelEditor;
using Leapster.ObjectSystem;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;

namespace Leapster.Screens;

public class GameScreen : IScreen
{
    public event Action OnRender = delegate { };

    public List<GameObject> gameObjects = [];

    public float Gravity;
    public float TimeScale;

    public GameObject PlayerObj;

    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    private void SpawnPlayer(PointF startPos)
    {
        PlayerObj = new(new RectangleF(startPos, new SizeF(20, 40)), "Player");
        PlayerObj.AddComponent(new CharacterController());
        PlayerObj.AddComponent(new H());

        gameObjects.Add(PlayerObj);
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
        Gravity = level.Gravity;
        TimeScale = level.TimeScale;

        for (int i = 0; i < level.Objects.Count; i++)
        {
            EditorObject obj = level.Objects[i];
            
            if (obj.Type == EditorObjectType.PlayerSpawn)
            {
                SpawnPlayer(obj.ViewRect.Location + obj.ViewRect.Size / 2);
                continue;
            }
            
            GameObject gameObject = new GameObject(obj.ViewRect, $"({i}) {Enum.GetName(obj.Type)}");

            switch (obj.Type)
            {
                case EditorObjectType.Box:
                    gameObject.AddComponent(new Box()
                    {
                        Color = obj.Color
                    });
                    break;

                case EditorObjectType.Spike:
                    gameObject.AddComponent(new Spike()
                    {
                        Color = obj.Color
                    });
                    break;
            }

            gameObjects.Add(gameObject);
        }
    }

    public void RenderImGui()
    {
        if (Game.Instance.Configuration.FpsOverlay)
        {
            ImGui.GetBackgroundDrawList().AddText(new Vector2(10, 10), Color.Lime.ToImGuiColor(), $"FPS: {ImGui.GetIO().Framerate:F2}");
        }

        OnRender();
    }
}
