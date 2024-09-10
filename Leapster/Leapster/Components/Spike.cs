using ImGuiNET;
using Leapster.ObjectSystem;
using System.Drawing;
using System.Numerics;

namespace Leapster.Components;

public class Spike : Component
{
    public Vector4 Color;

    private bool killing = false;

    public override void Update()
    {
        RectangleF rect = AssignedObject.Rect;
        Vector2 p0 = new(rect.X + rect.Width / 2, rect.Y); // Top-center point of the rectangle
        Vector2 p1 = new(rect.X, rect.Y + rect.Height); // Bottom-left point of the rectangle
        Vector2 p2 = new(rect.X + rect.Width, rect.Y + rect.Height); // Bottom-right point of the rectangle

        ImGui.GetBackgroundDrawList().AddTriangleFilled(p0 + Screenshake.ShakeOffset, p1 + Screenshake.ShakeOffset, p2 + Screenshake.ShakeOffset, Color.ToImguiColor());

        GameObject playerObject = Game.Instance.GameScreen.PlayerObj;

        if (playerObject == null)
            return;

        if (rect.IntersectsWith(playerObject.Rect))
        {
            if (killing)
                return;

            killing = true;

            playerObject.AddComponent(new Particly(playerObject.Rect.Location.ToVector2())
            {
                Amount = 1500
            });

            KillPlayerDelayedAsync();
        }
    }

    private async void KillPlayerDelayedAsync()
    {
        await Task.Delay(1500);

        GameObject playerObject = Game.Instance.GameScreen.PlayerObj;

        playerObject.Dispose();
        Game.Instance.GameScreen.gameObjects.Remove(playerObject);
        Game.Instance.GameScreen.PlayerObj = null;

        killing = false;
    }

}
