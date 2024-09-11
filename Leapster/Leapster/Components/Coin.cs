using ImGuiNET;
using Leapster.ObjectSystem;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leapster.Audio;

namespace Leapster.Components;

internal class Coin : Component
{
    public float Size = 15f;
    public float RotationSpeed = 1.5f;

    private Vector2 radius;
    private float time;


    public override void Start()
    {
        radius = new(Size, Size);

        Game.Instance.GameScreen.currentLevelCoinCount += 1;

        Console.WriteLine(Game.Instance.GameScreen.currentLevelCoinCount);

        Game.Instance.GameScreen.OnTriggerEvent += OnTrigger;

        base.Start();
    }

    public override void Dispose()
    {
        base.Dispose();

        Game.Instance.GameScreen.OnTriggerEvent -= OnTrigger;
    }


    public override void Update()
    {
        AssignedObject.GetComponent<Trigger>().SetCustomBounds(AssignedObject.Rect.Size.ToVector2() / 2 - new Vector2(15f, 15f), new(30, 30));

        time += ImGui.GetIO().DeltaTime;
        float sin = MathF.Sin(time * RotationSpeed);
        radius.X = sin * Size;

        ImGui.GetBackgroundDrawList().AddEllipseFilled(AssignedObject.Rect.Location.ToVector2() + AssignedObject.Rect.Size.ToVector2() * 0.5f, radius, Color.Orange.ToImGuiColor());
        //ImGui.GetBackgroundDrawList().AddEllipse(AssignedObject.Rect.Location.ToVector2() + AssignedObject.Rect.Size.ToVector2() * 0.5f, radius, Color.Yellow.ToImGuiColor());
    }

    private void OnTrigger(string tag, GameObject obj)
    {
        if(tag == "Coin" && obj == AssignedObject)
        {
            Game.Instance.Configuration.TotalCoinsCollected += 1;
            Game.Instance.GameScreen.CoinsCollected += 1;
            Console.WriteLine(Game.Instance.Configuration.TotalCoinsCollected);

            AudioEngine.Instance.PlayResourceSound("coin0.wav");

            AssignedObject.Dispose();
            Game.Instance.GameScreen.RemoveGameObject(AssignedObject);
        }
    }
}
