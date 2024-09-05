using System.Diagnostics;
using Veldrid.StartupUtilities;
using Veldrid;
using System.Numerics;
using Veldrid.Sdl2;

namespace Leapster;

public class Game
{
    public float DeltaTime = 0f;

    public bool Running { get; private set; } = true;

    public event Action OnRender = delegate { };

    public static Game Instance { get; private set; }

    private Sdl2Window window;
    private GraphicsDevice graphicsDevice;
    private CommandList commands;
    private ImGuiController imguiController;

    private Vector3 clearColor = new Vector3(0.45f, 0.55f, 0.6f);

    private Stopwatch deltaTimeWatch;

    public Game()
    {
        if (Instance != null)
        {
            throw new Exception("not allowed, game already created");
        }

        Instance = this;
    }

    public void InitRenderer()
    {
        // Create window, GraphicsDevice, and all resources necessary for the demo.
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "Leapster"),
            new GraphicsDeviceOptions(true, null, false, ResourceBindingModel.Improved, true, true),
            out window,
            out graphicsDevice);

        window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
            imguiController.WindowResized(window.Width, window.Height);
        };

        commands = graphicsDevice.ResourceFactory.CreateCommandList();
        imguiController = new ImGuiController(graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);

        deltaTimeWatch = Stopwatch.StartNew();
    }

    public void StartGame()
    {
        InitRenderer();

        while(Running)
        {
            RenderLoop();
        }

        graphicsDevice.WaitForIdle();
        imguiController.Dispose();
        commands.Dispose();
        graphicsDevice.Dispose();
    }

    public void StopGame()
    {
        Running = false;
    }

    private void RenderLoop()
    {
        DeltaTime = deltaTimeWatch.ElapsedTicks / (float)Stopwatch.Frequency;
        deltaTimeWatch.Restart();
        InputSnapshot snapshot = window.PumpEvents();
        if (!window.Exists)
        {
            Running = false;
        }

        imguiController.Update(DeltaTime, snapshot);

        // Render GUI here
        OnRender();

        commands.Begin();
        commands.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
        commands.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, 1f));
        imguiController.Render(graphicsDevice, commands);
        commands.End();
        graphicsDevice.SubmitCommands(commands);
        graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
    }
}
