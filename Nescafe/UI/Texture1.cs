using OpenTK.Graphics.OpenGL4;

namespace Nescafe.UI;

public partial class Renderer
{
	public class Texture : IDisposable
	{
		private readonly int _handle;

		public unsafe Texture(byte[] data, int width, int height)
		{
			_handle = GL.GenTexture();
			Bind();
			fixed (void* p = &data[0])
			{
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, (nint)p);
				SetParameters();
			}
		}

		private void SetParameters()
		{
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
		}

		public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
		{
			GL.ActiveTexture(textureSlot);
			GL.BindTexture(TextureTarget.Texture2D, _handle);
		}

		public void Dispose()
		{
			GL.DeleteTexture(_handle);
		}
	}
}
