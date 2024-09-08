using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Leapster.ParticleSystem;

public class Particly
{
    public int Amount = 100;
    public float InitialVelocityStrength = 50.0f;
    public Vector2 Position = new(50, 50);
    public Vector4 Color = new(1, 0, 0, 1);
    public float StartSize = 10.0f;

    private List<Particle> existingParticles = new List<Particle>();

    public Particly()
    {
        Start();
        Game.Instance.GameScreen.OnRender += Render;
    }

    public void Start()
    {
        for(int i = 0; i < Amount; i++)
        {
            Vector2 startVelocity = new Vector2(GetRandomNumber(-1.0f, 1.0f) * InitialVelocityStrength, GetRandomNumber(-1.0f, 1.0f) * InitialVelocityStrength);
            existingParticles.Add(new Particle(Position, Color, startVelocity, StartSize));
        }
    }

    private void Render()
    {
        foreach(Particle particle in existingParticles)
        {
            particle.UpdatePosition();
            ImGui.GetBackgroundDrawList().AddCircleFilled(particle.AbsolutePosition, particle.Size, ImGui.ColorConvertFloat4ToU32(particle.Color));
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
    public float Size;
    public float LifeTime;
    public Vector4 Color;
    public Vector2 Velocity;

    public Particle(Vector2 startPosition, Vector4 color, Vector2 initialVelocity, float size)
    {
        AbsolutePosition = startPosition;
        Color = color;
        Velocity = initialVelocity;
        Size = size;
    }

    public void UpdatePosition()
    {
        AbsolutePosition += Velocity * ImGui.GetIO().DeltaTime;
    }
}
