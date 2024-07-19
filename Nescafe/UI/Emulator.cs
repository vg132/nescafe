using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Nescafe.Core;

namespace Nescafe.UI;

public class Emulator
{
	private Thread _nesThread;
	private Core.Console _console;

	private bool screenDataUpdated = false;
	private byte[] screenData;
	private readonly object _drawLock = new object();

	private IWindow _window;
	private GL _gl;

	private BufferObject<float> Vbo;
	private BufferObject<uint> Ebo;
	private VertexArrayObject<float, uint> Vao;
	private Texture _screenTexture;
	private static Shader Shader;

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
		new Emulator().Start();
	}

	public void Start()
	{
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(512, 480);
		options.Title = "Nescafe";
		options.FramesPerSecond = 120;
		_window = Window.Create(options);

		_window.Load += OnLoad;
		_window.Render += OnRender;
		_window.Closing += OnClose;
		_window.Run();

		_window.Dispose();
	}

	private void OnClose()
	{
		//Remember to dispose all the instances.
		Vbo.Dispose();
		Ebo.Dispose();
		Vao.Dispose();
		Shader.Dispose();
		_screenTexture?.Dispose();
	}

	private void KeyDown(IKeyboard arg1, Key key, int arg3)
	{
		//if (key == Key.Escape)
		//{
		//	_window.Close();
		//}
		SetControllerButton(true, key);
	}

	private void KeyUp(IKeyboard keyboard, Key key, int arg3)
	{
		SetControllerButton(false, key);
	}

	private void SetControllerButton(bool state, Key key)
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

	private void OnLoad()
	{
		SetupControls();

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

	private void SetupControls()
	{
		IInputContext input = _window.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++)
		{
			input.Keyboards[i].KeyDown += KeyDown;
			input.Keyboards[i].KeyUp += KeyUp;
		}
	}

	unsafe void Draw(byte[] screen)
	{
		lock (_drawLock)
		{
			screenData = screen;
			screenDataUpdated = true;
		}
	}

	private void StartConsole()
	{
		_nesThread = new Thread(new ThreadStart(StartNes));
		_nesThread.IsBackground = true;
		_nesThread.Start();
	}

	private void StartNes()
	{
		_console.Start();
	}

	private unsafe void OnRender(double obj)
	{
		lock (_drawLock)
		{
			//System.Diagnostics.Debug.WriteLineIf(!screenDataUpdated, "No new screen on render call");
			if (screenDataUpdated)
			{
				byte[] textureData = new byte[256 * 240 * 3]; // RGB format
				for (int i = 0; i < screenData.Length; i++)
				{
					int colorIndex = screenData[i];
					var color = Palette.GetColor(colorIndex);
					textureData[i * 3] = color.R;
					textureData[i * 3 + 1] = color.G;
					textureData[i * 3 + 2] = color.B;
				}
				if (_screenTexture != null)
				{
					_screenTexture.Dispose();
				}
				_screenTexture = new Texture(_gl, textureData, 256, 240);
				screenDataUpdated = false;
			}
		}
		_gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		if (_screenTexture != null)
		{
			//Binding and using our VAO and shader.
			Vao.Bind();
			Shader.Use();
			_screenTexture.Bind(TextureUnit.Texture0);
			Shader.SetUniform("uTexture", 0);
			_gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
		}
	}
}