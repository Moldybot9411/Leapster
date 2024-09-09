using Leapster.ObjectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.Components;

public class Spike : Component
{
    public override void Update()
    {
        Console.WriteLine($"Spike Component from {AssignedObject.Name}");
    }
}
