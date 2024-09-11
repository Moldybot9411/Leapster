using ImGuiNET;
using Leapster.ObjectSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Components;

internal class Trigger : Component
{
    public RectangleF Bounds;
    public string Tag { get; private set; } = "";

    private Vector2 customOffset = new();

    private GameObject playerObj;

    public Trigger(string tag)
    {
        Tag = tag;
    }

    public override void Start()
    {
        Bounds = AssignedObject.Rect;

        base.Start();
    }

    public override void Update()
    {
        playerObj = Game.Instance.GameScreen.PlayerObj;

        if(playerObj == null)
        {
            return;
        }

        RectangleF updatedBound = new RectangleF(Bounds.X + customOffset.X, Bounds.Y + customOffset.Y, Bounds.Width, Bounds.Height);

        if (updatedBound.IntersectsWith(playerObj.Rect))
        {
            Game.Instance.GameScreen.OnTrigger(Tag, AssignedObject);
        }

        Vector2 tlObj = AssignedObject.Rect.Location.ToVector2();
        Vector2 brObj = tlObj + AssignedObject.Rect.Size.ToVector2();

        Vector2 tlCol = Bounds.Location.ToVector2() + customOffset;
        Vector2 brCol = tlCol + Bounds.Size.ToVector2();

#if DEBUG
        if (Game.Instance.Configuration.DebugMode)
        {
            ImGui.GetForegroundDrawList().AddRect(tlCol, brCol, Color.Blue.ToImGuiColor());
        }
#endif
    }

    public void SetCustomBounds(Vector2 offsetFromOrigin, Vector2 size)
    {
        customOffset = offsetFromOrigin;
        Bounds.Size = new SizeF(size.X, size.Y);
    }
}
