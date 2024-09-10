using ImGuiNET;
using Silk.NET.OpenGL;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Leapster;

public static class ImGuiExtensions
{

    public static Vector4 ToVector(this Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public static uint ToImguiColor(this Vector4 vector)
    {
        return ImGui.ColorConvertFloat4ToU32(vector);
    }

    public static uint ToImGuiColor(this Color color)
    {
        return color.ToVector().ToImguiColor();
    }

	private static IntPtr GetFontDataFromResources(string resourceName, Assembly assembly, out int fontDataLength)
	{
		using Stream fontStream = assembly.GetManifestResourceStream(resourceName);

		byte[] fontData = new byte[fontStream.Length];
		fontStream.Read(fontData, 0, (int)fontStream.Length);

		IntPtr fontPtr = ImGui.MemAlloc((uint)fontData.Length);
		Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

		fontDataLength = fontData.Length;

		return fontPtr;
	}

	public static unsafe ImFontPtr LoadFontFromResources(this ImFontAtlasPtr fontAtlas, string resourceName, Assembly assembly, float fontSize)
	{
		IntPtr fontPtr = GetFontDataFromResources(resourceName, assembly, out int fontDataLength);

		return fontAtlas.AddFontFromMemoryTTF(fontPtr, fontDataLength, fontSize);
	}

	public static unsafe ImFontPtr LoadFontFromResources(this ImFontAtlasPtr fontAtlas, string resourceName, Assembly assembly, float fontSize, ImFontConfigPtr fontConfig)
	{
		IntPtr fontPtr = GetFontDataFromResources(resourceName, assembly, out int fontDataLength);

		return fontAtlas.AddFontFromMemoryTTF(fontPtr, fontDataLength, fontSize, fontConfig);
	}

	public static unsafe ImFontPtr LoadFontFromResources(this ImFontAtlasPtr fontAtlas, string resourceName, Assembly assembly, float fontSize, ImFontConfigPtr fontConfig, IntPtr glyphRanges)
	{
		IntPtr fontPtr = GetFontDataFromResources(resourceName, assembly, out int fontDataLength);

		return fontAtlas.AddFontFromMemoryTTF(fontPtr, fontDataLength, fontSize, fontConfig, glyphRanges);
	}

	public static unsafe ImFontPtr LoadIconFontFromResources(this ImFontAtlasPtr fontAtlas, string resourceName, Assembly assembly, float size, (ushort, ushort) range)
	{
		ImFontConfigPtr configuration = ImGuiNative.ImFontConfig_ImFontConfig();

		configuration.MergeMode = true;
		configuration.PixelSnapH = true;
		configuration.GlyphOffset = new Vector2(0, 1);

		GCHandle rangeHandle = GCHandle.Alloc(new ushort[]
		{
			range.Item1,
			range.Item2,
			0
		}, GCHandleType.Pinned);

		try
		{
			return fontAtlas.LoadFontFromResources(resourceName, assembly, size, configuration, rangeHandle.AddrOfPinnedObject());
		}
		finally
		{
			configuration.Destroy();

			if (rangeHandle.IsAllocated)
			{
				rangeHandle.Free();
			}
		}
	}

    public static void HelpMarker(string description)
    {
        ImGui.TextDisabled(FontAwesome6.CircleQuestion);
        if (ImGui.BeginItemTooltip())
        {
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(description);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

	public static unsafe IntPtr LoadImage(string path, out Vector2 textureSize)
	{
        GL gl = Game.Instance.Gl;

        uint textureId = gl.GenTexture();

        using SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(path);

        textureSize = new Vector2(image.Width, image.Height);

        textureId = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, textureId);

        // Create an array to hold the pixel data
        byte[] pixels = new byte[4 * image.Width * image.Height];

        // Copy the pixel data to the array
        image.CopyPixelDataTo(pixels);

        fixed (byte* pixelPtr = pixels)
        {
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8,
                (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                pixelPtr);
        }

        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

		return (IntPtr)textureId;
    }
}
