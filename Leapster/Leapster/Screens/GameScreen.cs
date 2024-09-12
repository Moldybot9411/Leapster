using ImGuiNET;
using Leapster.Audio;
using Leapster.Components;
using Leapster.LevelEditor;
using Leapster.ObjectSystem;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using System.Reflection;

namespace Leapster.Screens;

public class GameScreen : IScreen
{
    public event Action OnRender = delegate { };
    public event Action<string, GameObject> OnTriggerEvent = delegate { };

    public ConcurrentBag<GameObject> gameObjects = [];

    public float Gravity;
    public float TimeScale;

    public GameObject PlayerObj;

    private string currentLevelPath = "";
    public string currentLevelHash;
    public int currentLevelCoinCount;
    public int CoinsCollected;

    public GameScreen()
    {
        string prefix = typeof(Program).Namespace + ".Assets.Sounds.";
        foreach (string name in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(n => n.StartsWith(prefix)))
        {
            AudioEngine.Instance.CacheResource(name);
        }
    }

    public void Show()
    {
        Game.Instance.clearColor = Color.FromArgb(255, 115, 140, 153);
    }

    private void SpawnPlayer(PointF startPos)
    {
        PlayerObj = new(new RectangleF(startPos, new SizeF(20, 40)), "Player");
        PlayerObj.AddComponent(new Player());
        PlayerObj.AddComponent(new CharacterController());

        if (Game.Instance.Configuration.HMode)
        {
            PlayerObj.AddComponent(new H());
        }

        gameObjects.Add(PlayerObj);
    }

    public void Hide()
    {
    }

    public void LoadLevel(string levelFile)
    {
        string content = File.ReadAllText(levelFile);
        EditorLevel level = JsonConvert.DeserializeObject<EditorLevel>(content);
        currentLevelPath = levelFile;
        LoadLevel(level);
    }

    public void LoadLevel(EditorLevel level)
    {
        currentLevelHash = Game.ComputeFileHash(currentLevelPath);

        SaveLevelPlayerData(GetLevelPlayerData(currentLevelHash).Item1, true, GetLevelPlayerData(currentLevelHash).Item3);
        Game.Instance.Configuration.SaveConfig();

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
                        Color = obj.Color,
                        Collidable = true
                    });

                    break;

                case EditorObjectType.Spike:
                    gameObject.AddComponent(new Trigger("Spike"));
                    gameObject.AddComponent(new Spike()
                    {
                        Color = obj.Color
                    });

                    break;

                case EditorObjectType.Goal:
                    gameObject.AddComponent(new Trigger("Goal"));
                    gameObject.AddComponent(new Box()
                    {
                        Color = new(1, 1, 0, 1),
                        Collidable = false
                    });

                    break;

                case EditorObjectType.Coin:
                    gameObject.AddComponent(new Coin());
                    gameObject.AddComponent(new Trigger("Coin"));

                    break;
            }

            gameObjects.Add(gameObject);
        }
    }

    public void UnloadLevel()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.Dispose();
        }

        PlayerObj = null;
        CoinsCollected = 0;
        gameObjects.Clear();
    }

    public void ReloadLevel()
    {
        UnloadLevel();
        CoinsCollected = 0;

        LoadLevel(currentLevelPath);
    }

    public async void ReloadLevelDelayed(int delay)
    {
        await Task.Delay(delay);

        ReloadLevel();
    }

    public void RenderImGui()
    {
        if (Game.Instance.Configuration.FpsOverlay)
        {
            ImGui.GetBackgroundDrawList().AddText(new Vector2(10, 10), Color.Lime.ToImGuiColor(), $"FPS: {ImGui.GetIO().Framerate:F2}");
        }

        if (ImGui.IsKeyPressed(ImGuiKey.Escape))
        {
            UnloadLevel();
            Game.Instance.ShowScreen(Game.Instance.LevelSelectScreen);
        }

        if (ImGui.IsKeyPressed(ImGuiKey.R))
        {
            ReloadLevel();
        }

        OnRender();
    }

    public void OnTrigger(string tag, GameObject obj)
    {
        OnTriggerEvent(tag, obj);
    }

    public void RemoveGameObject(GameObject itemToRemove)
    {
        lock (gameObjects)
        {
            var tempBag = new ConcurrentBag<GameObject>();

            foreach (var obj in gameObjects)
            {
                if (!obj.Equals(itemToRemove))
                {
                    tempBag.Add(obj);
                }
            }

            gameObjects = tempBag;
        }
    }

    public void SaveLevelPlayerData(int coinsCollected, bool levelPlayed, bool levelCompleted)
    {
        if (Game.Instance.Configuration.PlayerLevelData.ContainsKey(currentLevelHash))
        {
            Game.Instance.Configuration.PlayerLevelData[currentLevelHash] = (coinsCollected, levelPlayed, levelCompleted);
        }
        else
        {
            Game.Instance.Configuration.PlayerLevelData.Add(currentLevelHash, (coinsCollected, levelPlayed, levelCompleted));
        }
    }

    public (int, bool, bool) GetLevelPlayerData(string hash)
    {
        if (Game.Instance.Configuration.PlayerLevelData.ContainsKey(hash))
        {
            return Game.Instance.Configuration.PlayerLevelData[hash];
        }
        else
        {
            return (0, false, false);
        }
    }
}
