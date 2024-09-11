using Leapster.Audio;
using Leapster.ObjectSystem;
using System.Drawing;


namespace Leapster.Components;

internal class Player : Component
{
    private Random rand = new();

    private bool emitted = false;

    private int fireworkCount = 5;

    public override void Start()
    {
        Game.Instance.GameScreen.OnTriggerEvent += OnTriggerEvent;

        base.Start();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Game.Instance.GameScreen.OnTriggerEvent -= OnTriggerEvent;
    }

    private void OnTriggerEvent(string tag, GameObject obj)
    {
        if (tag == "Goal")
        {
            if (emitted)
                return;

            int savedCollectedCoins = Game.Instance.GameScreen.GetLevelPlayerData(Game.Instance.GameScreen.currentLevelHash).Item1;
            int coinCountToSave = Game.Instance.GameScreen.CoinsCollected > savedCollectedCoins ? Game.Instance.GameScreen.CoinsCollected : savedCollectedCoins;

            Game.Instance.GameScreen.SaveLevelPlayerData(coinCountToSave, true, true);
            Game.Instance.Configuration.SaveConfig();

            EmitFireworks();

            emitted = true;

            AssignedObject.Dispose();

            Game.Instance.GameScreen.RemoveGameObject(AssignedObject);
            Game.Instance.GameScreen.PlayerObj = null;
        }

        if (tag == "Spike")
        {
            Screenshake.Shake(100, 10f);
            Game.Instance.GameScreen.ReloadLevelDelayed(1500);

            AssignedObject.Dispose();

            Game.Instance.GameScreen.RemoveGameObject(AssignedObject);
            Game.Instance.GameScreen.PlayerObj = null;

            AudioEngine.Instance.PlayResourceSound("explosion0.wav");
        }
    }

    private async void EmitFireworks()
    {
        Random rand = new();
        List<GameObject> goals = Game.Instance.GameScreen.gameObjects.ToList().FindAll(obj => obj.Name.Contains("Goal"));

        if (goals == null)
            return;

        for (int i = 0; i < fireworkCount; i++)
        {
            foreach (GameObject go in goals)
            {
                RectangleF r = new(new(go.Rect.Location.X + go.Rect.Width / 2, go.Rect.Y), new SizeF(10, 10));
                GameObject p = new GameObject(r, "Firework");


                p.AddComponent(new Particly(AssignedObject.Rect.Location.ToVector2())
                {
                    Color = new((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1),
                    Amount = 100,
                    InitialVelocityStrength = 80.0f,
                    LifeTime = 4.0f,
                    StartSize = 10.0f,
                    LerpSpeed = 3.0f
                });

                p.AddComponent(new RigidBody()
                {
                    Velocity = new(0, -600),
                });

                p.AddComponent(new Firework());

            }

            AudioEngine.Instance.PlayResourceSound("explosion2.wav");

            await Task.Delay(500);
        }

        await Task.Delay(1500);
        //Game.Instance.GameScreen.QueueSync(Game.Instance.GameScreen.UnloadLevel);
        Game.Instance.GameScreen.UnloadLevel();
        Game.Instance.ShowScreen(Game.Instance.LevelSelectScreen);
    }
}
