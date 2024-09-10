using Leapster.ObjectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.Components;

internal class Firework : Component
{
    private Particly particles;
    private RigidBody rb;

    public override void Start()
    {
        particles = AssignedObject.GetComponent<Particly>();
        rb = AssignedObject.GetComponent<RigidBody>();

        base.Start();
    }

    public override void Update()
    {
        if(particles == null && rb == null)
        {
            particles = AssignedObject.GetComponent<Particly>();
            rb = AssignedObject.GetComponent<RigidBody>();
            return;
        }

        particles.Position = rb.AssignedObject.Rect.Location.ToVector2();
    }
}
