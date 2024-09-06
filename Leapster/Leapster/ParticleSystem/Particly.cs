using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Leapster.ParticleSystem;

public class Particly
{
    public int Amount = 10;
    public float InitialVelocityStrength = 2.0f;
    public Vector2 Position = new(50, 50);
    public Vector4 Color = new(1, 0, 0, 1);
    public float StartSize = 10.0f;

    private List<Particle> existingParticles;
    private Random rand;

    public Particly()
    {
        Game.Instance.GameScreen.OnRender += Render;
    }

    public void Start()
    {
        for(int i = 0; i < Amount; i++)
        {
            Vector2 startVelocity = new Vector2((float)rand.NextDouble() * InitialVelocityStrength, (float)rand.NextDouble() * InitialVelocityStrength);
            existingParticles.Add(new Particle(Color, startVelocity, StartSize));
        }
    }

    private void Render()
    {
        ImGui.GetBackgroundDrawList().AddCircle(new Vector2(50, 50), 10f, ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.8f, 0.8f, 1)));
    }
}

public class Particle
{
    public Vector2 RelativePosition;
    public Vector2 AbsolutePosition;
    public bool WorldSpace = false;
    public float Size;
    public Vector4 Color;
    public Vector2 velocity;

    public Particle(Vector4 color, Vector2 initialVelocity, float size)
    {
        Color = color;
        velocity = initialVelocity;
        Size = size;
    }

    public void UpdatePosition()
    {
        Console.WriteLine("YURRRRR");
    }
}
