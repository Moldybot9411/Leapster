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
