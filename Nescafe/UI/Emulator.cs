using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Drawing;
using Nescafe.Core;

namespace Nescafe.UI;

public class Emulator
{
	private static Thread _nesThread;
	private static Core.Console _console;

	private static readonly object _drawLock = new object();

	private static IWindow _window;
	private static GL _gl;

	private static BufferObject<float> Vbo;
	private static BufferObject<uint> Ebo;
	private static VertexArrayObject<float, uint> Vao;
	public static Texture Texture2;
	private static Shader Shader;

	private static uint _program;

	private static readonly float[] Vertices =
	{
		//X    Y      Z     S    T
		1.0f,  1.0f, 0.0f, 1.0f, 0.0f,
		1.0f, -1.0f, 0.0f, 1.0f, 1.0f,
		-1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
		-1.0f,  1.0f, 0.5f, 0.0f, 0.0f
	};

	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	private static void Main(string[] args)
	{
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(512, 480);
		options.Title = "Nescafe";
		options.FramesPerSecond = 60;
		_window = Window.Create(options);

		_window.Load += OnLoad;
		_window.Render += OnRender;
		_window.Closing += OnClose;
		_window.Run();

		_window.Dispose();
	}

	private static void OnClose()
	{
		//Remember to dispose all the instances.
		Vbo.Dispose();
		Ebo.Dispose();
		Vao.Dispose();
		Shader.Dispose();
		Texture2?.Dispose();
	}

	private static void KeyDown(IKeyboard arg1, Key key, int arg3)
	{
		//if (key == Key.Escape)
		//{
		//	_window.Close();
		//}
		SetControllerButton(true, key);
	}

	private static void KeyUp(IKeyboard keyboard, Key key, int arg3)
	{
		SetControllerButton(false, key);
	}

	private static void SetControllerButton(bool state, Key key)
	{
		switch (key)
		{
			case Key.Z:
				_console.Controller.setButtonState(Controller.Button.A, state);
				break;
			case Key.X:
				_console.Controller.setButtonState(Controller.Button.B, state);
				break;
			case Key.Left:
				_console.Controller.setButtonState(Controller.Button.Left, state);
				break;
			case Key.Right:
				_console.Controller.setButtonState(Controller.Button.Right, state);
				break;
			case Key.Up:
				_console.Controller.setButtonState(Controller.Button.Up, state);
				break;
			case Key.Down:
				_console.Controller.setButtonState(Controller.Button.Down, state);
				break;
			case Key.Q:
			case Key.Enter:
				_console.Controller.setButtonState(Controller.Button.Start, state);
				break;
			case Key.W:
			case Key.Escape:
				_console.Controller.setButtonState(Controller.Button.Select, state);
				break;
		}
	}

	private static void OnLoad()
	{
		IInputContext input = _window.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++)
		{
			input.Keyboards[i].KeyDown += KeyDown;
			input.Keyboards[i].KeyUp += KeyUp;
		}

		_gl = GL.GetApi(_window);

		//Instantiating our new abstractions
		Ebo = new BufferObject<uint>(_gl, Indices, BufferTargetARB.ElementArrayBuffer);
		Vbo = new BufferObject<float>(_gl, Vertices, BufferTargetARB.ArrayBuffer);
		Vao = new VertexArrayObject<float, uint>(_gl, Vbo, Ebo);

		//Telling the VAO object how to lay out the attribute pointers
		Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
		Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

		Shader = new Shader(_gl, @"D:\Projects\nescafe\Nescafe\UI\shader.vert", @"D:\Projects\nescafe\Nescafe\UI\shader.frag");

		_console = new Core.Console();
		_console.DrawAction = Draw;

