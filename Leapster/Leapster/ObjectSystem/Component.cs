namespace Leapster.ObjectSystem;

public abstract class Component : IDisposable
{
    public GameObject AssignedObject { get; internal set; }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        Game.Instance.GameScreen.OnRender -= Update;
    }

    public virtual void Start()
    {
        Game.Instance.GameScreen.OnRender += Update;
    }

    public virtual void Update()
    {
        //Intentionally empty
    }
}
