using Newtonsoft.Json;
using System.Drawing;

namespace Leapster;

public class Config
{
	public static string ConfigPath => Path.GetFullPath("./config.json");

	public Size Resolution = new(1280, 720);
	public bool FpsOverlay;
	public string LevelsFolder = Path.GetFullPath("./Levels/");
	public int TotalCoinsCollected;
	public bool HMode;

	// hashcode, (coins collected in each played level, level played, level completed)
	public Dictionary<string, (int, bool, bool)> PlayerLevelData = [];

#if DEBUG
	public bool DebugMode;
#endif

	public static Config LoadConfig()
	{
		return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
	}

	public void SaveConfig()
	{
		File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
	}
}
