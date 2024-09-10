using ImGuiNET;
using Leapster.ObjectSystem;
using System.Numerics;

namespace Leapster.Components;

internal class Image : Component
{
    private string path;

    private IntPtr textureId;

    public Image(string pathToImage)
    {
        path = pathToImage;
    }

    public override void Start()
    {
        textureId = ImGuiExtensions.LoadImage(path, out _);
        
        base.Start();
    }

    public override void Update()
    {
        Vector2 topLeft = new(AssignedObject.Rect.X, AssignedObject.Rect.Y);
        Vector2 bottomRight = new(AssignedObject.Rect.X + AssignedObject.Rect.Width, AssignedObject.Rect.Y + AssignedObject.Rect.Height);

        ImGui.GetBackgroundDrawList().AddImage(textureId, topLeft + Screenshake.ShakeOffset, bottomRight + Screenshake.ShakeOffset);
    }

}
