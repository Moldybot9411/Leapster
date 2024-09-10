using ImGuiNET;
using Leapster.ObjectSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Components;

public class Spike : Component
{
    public Vector4 Color;

    private Trigger triggerRef;

    private GameObject playerObject;

    public override void Start()
    {
        Game.Instance.GameScreen.OnTriggerEvent += CollisionEvent;
        triggerRef = AssignedObject.GetComponent<Trigger>();

        base.Start();
    }

    public override void Dispose()
    {
        base.Dispose();

        Game.Instance.GameScreen.OnTriggerEvent -= CollisionEvent;
    }

    public override void Update()
    {
        RectangleF rect = AssignedObject.Rect;
        Vector2 p0 = new(rect.X + rect.Width / 2, rect.Y); // Top-center point of the rectangle
        Vector2 p1 = new(rect.X, rect.Y + rect.Height); // Bottom-left point of the rectangle
        Vector2 p2 = new(rect.X + rect.Width, rect.Y + rect.Height); // Bottom-right point of the rectangle

        ImGui.GetBackgroundDrawList().AddTriangleFilled(p0 + Screenshake.ShakeOffset, p1 + Screenshake.ShakeOffset, p2 + Screenshake.ShakeOffset, Color.ToImguiColor());

        playerObject = Game.Instance.GameScreen.PlayerObj;

        if (triggerRef != null)
        {
            //Imitates collision Shape of Geometry Dash spikes
            RectangleF r = AssignedObject.Rect;
            triggerRef.Bounds = new(r.X + (r.Width * 0.35f),r.Y + (r.Height * 0.3f), r.Width * 0.3f, r.Height * 0.7f);
        }
    }

    private void CollisionEvent(string Tag)
    {
        if(Tag == "Spike" && playerObject != null)
        {
            GameObject particles = new GameObject(playerObject.Rect, "DeathParticles");
            particles.AddComponent(new Particly(playerObject.Rect.Location.ToVector2())
            {
                Amount = 50,
                Color = new Vector4(1, 0, 0, 1),
                InitialVelocityStrength = 80.0f,
                LifeTime = 4.0f,
                StartSize = 10.0f,
                LerpSpeed = 3.0f,
            });
        }
    }
}
