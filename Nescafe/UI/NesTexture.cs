using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using StbImageSharp;
using System.Drawing;

namespace Nescafe.UI;

public class NesTexture : IDisposable
{
	private uint _handle;
	private GL _gl;

	private readonly Color[] _palette = new[]
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

	public unsafe NesTexture(GL gl, Span<byte> data, uint width, uint height)
	{
		//Saving the gl instance.
		_gl = gl;

		////Generating the opengl handle;
		_handle = _gl.GenTexture();
		Bind();

		

		byte[] textureData = new byte[width * height * 4]; // RGB format

		for (int i = 0; i < data.Length; i++)
		{
			int colorIndex = data[i];
			var color = _palette[colorIndex];
			textureData[i * 3] = 1;//color.R;
			textureData[i * 3 + 1] = 255;//color.G;
			textureData[i * 3 + 2] = 1;//color.B;
			textureData[i * 3 + 3] = 255;
		}

		//uint tex;
		//gl.GenTextures(1, out tex);
		//gl.BindTexture(TextureTarget.Texture2D, tex);

			fixed (byte* data2 = textureData)
			{
				gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data2);
			}
			SetParameters();

		//gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		//gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);


		////Saving the gl instance.
		//_gl = gl;

		////Generating the opengl handle;
		//_handle = _gl.GenTexture();
		//Bind();
		////We want the ability to create a texture using data generated from code aswell.
		//fixed (void* d = &data[0])
		//{
		//	//Setting the data of a texture.
		//	_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.RgbInteger, PixelType.UnsignedByte, d);
		//	SetParameters();
		//}
	}

	private void SetParameters()
	{
		//Setting some texture perameters so the texture behaves as expected.
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

		//Generating mipmaps.
		//_gl.GenerateMipmap(TextureTarget.Texture2D);
	}

	public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
	{
		//When we bind a texture we can choose which textureslot we can bind it to.
		_gl.ActiveTexture(textureSlot);
		_gl.BindTexture(TextureTarget.Texture2D, _handle);
	}

	public void Dispose()
	{
		//In order to dispose we need to delete the opengl handle for the texure.
		_gl.DeleteTexture(_handle);
	}
}