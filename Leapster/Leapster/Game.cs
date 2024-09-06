using System.Numerics;
using ImGuiNET;
using Silk.NET.SDL;
using Silk.NET.OpenGL;

using SysColor = System.Drawing.Color;
using System.Reflection;

namespace Leapster;

public class Game
{
	public bool Running { get; private set; } = true;

	public event Action OnRender = delegate { };

	public static Game Instance { get; private set; }
	public Level CurrentLevel { get; private set; }

	public bool InMainMenu { get; private set; } = true;

	public ImFontPtr BigFont { get; private set; }

	private bool inOptionsWindow = false;

	private readonly SysColor clearColor = SysColor.FromArgb(255, 115, 140, 153);
	private readonly SysColor mainMenuClearColor = SysColor.FromArgb(240, 40, 15, 15);

	private Sdl sdl;
	private unsafe Window* window;
	private SdlContext sdlContext;

	private IntPtr glContext;
	private GL gl;

    private readonly int[] resolutionInput = [0, 0];

    public Game()
	{
		if (Instance != null)
		{
			throw new Exception("not allowed, game already created");
		}

		Instance = this;
	}

	private unsafe void InitRenderer()
	{
		sdl = SdlProvider.SDL.Value;

		WindowFlags windowFlags = WindowFlags.Opengl | WindowFlags.AllowHighdpi | WindowFlags.Shown;

		window = sdl.CreateWindow("Leapster", 50, 50, 1280, 720, (uint)windowFlags);

		// Create context for rendering		
		sdlContext = new SdlContext(sdl, window);
		sdlContext.Create();
		glContext = sdlContext.Handle;

		sdlContext.MakeCurrent();
		sdl.GLSetSwapInterval(-1); // -1 is vsync

		gl = GL.GetApi(sdlContext);

		InitImGui();

		sdl.GetWindowSize(window, ref resolutionInput[0], ref resolutionInput[1]);
	}

	private unsafe void InitImGui()
	{
		ImGui.CreateContext();

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

		ImGui.ImGui_ImplSDL2_InitForOpenGL(new IntPtr(window), glContext);
		ImGui.ImGui_ImplOpenGL3_Init("#version 130");
	}

	public unsafe void StartGame()
	{
		InitRenderer();

		Player player = new();
		CurrentLevel = new Level();

		while(Running)
		{
			RenderLoop();
		}

		gl.Dispose();
		sdlContext.Dispose();

		sdl.DestroyWindow(window);
		sdl.Quit();
	}

	public void StopGame()
	{
		Running = false;
	}

	private void RenderLoop()
	{

		Event sdlEvent = new();
		while (sdl.PollEvent(ref sdlEvent) != 0)
		{
			unsafe
			{
				ImGui.ImGui_ImplSDL2_ProcessEvent(new IntPtr(&sdlEvent));
			}

			EventType eventType = (EventType)sdlEvent.Type;

			if (eventType == EventType.Quit)
				Running = false;

			if (eventType == EventType.Windowevent) unsafe
			{
				if ((WindowEventID)sdlEvent.Window.Event == WindowEventID.Close && sdlEvent.Window.WindowID == sdl.GetWindowID(window))
				{
					Running = false;
				}
			}
		}

		ImGui.ImGui_ImplSDL2_NewFrame();
		ImGui.ImGui_ImplOpenGL3_NewFrame();
		ImGui.NewFrame();

		// Render GUI here
		if (InMainMenu)
		{
			RenderMainMenu();
		}
		else
		{
			OnRender();
		}

		unsafe
		{
			int windowWidth = 0;
			int windowHeight = 0;

			sdl.GetWindowSize(window, ref windowWidth, ref windowHeight);

			ImGui.Render();
			gl.Viewport(0, 0, (uint)windowWidth, (uint)windowHeight);

			gl.ClearColor(InMainMenu ? mainMenuClearColor : clearColor);
			
			gl.Clear(ClearBufferMask.ColorBufferBit);
			ImGui.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());

			if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
			{
				Window* backup_current_window = sdl.GLGetCurrentWindow();
				void* backup_current_context = sdl.GLGetCurrentContext();
				ImGui.UpdatePlatformWindows();
				ImGui.RenderPlatformWindowsDefault();
				sdl.GLMakeCurrent(backup_current_window, backup_current_context);
			}

			sdl.GLSwapWindow(window);
		}
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
