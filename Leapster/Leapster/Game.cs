using ImGuiNET;
using System.Reflection;
using Leapster.Screens;

namespace Leapster;

public class Game : Application
{
	public static Game Instance { get; private set; }

	public IScreen CurrentScreen { get; private set; } = null;
	public GameScreen GameScreen { get; private set; } = new GameScreen();

	public LevelSelectScreen LevelSelectScreen { get; private set; } = new LevelSelectScreen();

	public ImFontPtr BigFont { get; private set; }

    public Game()
	{
		if (Instance != null)
		{
			throw new Exception("not allowed, game already created");
		}

		Instance = this;
	}

	protected override unsafe void InitImGui()
	{
		base.InitImGui();

		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad | ImGuiConfigFlags.DockingEnable;

		//ImGui.GetIO().NativePtr->IniFilename = null;

		string assetsPath = typeof(Game).Namespace + ".Assets";
		Assembly assembly = Assembly.GetExecutingAssembly();

		ImFontAtlasPtr fonts = ImGui.GetIO().Fonts;

		float baseFontSize = 15f;
		float iconFontSize = baseFontSize / 3 * 3.5f;

		// load small and big version of font
		ImFontPtr robotoFont = fonts.LoadFontFromResources(assetsPath + ".Roboto.Roboto-Regular.ttf", assembly, baseFontSize);
		ImGui.GetIO().NativePtr->FontDefault = robotoFont.NativePtr;

		// Load FontAwesome icon font
		(ushort, ushort) fontAwesomeRange = (FontAwesome6.IconMin, FontAwesome6.IconMax16);
		ImFontPtr first = fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAR, assembly, iconFontSize, fontAwesomeRange);
		ImFontPtr second = fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAS, assembly, iconFontSize, fontAwesomeRange);

		BigFont = fonts.LoadFontFromResources(assetsPath + ".Roboto.Roboto-Regular.ttf", assembly, baseFontSize * 2);

		fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAR, assembly, iconFontSize * 1.5f, fontAwesomeRange);
        fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAS, assembly, iconFontSize * 1.5f, fontAwesomeRange);

        fonts.Build();
	}

    protected override void OnStart()
    {
		ShowScreen(new MainmenuScreen());
    }

	public void StopGame()
	{
		Running = false;
	}

	public void ShowScreen(IScreen screen)
	{
		CurrentScreen?.Hide();

		CurrentScreen = screen;
		CurrentScreen.Show();
	}

    protected override void OnRenderImGui()
    {
		CurrentScreen.RenderImGui();
    }
}
