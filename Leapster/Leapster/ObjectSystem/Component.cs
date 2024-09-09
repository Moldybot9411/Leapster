using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leapster.ObjectSystem;

public abstract class Component
{
    public GameObject AssignedObject { get; internal set; }

    public virtual void Start()
    {
        //Intentionally empty
    }

    public virtual void Update()
    {
        //Intentionally empty
    }
}
