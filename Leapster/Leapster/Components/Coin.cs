using ImGuiNET;
using Leapster.ObjectSystem;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.Components;

internal class Coin : Component
{
    public float Size = 15f;

    private Vector2 radius;


    public override void Start()
    {
        radius = new(Size, Size);

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

        ImGui.GetBackgroundDrawList().AddEllipseFilled(AssignedObject.Rect.Location.ToVector2() + AssignedObject.Rect.Size.ToVector2() * 0.5f, radius, Color.Orange.ToImGuiColor());
    }

    private void OnTrigger(string Tag)
    {
        Console.WriteLine("Collected Coin!");
    }
}
