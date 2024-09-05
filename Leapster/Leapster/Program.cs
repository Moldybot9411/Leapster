﻿using ImGuiNET;
using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Leapster;

class Program
{
    private static Sdl2Window _window;
    private static GraphicsDevice _gd;
    private static CommandList _cl;
    private static ImGuiController _controller;
    // private static MemoryEditor _memoryEditor;

    // UI state
    private static float _f = 0.0f;
    private static int _counter = 0;
    private static int _dragInt = 0;
    private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);
    private static bool _showImGuiDemoWindow = true;
    private static bool _showAnotherWindow = false;
    private static bool _showMemoryEditor = false;
    private static byte[] _memoryEditorData;
    private static uint s_tab_bar_flags = (uint)ImGuiTabBarFlags.Reorderable;
    static bool[] s_opened = { true, true, true, true }; // Persistent user state

    static void SetThing(out float i, float val) { i = val; }

    static void Main(string[] args)
    {
        // Create window, GraphicsDevice, and all resources necessary for the demo.
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out _window,
            out _gd);
        _window.Resized += () =>
        {
            _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
            _controller.WindowResized(_window.Width, _window.Height);
        };
        _cl = _gd.ResourceFactory.CreateCommandList();
        _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);
        // _memoryEditor = new MemoryEditor();
        Random random = new Random();
        _memoryEditorData = Enumerable.Range(0, 1024).Select(i => (byte)random.Next(255)).ToArray();

        var stopwatch = Stopwatch.StartNew();
        float deltaTime = 0f;
        // Main application loop
        while (_window.Exists)
        {
            deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            InputSnapshot snapshot = _window.PumpEvents();
            if (!_window.Exists) { break; }
            _controller.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            SubmitUI();

            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
            _controller.Render(_gd, _cl);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_gd.MainSwapchain);
        }

        // Clean up Veldrid resources
        _gd.WaitForIdle();
        _controller.Dispose();
        _cl.Dispose();
        _gd.Dispose();
    }

    private static void SubmitUI()
    {
        ImGui.ShowDemoWindow();

        ImDrawListPtr drawList = ImGui.GetBackgroundDrawList();

        drawList.AddCircle(new Vector2(100, 100), 100, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 1, 1)));
    }
}