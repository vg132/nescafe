using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using System.Windows.Forms;


namespace Nescafe.Core
{
	class Emulator
	{
		[STAThread]
		public static void Main()
		{
			Application.Run(new Ui());
		}
	}
	
	public class SilkEmulator
	{
		private static IWindow _window;

		[STAThread]
		public static void Main()
		{
			WindowOptions options = new WindowOptions
			{
				Size = new Vector2D<int>(800, 600),
				Title = "My first Silk.NET application!"
			};
			_window = Window.Create(options);
			_window.Run();
		}
	}

	//class SilkEmulator
	//{
	//	private static IWindow window;
	//	private static GL gl;
	//	private static uint texture;
	//	private static uint vertexArray;
	//	private static uint vertexBuffer;
	//	private static uint indexBuffer;
	//	private static Shader shader;

	//	private static float[] vertices = {
 //       // positions      // texture coords
 //       0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
 //       0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
 //      -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
 //      -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
 //   };

	//	private static uint[] indices = {
	//			0, 1, 3,
	//			1, 2, 3
	//	};

	//	static void Main(string[] args)
	//	{
	//		var options = WindowOptions.Default;
	//		options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
	//		options.Title = "Silk.NET OpenGL Example";

	//		window = Window.Create(options);
	//		window.Load += OnLoad;
	//		window.Render += OnRender;
	//		window.Run();
	//	}

	//	private static void OnLoad()
	//	{
	//		gl = GL.GetApi(window);

	//		// Load the image
	//		using (var image = Image.Load<Rgba32>("path/to/your/image.png"))
	//		{
	//			var pixels = new byte[image.Width * image.Height * 4];
	//			image.CopyPixelDataTo(pixels);

	//			// Generate texture
	//			texture = gl.GenTexture();
	//			gl.BindTexture(TextureTarget.Texture2D, texture);
	//			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
	//			gl.GenerateMipmap(TextureTarget.Texture2D);

	//			// Set texture parameters
	//			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
	//			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
	//			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
	//			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
	//		}

	//		// Create vertex array, buffer, and index buffer
	//		vertexArray = gl.GenVertexArray();
	//		gl.BindVertexArray(vertexArray);

	//		vertexBuffer = gl.GenBuffer();
	//		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vertexBuffer);
	//		gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), vertices, BufferUsageARB.StaticDraw);

	//		indexBuffer = gl.GenBuffer();
	//		gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexBuffer);
	//		gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), indices, BufferUsageARB.StaticDraw);

	//		// Load and compile shaders
	//		shader = new Shader(gl, vertexShaderSource, fragmentShaderSource);

	//		// Set vertex attribute pointers
	//		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
	//		gl.EnableVertexAttribArray(0);
	//		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
	//		gl.EnableVertexAttribArray(1);
	//	}

	//	private static void OnRender(double delta)
	//	{
	//		gl.Clear((uint)GLEnum.ColorBufferBit);

	//		// Bind texture
	//		gl.BindTexture(TextureTarget.Texture2D, texture);

	//		// Use shader program
	//		shader.Use();

	//		// Bind vertex array and draw
	//		gl.BindVertexArray(vertexArray);
	//		gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
	//	}

	//	private const string vertexShaderSource = @"
 //   #version 330 core
 //   layout (location = 0) in vec3 aPos;
 //   layout (location = 1) in vec2 aTexCoord;

 //   out vec2 TexCoord;

 //   void main()
 //   {
 //       gl_Position = vec4(aPos, 1.0);
 //       TexCoord = aTexCoord;
 //   }";

	//	private const string fragmentShaderSource = @"
 //   #version 330 core
 //   out vec4 FragColor;

 //   in vec2 TexCoord;

 //   uniform sampler2D texture1;

 //   void main()
 //   {
 //       FragColor = texture(texture1, TexCoord);
 //   }";
	//}

	//public class Shader
	//{
	//	private uint handle;
	//	private GL gl;

	//	public Shader(GL gl, string vertexPath, string fragmentPath)
	//	{
	//		this.gl = gl;
	//		uint vertex = CompileShader(vertexPath, GLEnum.VertexShader);
	//		uint fragment = CompileShader(fragmentPath, GLEnum.FragmentShader);

	//		handle = gl.CreateProgram();
	//		gl.AttachShader(handle, vertex);
	//		gl.AttachShader(handle, fragment);
	//		gl.LinkProgram(handle);
	//		gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
	//		if (status == 0)
	//		{
	//			throw new Exception($"Program linking failed: {gl.GetProgramInfoLog(handle)}");
	//		}
	//		gl.DeleteShader(vertex);
	//		gl.DeleteShader(fragment);
	//	}

	//	private uint CompileShader(string source, GLEnum type)
	//	{
	//		var shader = gl.CreateShader(type);
	//		gl.ShaderSource(shader, source);
	//		gl.CompileShader(shader);
	//		gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
	//		if (status == 0)
	//		{
	//			throw new Exception($"Shader compilation failed: {gl.GetShaderInfoLog(shader)}");
	//		}
	//		return shader;
	//	}

	//	public void Use()
	//	{
	//		gl.UseProgram(handle);
	//	}
	//}
}
