using OpenTK.WinForms;
using OpenTK.Graphics.OpenGL4;
using Nescafe.UI.Shaders;
using Nescafe.Core;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Nescafe.UI;

public partial class Renderer
{
	private readonly GLControl _control;

	private System.Windows.Forms.Timer _timer;
	private readonly object _drawLock = new object();
	private byte[] screenData;
	private bool screenDataUpdated = false;
	private int _elementBufferObject;
	private int _vertexBufferObject;
	private int _vertexArrayObject;
	private Shader _shader;
	//private Shader _scanlineShader;
	private Texture _texture;

	private readonly float[] _vertices =
	{
		// Position         Texture coordinates
		1.0f,  1.0f, 0.0f, 1.0f, 0.0f, // top right
		1.0f, -1.0f, 0.0f, 1.0f, 1.0f, // bottom right
		-1.0f, -1.0f, 0.0f, 0.0f, 1.0f, // bottom left
		-1.0f,  1.0f, 0.0f, 0.0f, 0.0f  // top left
	};

	private readonly uint[] _indices =
	{
		0, 1, 3,
		1, 2, 3
	};


	public Renderer(GLControl control)
	{
		_control = control;
		Setup();
	}

	private void Setup()
	{
		_control.Resize += glControl_Resize;
		_control.Paint += glControl_Paint;

		SetupViewport();
		SetupShader();
		//SetupScanlineShader();
		SetupRenderLoop();
	}

	//private void SetupScanlineShader()
	//{
	//	// The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
	//	var fragCode = ReadEmbeddedFile("UI/Shaders/Scanline/scanline.frag");
	//	var vertCode = ReadEmbeddedFile("UI/Shaders/Scanline/scanline.vert");

	//	_scanlineShader = new Shader(vertCode, fragCode);
	//	_scanlineShader.Use();

	//	int vertexColorLocation = GL.GetUniformLocation(_scanlineShader.Handle, "u_screenHeight");
	//	GL.Uniform1(vertexColorLocation, _control.Height);
	//}

	private void SetupRenderLoop()
	{
		// Redraw the screen every 1/120 of a second.
		_timer = new System.Windows.Forms.Timer();
		_timer.Tick += (sender, e) =>
		{
			Render();
		};
		_timer.Interval = 1000 / 120;   // 1000 ms per sec / 120 fps = 8 MS
		_timer.Start();
	}

	private void SetupViewport()
	{
		// Ensure that the viewport and projection matrix are set correctly initially.
		glControl_Resize(_control, EventArgs.Empty);

		_vertexArrayObject = GL.GenVertexArray();
		GL.BindVertexArray(_vertexArrayObject);

		_vertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

		_elementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
	}

	private void SetupShader()
	{
		// The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
		var fragCode = ReadEmbeddedFile("UI/Shaders/shader.frag");
		var vertCode = ReadEmbeddedFile("UI/Shaders/shader.vert");

		_shader = new Shader(vertCode, fragCode);
		_shader.Use();

		// Because there's now 5 floats between the start of the first vertex and the start of the second,
		// we modify the stride from 3 * sizeof(float) to 5 * sizeof(float).
		// This will now pass the new vertex array to the buffer.
		var vertexLocation = _shader.GetAttribLocation("aPosition");
		GL.EnableVertexAttribArray(vertexLocation);
		GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

		// Next, we also setup texture coordinates. It works in much the same way.
		// We add an offset of 3, since the texture coordinates comes after the position data.
		// We also change the amount of data to 2 because there's only 2 floats for texture coordinates.
		var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
		GL.EnableVertexAttribArray(texCoordLocation);
		GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
	}

	private static string ReadEmbeddedFile(string path)
	{
		var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
		using var reader = embeddedProvider.GetFileInfo(path).CreateReadStream();
		using var sr = new StreamReader(reader);
		return sr.ReadToEnd();
	}

	private void glControl_Resize(object sender, EventArgs e)
	{
		_control.MakeCurrent();
		if (_control.ClientSize.Height == 0)
		{
			_control.ClientSize = new Size(_control.ClientSize.Width, 1);
		}
		GL.Viewport(0, 0, _control.ClientSize.Width, _control.ClientSize.Height);
	}

	private void glControl_Paint(object sender, PaintEventArgs e)
	{
		Render();
	}

	private unsafe void Render()
	{
		if (screenDataUpdated)
		{
			lock (_drawLock)
			{
				byte[] textureData = new byte[256 * 240 * 3];
				for (int i = 0; i < screenData.Length; i++)
				{
					int colorIndex = screenData[i];
					var color = Palette.GetColor(colorIndex);
					textureData[i * 3] = color.R;
					textureData[i * 3 + 1] = color.G;
					textureData[i * 3 + 2] = color.B;
				}
				if (_texture != null)
				{
					_texture.Dispose();
				}
				_texture = new Texture(textureData, 256, 240);
				screenDataUpdated = false;
			}
			if (_texture != null)
			{
				_control.MakeCurrent();
				GL.Clear(ClearBufferMask.ColorBufferBit);
				_shader.Use();
				//_scanlineShader.Use();
				GL.BindVertexArray(_vertexArrayObject);
				_texture.Bind(TextureUnit.Texture0);
				GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

				_control.SwapBuffers();
			}
		}
	}

	public unsafe void UpdateScreen(byte[] screen)
	{
		lock (_drawLock)
		{
			if (screenData == null)
			{
				screenData = new byte[screen.Length];
			}
			Array.Copy(screen, screenData, screen.Length);
			screenDataUpdated = true;
		}
	}
}