using Leapster.Audio;
using Leapster.ObjectSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

    private void OnTriggerEvent(string Tag)
    {
        if (Tag == "Goal")
        {
            if (emitted)
                return;

            EmitFireworks();

            emitted = true;

            AssignedObject.Dispose();

            //Refactor pls
            Game.Instance.GameScreen.gameObjects = Game.Instance.RemoveItem(Game.Instance.GameScreen.gameObjects, AssignedObject);
            Game.Instance.GameScreen.PlayerObj = null;
        }

        if (Tag == "Spike")
        {
            Screenshake.Shake(100, 10f);
            Game.Instance.GameScreen.ReloadLevelDelayed(1500);

            AssignedObject.Dispose();

            //Refactor pls
            Game.Instance.GameScreen.gameObjects = Game.Instance.RemoveItem(Game.Instance.GameScreen.gameObjects, AssignedObject);
            Game.Instance.GameScreen.PlayerObj = null;

            AudioEngine.Instance.PlayResourceSound("explosion0.wav");
        }
    }

    private async void EmitFireworks()
    {
        Random rand = new();
        GameObject goal = Game.Instance.GameScreen.gameObjects.ToList().Find(obj => obj.Name.Contains("Goal"));

        if (goal == null)
            return;

        for (int i = 0; i < fireworkCount; i++)
        {
            RectangleF r = new(new(goal.Rect.Location.X + goal.Rect.Width / 2, goal.Rect.Y), new SizeF(10, 10));
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

            AudioEngine.Instance.PlayResourceSound("explosion2.wav");

            await Task.Delay(500);
        }

        await Task.Delay(1500);
        //Game.Instance.GameScreen.QueueSync(Game.Instance.GameScreen.UnloadLevel);
        Game.Instance.GameScreen.UnloadLevel();
        Game.Instance.ShowScreen(Game.Instance.LevelSelectScreen);
    }
}
