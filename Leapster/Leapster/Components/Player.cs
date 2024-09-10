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

    private int fireworkCount = 10;

    public Player()
    {
        Game.Instance.GameScreen.OnTriggerEvent += OnTriggerEvent;
    }

    private void OnTriggerEvent(string Tag)
    {
        if(Tag == "Goal")
        {
            if (emitted)
                return;

            EmitFireworks();

            emitted = true;
            //Game.Instance.GameScreen.Unloadlevel();
        }

        if(Tag == "Spike")
        {
            GameObject player = Game.Instance.GameScreen.gameObjects.Find(obj => obj.HasComponent<CharacterController>());
            Game.Instance.GameScreen.gameObjects.Remove(player);

            Game.Instance.GameScreen.ReloadLevelDelayed(1500);

            Game.Instance.GameScreen.PlayerObj = null;

            AssignedObject.Dispose();
        }
    }

    private async void EmitFireworks()
    {
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

            await Task.Delay(500);
        }
    }
}
