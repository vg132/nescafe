using Nescafe.Core;
using System;

namespace Nescafe.UI.Debug.Controls;

public partial class Sprite : UserControl
{
	private const int SpriteWidth = 8;
	private const int SpriteHeight = 8;

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
		var pixleSize = Height > Width ? Height / SpriteHeight : Width / SpriteHeight;
		var g = CreateGraphics();
		for (int row = 0; row < SpriteHeight; row++)
		{
			byte plane0 = Console.PpuMemory.Read((ushort)(SpriteInfo.TileOffset + row));
			byte plane1 = Console.PpuMemory.Read((ushort)(SpriteInfo.TileOffset + row + 8));
			for (int col = 0; col < SpriteWidth; col++)
			{
				int bit0 = (plane0 >> (7 - col)) & 1;
				int bit1 = (plane1 >> (7 - col)) & 1;
				int colorIndex = (bit1 << 1) | bit0;

				Color color;
				if (colorIndex == 0)
				{
					color = Color.LightGray;
				}
				else
				{
					color = Palette.GetColor(Console.PpuMemory.Read((ushort)(SpriteInfo.PaletteBaseAddress + colorIndex)));
				}

				using (var brush = new SolidBrush(color))
				{
					var x = SpriteInfo.FlipHorizontal ? (8 - 1 - col) * pixleSize : col * pixleSize;
					var y = SpriteInfo.FlipVertical ? (8 - 1 - row) * pixleSize : row * pixleSize;
					g.FillRectangle(brush, x, y, pixleSize, pixleSize);
				}
			}
		}
	}
}
