using Nescafe.Core;
using System;

namespace Nescafe.UI.Debug.Controls;

public partial class Sprite : UserControl
{
	public Sprite()
	{
		InitializeComponent();
	}

	public Core.Console Console { get; set; }
	public SpriteInfo SpriteInfo { get; private set; }

	public void UpdateSprite(SpriteInfo spriteInfo)
	{
		SpriteInfo = spriteInfo;

		Invalidate();
	}

	private void Sprite_Paint(object sender, PaintEventArgs e)
	{
		var spriteWidth = 8;
		var spriteHeight = SpriteInfo.IsLarge ? 16 : 8;

		var pixleSize = SpriteInfo.IsLarge ? 2 : 4;
		var xOffset = Width / 2 - ((pixleSize * spriteWidth) / 2);
		var yOffset = Height / 2 - ((pixleSize * spriteHeight) / 2);

		Color[] pixelData = null;
		if (SpriteInfo.IsLarge)
		{
			var spriteData = Read8x16Sprite();
			pixelData = Convert8x16SpriteToBitmap(spriteData);
		}
		else
		{
			var spriteData = Read8x8Sprite();
			pixelData = Convert8x8SpriteToBitmap(spriteData);
		}

		var g = CreateGraphics();
		DrawControlBoder(g);
		for (int row = 0; row < spriteHeight; row++)
		{
			for (int col = 0; col < spriteWidth; col++)
			{
				using (var brush = new SolidBrush(pixelData[(row * spriteWidth) + col]))
				{
					var x = SpriteInfo.FlipHorizontal ? (8 - 1 - col) * pixleSize : col * pixleSize;
					var y = SpriteInfo.FlipVertical ? (8 - 1 - row) * pixleSize : row * pixleSize;
					g.FillRectangle(brush, x + xOffset, y + yOffset, pixleSize, pixleSize);
				}
			}
		}
	}

	private void DrawControlBoder(Graphics g)
	{
		g.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, Width, Height);

		g.DrawLine(new Pen(Color.DarkGray, 1.0f), 0, 0, Width, 0); // Top
		g.DrawLine(new Pen(Color.Black, 1.0f), 0, Height, Width, Height); // Bottom
		g.DrawLine(new Pen(Color.DarkGray, 1.0f), 0, 0, 0, Height); // Left
		g.DrawLine(new Pen(Color.Black, 1.0f), Width, 0, Width, Height); // Right
	}

	public Color[] Convert8x8SpriteToBitmap(byte[] spriteData)
	{
		Color[] bitmap = new Color[64];

		for (int y = 0; y < 8; y++)
		{
			byte plane0 = spriteData[y * 2];
			byte plane1 = spriteData[y * 2 + 1];

			for (int x = 0; x < 8; x++)
			{
				int bit0 = (plane0 >> (7 - x)) & 1;
				int bit1 = (plane1 >> (7 - x)) & 1;
				int paletteIndex = (bit1 << 1) | bit0;

				Color color;
				if (paletteIndex == 0)
				{
					color = Color.LightGray;
				}
				else
				{
					color = Palette.GetColor(Console.PpuMemory.Read((ushort)(SpriteInfo.PaletteBaseAddress + paletteIndex)));
				}

				bitmap[y * 8 + x] = color;
			}
		}

		return bitmap;
	}

	public Color[] Convert8x16SpriteToBitmap(byte[] spriteData)
	{
		Color[] bitmap = new Color[128];

		for (int y = 0; y < 16; y++)
		{
			byte plane0 = spriteData[y * 2];
			byte plane1 = spriteData[y * 2 + 1];

			for (int x = 0; x < 8; x++)
			{
				int bit0 = (plane0 >> (7 - x)) & 1;
				int bit1 = (plane1 >> (7 - x)) & 1;
				int paletteIndex = (bit1 << 1) | bit0;

				Color color;
				if (paletteIndex == 0)
				{
					color = Color.LightGray;
				}
				else
				{
					color = Palette.GetColor(Console.PpuMemory.Read((ushort)(SpriteInfo.PaletteBaseAddress + paletteIndex)));
				}

				bitmap[(y * 8) + x] = color;
			}
		}
		return bitmap;
	}

	public byte[] Read8x8Sprite()
	{
		var spriteData = new byte[16];

		for (var row = 0; row < 8; row++)
		{
			var currentRow = SpriteInfo.FlipVertical ? 7 - row : row;

			var plane0 = Console.PpuMemory.Read((ushort)(SpriteInfo.TileOffset + row));
			var plane1 = Console.PpuMemory.Read((ushort)(SpriteInfo.TileOffset + row + 8));

			if (SpriteInfo.FlipHorizontal)
			{
				plane0 = ReverseBits(plane0);
				plane1 = ReverseBits(plane1);
			}

			spriteData[row * 2] = plane0;
			spriteData[row * 2 + 1] = plane1;
		}

		return spriteData;
	}

	public byte[] Read8x16Sprite()
	{
		//int patternTableAddress = (tileIndex & 1) * 0x1000; // Determine which pattern table to use
		//int tileAddress = patternTableAddress + (tileIndex & 0xFE) * 16; // Get the base address of the tile

		var spriteData = new byte[32];

		for (int row = 0; row < 16; row++)
		{
			var currentRow = SpriteInfo.FlipVertical ? 15 - row : row;
			var address = SpriteInfo.TileOffset + currentRow + (currentRow >= 8 ? 8 : 0);

			var plane0 = Console.PpuMemory.Read((ushort)address);
			var plane1 = Console.PpuMemory.Read((ushort)(address + 8));

			if (SpriteInfo.FlipHorizontal)
			{
				plane0 = ReverseBits(plane0);
				plane1 = ReverseBits(plane1);
			}

			spriteData[row * 2] = plane0;
			spriteData[row * 2 + 1] = plane1;
		}

		return spriteData;
	}

	private byte ReverseBits(byte b)
	{
		b = (byte)((b * 0x0202020202UL & 0x010884422010UL) % 1023);
		return b;
	}
}
