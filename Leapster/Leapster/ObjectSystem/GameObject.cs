using System.Drawing;

namespace Leapster.ObjectSystem;

public class GameObject
{
    public List<Component> Components = [];

    public string Name;
    public RectangleF Rect;

    public GameObject(RectangleF rect, string name)
    {
        Name = name;
        Rect = rect;
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
        Game.Instance.GameScreen.OnRender += component.Update;
    }

    public void RemoveComponent(Component component)
    {
        Components.Remove(component);
        Game.Instance.GameScreen.OnRender -= component.Update;
    }
}

