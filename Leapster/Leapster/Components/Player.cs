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
            //Game.Instance.GameScreen.Unloadlevel();
        }

        if (Tag == "Spike")
        {
            Game.Instance.GameScreen.ReloadLevelDelayed(1500);

            AssignedObject.Dispose();
            Game.Instance.GameScreen.gameObjects = Game.Instance.RemoveItem(Game.Instance.GameScreen.gameObjects, AssignedObject);
            Game.Instance.GameScreen.PlayerObj = null;

            AudioEngine.Instance.PlayResourceSound("death.wav");
        }
    }

    private async void EmitFireworks()
    {
        Random rand = new();
        for (int i = 0; i < fireworkCount; i++)
        {
            RectangleF r = new(AssignedObject.Rect.Location, new SizeF(10, 10));
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

            await Task.Delay(300);
        }

        await Task.Delay(500);
        //Game.Instance.GameScreen.QueueSync(Game.Instance.GameScreen.UnloadLevel);
        Game.Instance.GameScreen.UnloadLevel();
        Game.Instance.ShowScreen(Game.Instance.LevelSelectScreen);
    }
}
