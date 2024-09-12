using ImGuiNET;
using System.Numerics;
using Leapster.ObjectSystem;
using System.Collections.Concurrent;

namespace Leapster.Components;

public class Particly : Component 
{ 
    public int Amount = 100;
    public float InitialVelocityStrength = 1500.0f;
    public Vector2 Position = new(50, 50);
    public Vector4 Color = new(1, 1, 0, 1);
    public float StartSize = 2.0f;
    public float LifeTime = 0.05f;
    public float LerpSpeed = 3.0f;

    private ConcurrentQueue<Particle> existingParticles = new ConcurrentQueue<Particle>();

    public Particly(Vector2 position)
    {
        Position = position;

        Game.Instance.GameScreen.OnRender += Render;
    }

    public override void Start()
    {
        for(int i = 0; i < Amount; i++)
        {
            Vector2 startVelocity = new Vector2(GetRandomNumber(-1.0f, 1.0f), GetRandomNumber(-1.0f, 1.0f));

            existingParticles.Enqueue(new Particle(
                Vector2.Zero, 
                Color, 
                Vector2.Normalize(startVelocity) * InitialVelocityStrength * GetRandomNumber(0.1f, 1f), 
                StartSize, 
                LifeTime * GetRandomNumber(0.9f, 1.0f), 
                true, 
                LerpSpeed)
            );
        }

        base.Start();
    }

    private void Render()
    {
        foreach(Particle particle in existingParticles)
        {
            particle.UpdatePosition();

            if(particle.LifeTime >= 0)
            {
                ImGui.GetBackgroundDrawList().AddCircleFilled(particle.AbsolutePosition + Position + Screenshake.ShakeOffset, particle.Size, ImGui.ColorConvertFloat4ToU32(particle.Color));
            }
        }

        bool allParticlesDead = existingParticles.Any(p => p.LifeTime <= 0);

        if (allParticlesDead)
        {
            existingParticles.Clear();
            AssignedObject.Dispose();
        }
    }

    private float GetRandomNumber(float minimum, float maximum)
    {
        Random random = new Random();
        return (float)random.NextDouble() * (maximum - minimum) + minimum;
    }
}

public class Particle
{
    public Vector2 RelativePosition;
    public Vector2 AbsolutePosition;
    public bool WorldSpace = false;
    public bool FadeSize = true;
    public float Size;
    public float LifeTime;
    public Vector4 Color;
    public Vector2 Velocity;
    public float LerpSpeed;

    public Particle(Vector2 startPosition, Vector4 color, Vector2 initialVelocity, float size, float lifetime, bool fadesize, float lerpSpeed)
    {
        AbsolutePosition = startPosition;
        Color = color;
        Velocity = initialVelocity;
        Size = size;
        LifeTime = lifetime;
        FadeSize = fadesize;
        LerpSpeed = lerpSpeed;
    }

    public void UpdatePosition()
    {
        AbsolutePosition += Velocity * ImGui.GetIO().DeltaTime;

        if(FadeSize)
        {
            Size = float.Lerp(Size, 0, LerpSpeed * ImGui.GetIO().DeltaTime);
        }

        LifeTime -= ImGui.GetIO().DeltaTime;
    }
}
