using System.Collections.Concurrent;
using System.Drawing;

namespace Leapster.ObjectSystem;

public class GameObject : IDisposable
{
    public ConcurrentBag<Component> Components = [];

    public string Name;
    public RectangleF Rect;

    public GameObject(RectangleF rect, string name)
    {
        Name = name;
        Rect = rect;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (var component in Components)
        {
            component.Dispose();
        }

        Components.Clear();
    }

    public bool HasComponent<T>()
    {
        return Components.Where(component => component.GetType() == typeof(T)).Any();
    }

    public T GetComponent<T>() where T : Component
    {
        return (T)Components.Where(component => component.GetType() == typeof(T)).FirstOrDefault();
    }

    public void AddComponent(Component component)
    {
        Components.Add(component);

        component.AssignedObject = this;
        component.Start();
    }

    public void RemoveComponent(Component component)
    {
        lock (Components)
        {
            var tempBag = new ConcurrentBag<Component>();

            foreach (var obj in Components)
            {
                if (!object.Equals(obj, component))
                {
                    tempBag.Add(obj);
                }
            }

            Components = tempBag;
        }

        component.Dispose();
    }

}

