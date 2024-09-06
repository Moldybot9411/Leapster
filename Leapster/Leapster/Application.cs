﻿using ImGuiNET;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

using SysColor = System.Drawing.Color;

namespace Leapster;

public class Application
{
    public bool Running { get; protected set; } = true;

    public SysColor clearColor = SysColor.FromArgb(255, 115, 140, 153);

    public Sdl SdlInstance { get; protected set; }
    public unsafe Window* ApplicationWindow { get; protected set; }
    protected SdlContext sdlContext;

    protected IntPtr glContext;
    public GL Gl { get; protected set; }

    protected unsafe virtual void InitRenderer()
    {
        SdlInstance = SdlProvider.SDL.Value;

        WindowFlags windowFlags = WindowFlags.Opengl | WindowFlags.AllowHighdpi | WindowFlags.Shown;

        ApplicationWindow = SdlInstance.CreateWindow("Leapster", 50, 50, 1280, 720, (uint)windowFlags);

        // Create context for rendering		
        sdlContext = new SdlContext(SdlInstance, ApplicationWindow);
        sdlContext.Create();
        glContext = sdlContext.Handle;

        sdlContext.MakeCurrent();
        SdlInstance.GLSetSwapInterval(1); // -1 is vsync

        Gl = GL.GetApi(sdlContext);

        InitImGui();
    }

    protected unsafe virtual void InitImGui()
    {
        ImGui.CreateContext();
        ImGui.ImGui_ImplSDL2_InitForOpenGL(new IntPtr(ApplicationWindow), glContext);
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

        Gl.Dispose();
        sdlContext.Dispose();

        SdlInstance.DestroyWindow(ApplicationWindow);
        SdlInstance.Quit();
    }

    public void Stop()
    {
        Running = false;
    }

    protected virtual void OnRenderImGui() { }

    private void RenderLoop()
    {
        Event sdlEvent = new();
        while (SdlInstance.PollEvent(ref sdlEvent) != 0)
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
                if ((WindowEventID)sdlEvent.Window.Event == WindowEventID.Close && sdlEvent.Window.WindowID == SdlInstance.GetWindowID(ApplicationWindow))
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

            SdlInstance.GetWindowSize(ApplicationWindow, ref windowWidth, ref windowHeight);

            ImGui.Render();
            Gl.Viewport(0, 0, (uint)windowWidth, (uint)windowHeight);

            Gl.ClearColor(clearColor);

            Gl.Clear(ClearBufferMask.ColorBufferBit);
            ImGui.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());

            if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
            {
                Window* backup_current_window = SdlInstance.GLGetCurrentWindow();
                void* backup_current_context = SdlInstance.GLGetCurrentContext();
                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();
                SdlInstance.GLMakeCurrent(backup_current_window, backup_current_context);
            }

            SdlInstance.GLSwapWindow(ApplicationWindow);
        }
    }

}
