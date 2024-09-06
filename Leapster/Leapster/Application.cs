using ImGuiNET;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

using SysColor = System.Drawing.Color;

namespace Leapster;

public class Application
{
    public bool Running { get; protected set; } = true;

    protected SysColor clearColor = SysColor.FromArgb(255, 115, 140, 153);

    protected Sdl sdl;
    protected unsafe Window* window;
    protected SdlContext sdlContext;

    protected IntPtr glContext;
    protected GL gl;

    protected unsafe virtual void InitRenderer()
    {
        sdl = SdlProvider.SDL.Value;

        WindowFlags windowFlags = WindowFlags.Opengl | WindowFlags.AllowHighdpi | WindowFlags.Shown;

        window = sdl.CreateWindow("Leapster", 50, 50, 1280, 720, (uint)windowFlags);

        // Create context for rendering		
        sdlContext = new SdlContext(sdl, window);
        sdlContext.Create();
        glContext = sdlContext.Handle;

        sdlContext.MakeCurrent();
        sdl.GLSetSwapInterval(1); // -1 is vsync

        gl = GL.GetApi(sdlContext);

        InitImGui();
    }

    protected unsafe virtual void InitImGui()
    {
        ImGui.CreateContext();
        ImGui.ImGui_ImplSDL2_InitForOpenGL(new IntPtr(window), glContext);
        ImGui.ImGui_ImplOpenGL3_Init("#version 130");
    }

    protected virtual void OnStart() { }

    public unsafe void Start()
    {
        InitRenderer();

        OnStart();

        while (Running)
        {
            RenderLoop();
        }

        gl.Dispose();
        sdlContext.Dispose();

        sdl.DestroyWindow(window);
        sdl.Quit();
    }

    public void Stop()
    {
        Running = false;
    }

    protected virtual void OnRenderImGui() { }

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

        OnRenderImGui();

        unsafe
        {
            int windowWidth = 0;
            int windowHeight = 0;

            sdl.GetWindowSize(window, ref windowWidth, ref windowHeight);

            ImGui.Render();
            gl.Viewport(0, 0, (uint)windowWidth, (uint)windowHeight);

            gl.ClearColor(clearColor);

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

}
