using Nescafe.UI.Debug.Controls;

namespace Nescafe.UI.Debug;

public partial class SpriteViewer : Form
{
	private readonly Core.Console _console;

	private const int SpriteWidth = 8;
	private const int SpriteHeight = 8;
	private const int PatternTableSize = 0x1000; // 4 KB for each pattern table
	private const int NumSprites = 64; // Number of sprites in OAM

	private List<Sprite> _sprites;

	public SpriteViewer(Core.Console console)
	{
		InitializeComponent();
		_console = console;

		Setup();
		UpdateSprites();
	}

	public void Setup()
	{
		_sprites = new List<Sprite>();
		for (int row = 0; row < 8; row++)
		{
			for (int col = 0; col < 8; col++)
			{
				var sprite = new Sprite
				{
					Console = _console,
					Width = 32,
					Height = 32,
					Top = (row * 32) + 5,
					Left = (col * 32) + 5,
				};
				Controls.Add(sprite);
				_sprites.Add(sprite);
			}
		}
	}

	private void UpdateSprites()
	{
		int tilesPerRow = 8;
		int bitmapWidth = tilesPerRow * SpriteWidth;
		int bitmapHeight = (NumSprites / tilesPerRow) * SpriteHeight;

		for (int spriteIndex = 0; spriteIndex < NumSprites; spriteIndex++)
		{
			//int yPos = _console.Ppu.State.Oam[spriteIndex * 4];
			int tileIndex = _console.Ppu.State.Oam[spriteIndex * 4 + 1];
			var attributes = _console.Ppu.State.Oam[spriteIndex * 4 + 2];
			//int xPos = _console.Ppu.State.Oam[spriteIndex * 4 + 3];
			// Assume the sprites are from pattern table 0
			int tileOffset = tileIndex * 16;

			var spriteInfo = new SpriteInfo
			{
				Index = spriteIndex,
				FlipHorizontal = (attributes & 0x40) != 0,
				FlipVertical = (attributes & 0x80) != 0,
				PaletteIndex = attributes & 0x03,
				TileOffset = tileOffset
			};

			_sprites[spriteIndex].UpdateSprite(spriteInfo);
		}
	}
}

public class SpriteInfo
{
	public int Index { get; set; }
	public bool FlipHorizontal { get; set; }
	public bool FlipVertical { get; set; }
	public int PaletteIndex { get; set; }
	public int TileOffset { get; set; }
	public ushort PaletteBaseAddress => (ushort)(0x3F10 + (PaletteIndex * 4));
}