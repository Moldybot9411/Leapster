﻿using ImGuiNET;
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
    public int Amount = 5000;
    public float InitialVelocityStrength = 150.0f;
    public Vector2 Position = new(50, 50);
    public Vector4 Color = new(0.8f, 0.8f, 0.8f, 1);
    public float StartSize = 2.0f;
    public float LifeTime = 0.5f;

    private List<Particle> existingParticles = new List<Particle>();

    public Particly(Vector2 position)
    {
        Position = position;

        Start();
        Game.Instance.GameScreen.OnRender += Render;
    }

    public void Start()
    {
        for(int i = 0; i < Amount; i++)
        {
            Vector2 startVelocity = new Vector2(GetRandomNumber(-1.0f, 1.0f), GetRandomNumber(-1.0f, 1.0f));

            existingParticles.Add(new Particle(Position, Color, Vector2.Normalize(startVelocity) * InitialVelocityStrength * GetRandomNumber(0.1f, 1f), StartSize, LifeTime * GetRandomNumber(0.9f, 1.0f), true));
        }
    }

    private void Render()
    {
        foreach(Particle particle in existingParticles)
        {
            particle.UpdatePosition();

            if(particle.LifeTime >= 0)
            {
                ImGui.GetBackgroundDrawList().AddCircleFilled(particle.AbsolutePosition, particle.Size, ImGui.ColorConvertFloat4ToU32(particle.Color));
            }
        }

        bool allParticlesDead = existingParticles.Any(p => p.LifeTime <= 0);

        if (allParticlesDead)
        {
            existingParticles.Clear();
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

    public Particle(Vector2 startPosition, Vector4 color, Vector2 initialVelocity, float size, float lifetime, bool fadesize)
    {
        AbsolutePosition = startPosition;
        Color = color;
        Velocity = initialVelocity;
        Size = size;
        LifeTime = lifetime;
        FadeSize = fadesize;
    }

    public void UpdatePosition()
    {
        AbsolutePosition += Velocity * ImGui.GetIO().DeltaTime;

        if(FadeSize)
        {
            Size = float.Lerp(Size, 0, LifeTime * ImGui.GetIO().DeltaTime);
        }

        LifeTime -= ImGui.GetIO().DeltaTime;
    }
}