		//_console.LoadCartridge(@"D:\Projects\nescafe\test_roms\blargg_nes_cpu_test5\official.nes");
		_console.LoadCartridge(@"D:\Projects\nescafe\roms\Super Mario Bros. (World).nes");
		StartConsole();
	}

	static int counter = 0;
	static byte[] screenData;

	private static readonly Color[] _palette = new[]
{
		Color.FromArgb(84, 84, 84), Color.FromArgb(0, 30, 116), Color.FromArgb(8, 16, 144), Color.FromArgb(48, 0, 136), Color.FromArgb(68, 0, 100), Color.FromArgb(92, 0, 48), Color.FromArgb(84, 4, 0), Color.FromArgb(60, 24, 0),
		Color.FromArgb(32, 42, 0), Color.FromArgb(8, 58, 0), Color.FromArgb(0, 64, 0), Color.FromArgb(0, 60, 0), Color.FromArgb(0, 50, 60), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
		Color.FromArgb(152, 150, 152), Color.FromArgb(8, 76, 196), Color.FromArgb(48, 50, 236), Color.FromArgb(92, 30, 228), Color.FromArgb(136, 20, 176), Color.FromArgb(160, 20, 100), Color.FromArgb(152, 34, 32), Color.FromArgb(120, 60, 0),
		Color.FromArgb(84, 90, 0), Color.FromArgb(40, 114, 0), Color.FromArgb(8, 124, 0), Color.FromArgb(0, 118, 40), Color.FromArgb(0, 102, 120), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
		Color.FromArgb(236, 238, 236), Color.FromArgb(76, 154, 236), Color.FromArgb(120, 124, 236), Color.FromArgb(176, 98, 236), Color.FromArgb(228, 84, 236), Color.FromArgb(236, 88, 180), Color.FromArgb(236, 106, 100), Color.FromArgb(212, 136, 32),
		Color.FromArgb(160, 170, 0), Color.FromArgb(116, 196, 0), Color.FromArgb(76, 208, 32), Color.FromArgb(56, 204, 108), Color.FromArgb(56, 180, 204), Color.FromArgb(60, 60, 60), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
		Color.FromArgb(236, 238, 236), Color.FromArgb(168, 204, 236), Color.FromArgb(188, 188, 236), Color.FromArgb(212, 178, 236), Color.FromArgb(236, 174, 236), Color.FromArgb(236, 174, 212), Color.FromArgb(236, 180, 176), Color.FromArgb(228, 196, 144),
		Color.FromArgb(204, 210, 120), Color.FromArgb(180, 222, 120), Color.FromArgb(168, 226, 144), Color.FromArgb(152, 226, 180), Color.FromArgb(160, 214, 228), Color.FromArgb(160, 162, 160), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0)
	};
	private static bool newScreen = false;
	unsafe static void Draw(byte[] screen)
	{
		counter++;
		lock (_drawLock)
		{
			screenData = screen;
			newScreen = true;
		}
	}

	private static void StartConsole()
	{
		_nesThread = new Thread(new ThreadStart(StartNes));
		_nesThread.IsBackground = true;
		_nesThread.Start();
	}

	private static void StartNes()
	{
		_console.Start();
	}

	private static unsafe void OnRender(double obj)
	{
		lock (_drawLock)
		{
			if (newScreen)
			{
				byte[] textureData = new byte[256 * 240 * 3]; // RGB format
				for (int i = 0; i < screenData.Length; i++)
				{
					int colorIndex = screenData[i];
					var color = _palette[colorIndex];
					textureData[i * 3] = color.R;
					textureData[i * 3 + 1] = color.G;
					textureData[i * 3 + 2] = color.B;
				}
				if (Texture2 != null)
				{
					Texture2.Dispose();
				}
				Texture2 = new Texture(_gl, textureData, 256, 240);
				newScreen = false;
			}
		}
		_gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		if (Texture2 != null)
		{
			//Binding and using our VAO and shader.
			Vao.Bind();
			Shader.Use();
			Texture2.Bind(TextureUnit.Texture0);
			Shader.SetUniform("uTexture", 0);
			_gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
		}
	}
}