﻿using System.Numerics;
using ImGuiNET;

using SysColor = System.Drawing.Color;
using System.Reflection;

namespace Leapster;

public class Game : Application
{
	public static Game Instance { get; private set; }

	public event Action OnRender = delegate { };

	public Player Player { get; private set; }

	public Level CurrentLevel { get; private set; }
	public List<Level> AvailableLevels { get; private set; } = Levels.AllLevels;

	public bool InMainMenu
	{
		get => inMainMenu;
		private set
		{
			inMainMenu = value;
			clearColor = inMainMenu ? mainMenuClearColor : defaultClearColor;
		}
	}

	private bool inMainMenu;

	public ImFontPtr BigFont { get; private set; }

	private bool inOptionsWindow = false;

	private readonly SysColor defaultClearColor = SysColor.FromArgb(255, 115, 140, 153);
	private readonly SysColor mainMenuClearColor = SysColor.FromArgb(240, 40, 15, 15);

    private readonly int[] resolutionInput = [0, 0];

    public Game()
	{
		if (Instance != null)
		{
			throw new Exception("not allowed, game already created");
		}

		Instance = this;
	}

	protected override unsafe void InitRenderer()
	{
		base.InitRenderer();

		sdl.GetWindowSize(window, ref resolutionInput[0], ref resolutionInput[1]);
	}

	protected override unsafe void InitImGui()
	{
		base.InitImGui();

		string assetsPath = typeof(Game).Namespace + ".Assets";
		Assembly assembly = Assembly.GetExecutingAssembly();

		ImFontAtlasPtr fonts = ImGui.GetIO().Fonts;

		float baseFontSize = 15f;
		float iconFontSize = baseFontSize / 3 * 3.5f;

		// load small and big version of font
		ImFontPtr robotoFont = fonts.LoadFontFromResources(assetsPath + ".Roboto.Roboto-Regular.ttf", assembly, baseFontSize);
		BigFont = fonts.LoadFontFromResources(assetsPath + ".Roboto.Roboto-Regular.ttf", assembly, baseFontSize * 2);
		ImGui.GetIO().NativePtr->FontDefault = robotoFont.NativePtr;

		// Load FontAwesome icon font
		(ushort, ushort) fontAwesomeRange = (FontAwesome6.IconMin, FontAwesome6.IconMax16);
		ImFontPtr first = fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAR, assembly, iconFontSize, fontAwesomeRange);
		ImFontPtr second = fonts.LoadIconFontFromResources(assetsPath + ".FontAwesome." + FontAwesome6.FontIconFileNameFAS, assembly, iconFontSize, fontAwesomeRange);

		fonts.Build();
	}

    protected override void OnStart()
    {
		Player = new();

		LoadLevel(0);

        InMainMenu = true;
    }

	public void StopGame()
	{
		Running = false;
	}

	public void LoadLevel(int index)
	{
		LoadLevel(AvailableLevels[index]);
	}

	private void LoadLevel(Level level)
	{
		CurrentLevel?.OnUnload();

		CurrentLevel = level;
		CurrentLevel.OnLoad();

		Player.position = CurrentLevel.PlayerSpawn;
		Player.Velocity = Vector2.Zero;
	}

    protected override void OnRenderImGui()
    {
        if (InMainMenu)
        {
            RenderMainMenu();
			return;
        }
        OnRender();
    }

    private Vector2 childSize = Vector2.Zero;

	private void RenderMainMenu()
	{
		ImGui.Begin("test", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs);

		Vector2 parentSize = ImGui.GetWindowSize();

		ImGui.SetNextWindowPos((parentSize - childSize) / 2);

		ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10);
		ImGui.PushFont(BigFont);

		if (ImGui.BeginChild("mainMenu", Vector2.Zero, ImGuiChildFlags.AutoResizeX | ImGuiChildFlags.AutoResizeY))
		{
			Vector2 buttonSize = new Vector2(200, 100);

			Vector2 cursorPos = ImGui.GetCursorPos();

			ImGui.SetCursorPosX((childSize.X - ImGui.CalcTextSize("LEAPSTER").X) / 2f);
			ImGui.Text("LEAPSTER");

			ImGui.SetCursorPosX(cursorPos.X);

            ImGui.Dummy(new Vector2(0.0f, 100.0f));

            if (ImGui.Button("Start", buttonSize))
			{
				InMainMenu = false;
			}

			if (ImGui.Button("Options", buttonSize))
			{
				inOptionsWindow = !inOptionsWindow;
			}

			childSize = ImGui.GetWindowSize();

			ImGui.EndChild();
		}

		ImGui.PopFont();

		ImGui.PopStyleVar();

		ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
		ImGui.SetWindowPos(Vector2.Zero);

		ImGui.End();

		if (inOptionsWindow)
		{
			RenderOptionsMenu();
		}
	}

	private void RenderOptionsMenu()
	{
		if (!ImGui.Begin("Options", ref inOptionsWindow))
		{
			ImGui.End();
			return;
        }

		ImGui.InputInt2("Resolution", ref resolutionInput[0]);

		if (ImGui.Button("Apply"))
		{
			unsafe
			{
				sdl.SetWindowSize(window, resolutionInput[0], resolutionInput[1]);
			}
		}

        ImGui.End();
	}
}
