namespace Leapster.LevelEditor;

public class EditorLevel
{
    public string Name = "New Level";

    public List<EditorObject> Objects { get; private set; } = [];

    public float Gravity = 980f;
    public float TimeScale = 1f;
}
