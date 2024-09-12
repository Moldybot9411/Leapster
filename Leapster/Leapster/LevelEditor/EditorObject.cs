using System.Drawing;
using System.Numerics;

namespace Leapster.LevelEditor;

public class EditorObject : ICloneable
{
    public RectangleF ViewRect;
    public RectangleF? CollisionRect;

    public Vector4 Color;

    public EditorObjectType Type;

    public object Clone()
    {
        return MemberwiseClone();
    }
}
