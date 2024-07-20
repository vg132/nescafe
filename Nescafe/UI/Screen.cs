//using Silk.NET.Maths;
//using Silk.NET.Windowing;
//using Silk.NET.OpenGL;
//using Silk.NET.Input;
//using Nescafe.Core;
//using Silk.NET.GLFW;

//namespace Nescafe.UI;

//public class Screen
//{
//	public Core.Console Console { get; set; }

//	private bool screenDataUpdated = false;
//	private byte[] screenData;
//	private readonly object _drawLock = new object();

//	private IWindow _window;
//	private GL _gl;

//	private BufferObject<float> Vbo;
//	private BufferObject<uint> Ebo;
//	private VertexArrayObject<float, uint> Vao;
//	private Texture _screenTexture;
//	private static Shader Shader;

//	private static readonly float[] Vertices =
//	{
//		//X    Y      Z     S    T
//		1.0f,  1.0f, 0.0f, 1.0f, 0.0f,
//		1.0f, -1.0f, 0.0f, 1.0f, 1.0f,
//		-1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
//		-1.0f,  1.0f, 0.5f, 0.0f, 0.0f
//	};

//	private static readonly uint[] Indices =
//	{
//		0, 1, 3,
//		1, 2, 3
//	};

//	public void Start()
//	{
//		var options = WindowOptions.Default;
//		options.Size = new Vector2D<int>(512, 480);
//		options.Title = "Nescafe";
//		options.FramesPerSecond = 120;

//		_window = Window.Create(options);
//		_window.Load += OnLoad;
//		_window.Render += OnRender;
//		_window.Closing += OnClose;
//		_window.IsVisible = false;
//		_window.WindowBorder = WindowBorder.Resizable;
//		_window.Run();

//		_window.Dispose();
//	}

//	public nint Handle { get; set; }

//	public bool IsVisible
//	{
//		get => _window.IsVisible;
//		set => _window.IsVisible = value;
//	}

//	private void OnClose()
//	{
//		//Remember to dispose all the instances.
//		Vbo.Dispose();
//		Ebo.Dispose();
//		Vao.Dispose();
//		Shader.Dispose();
//		_screenTexture?.Dispose();
//	}

//	private void KeyDown(IKeyboard arg1, Key key, int arg3)
//	{
//		//if (key == Key.Escape)
//		//{
//		//	_window.Close();
//		//}
//		SetControllerButton(true, key);
//	}

//	private void KeyUp(IKeyboard keyboard, Key key, int arg3)
//	{
//		SetControllerButton(false, key);
//	}

//	private void SetControllerButton(bool state, Key key)
//	{
//		switch (key)
//		{
//			case Key.Z:
//				Console.Controller.setButtonState(Controller.Button.A, state);
//				break;
//			case Key.X:
//				Console.Controller.setButtonState(Controller.Button.B, state);
//				break;
//			case Key.Left:
//				Console.Controller.setButtonState(Controller.Button.Left, state);
//				break;
//			case Key.Right:
//				Console.Controller.setButtonState(Controller.Button.Right, state);
//				break;
//			case Key.Up:
//				Console.Controller.setButtonState(Controller.Button.Up, state);
//				break;
//			case Key.Down:
//				Console.Controller.setButtonState(Controller.Button.Down, state);
//				break;
//			case Key.Q:
//			case Key.Enter:
//				Console.Controller.setButtonState(Controller.Button.Start, state);
//				break;
//			case Key.W:
//			case Key.Escape:
//				Console.Controller.setButtonState(Controller.Button.Select, state);
//				break;
//		}
//	}

//	private unsafe void OnLoad()
//	{
//		SetupControls();

//		_gl = GL.GetApi(_window);


//		//IntPtr nativeHandle = _gl.   //GetWin32Window((Silk.NET.GLFW.WindowHandle*)window.Handle);

//		var glfw = Glfw.GetApi();
//		Handle = new GlfwNativeWindow(glfw, (WindowHandle*)_window.Handle).Win32.Value.Hwnd;

//		//Instantiating our new abstractions
//		Ebo = new BufferObject<uint>(_gl, Indices, BufferTargetARB.ElementArrayBuffer);
//		Vbo = new BufferObject<float>(_gl, Vertices, BufferTargetARB.ArrayBuffer);
//		Vao = new VertexArrayObject<float, uint>(_gl, Vbo, Ebo);

//		//Telling the VAO object how to lay out the attribute pointers
//		Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
//		Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

//		Shader = new Shader(_gl, @"D:\Projects\nescafe\Nescafe\UI\shader.vert", @"D:\Projects\nescafe\Nescafe\UI\shader.frag");
//	}

//	private void SetupControls()
//	{
//		IInputContext input = _window.CreateInput();
//		for (int i = 0; i < input.Keyboards.Count; i++)
//		{
//			input.Keyboards[i].KeyDown += KeyDown;
//			input.Keyboards[i].KeyUp += KeyUp;
//		}
//	}

//	public unsafe void UpdateScreen(byte[] screen)
//	{
//		lock (_drawLock)
//		{
//			screenData = screen;
//			screenDataUpdated = true;
//		}
//	}

//	private unsafe void OnRender(double obj)
//	{
//		lock (_drawLock)
//		{
//			//System.Diagnostics.Debug.WriteLineIf(!screenDataUpdated, "No new screen on render call");
//			if (screenDataUpdated)
//			{
//				byte[] textureData = new byte[256 * 240 * 3]; // RGB format
//				for (int i = 0; i < screenData.Length; i++)
//				{
//					int colorIndex = screenData[i];
//					var color = Palette.GetColor(colorIndex);
//					textureData[i * 3] = color.R;
//					textureData[i * 3 + 1] = color.G;
//					textureData[i * 3 + 2] = color.B;
//				}
//				if (_screenTexture != null)
//				{
//					_screenTexture.Dispose();
//				}
//				_screenTexture = new Texture(_gl, textureData, 256, 240);
//				screenDataUpdated = false;
//			}
//		}
//		_gl.Clear((uint)ClearBufferMask.ColorBufferBit);

//		if (_screenTexture != null)
//		{
//			//Binding and using our VAO and shader.
//			Vao.Bind();
//			Shader.Use();
//			_screenTexture.Bind(TextureUnit.Texture0);
//			Shader.SetUniform("uTexture", 0);
//			_gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
//		}
//	}
//}