namespace Nescafe.Core;

/// <summary>
/// Represents a NTSC PPU.
/// </summary>
public partial class Ppu
{
	/// <summary>
	/// Gets an array containing bitmap data currently drawn to the screen.
	/// </summary>
	/// <value>The bitmap data.</value>
	public byte[] BitmapData { get; }

	private readonly PpuMemory _memory;
	private readonly Console _console;

	private readonly int _width = 256;
	private readonly int _height = 240;

	public PpuState State { get; private set; }

	/// <summary>
	/// Is <c>true</c> if rendering is currently enabled.
	/// </summary>
	/// <value><c>true</c> if rendering is enabled; otherwise, <c>false</c>.</value>
	public bool RenderingEnabled => State.FlagShowSprites != 0 || State.FlagShowBackground != 0;

	/// <summary>
	/// Constructs a new PPU.
	/// </summary>
	/// <param name="console">Console that this PPU is a part of</param>
	public Ppu(Console console)
	{
		_memory = console.PpuMemory;
		_console = console;
		State = new PpuState();

		BitmapData = new byte[_width * _height];

		State.Oam = new byte[256];
		State.Sprites = new byte[32];
		State.SpriteIndicies = new int[8];
	}

	/// <summary>
	/// Resets this PPU to its startup state.
	/// </summary>
	public void Reset()
	{
		Array.Clear(BitmapData, 0, BitmapData.Length);

		State.Scanline = 240;
		State.Cycle = 340;

		State.NmiOccurred = 0;
		State.NmiOutput = 0;

		State.W = 0;
		State.F = 0;

		Array.Clear(State.Oam, 0, State.Oam.Length);
		Array.Clear(State.Sprites, 0, State.Sprites.Length);
	}

	private byte LookupBackgroundColor(byte data)
	{
		var colorNum = data & 0x3;
		var paletteNum = (data >> 2) & 0x3;

		// Special case for universal background color
		if (colorNum == 0)
		{
			return _memory.Read(0x3F00);
		}

		ushort paletteAddress;
		switch (paletteNum)
		{
			case 0:
				paletteAddress = 0x3F01;
				break;
			case 1:
				paletteAddress = 0x3F05;
				break;
			case 2:
				paletteAddress = 0x3F09;
				break;
			case 3:
				paletteAddress = 0x3F0D;
				break;
			default:
				throw new Exception("Invalid background palette Number: " + paletteNum.ToString());
		}

		paletteAddress += (ushort)(colorNum - 1);
		return _memory.Read(paletteAddress);
	}

	private byte LookupSpriteColor(byte data)
	{
		var colorNum = data & 0x3;
		var paletteNum = (data >> 2) & 0x3;

		// Special case for universal background color
		if (colorNum == 0)
		{
			return _memory.Read(0x3F00);
		}

		ushort paletteAddress;
		switch (paletteNum)
		{
			case 0:
				paletteAddress = 0x3F11;
				break;
			case 1:
				paletteAddress = 0x3F15;
				break;
			case 2:
				paletteAddress = 0x3F19;
				break;
			case 3:
				paletteAddress = 0x3F1D;
				break;
			default:
				throw new Exception("Invalid background palette Number: " + paletteNum.ToString());
		}

		paletteAddress += (ushort)(colorNum - 1);
		return _memory.Read(paletteAddress);
	}

	private byte GetBgPixelData()
	{
		var xPos = State.Cycle - 1;

		return State.FlagShowBackground == 0 ? (byte)0 : State.FlagShowBackgroundLeft == 0 && xPos < 8 ? (byte)0 : (byte)((State.TileShiftReg >> (State.X * 4)) & 0xF);
	}

	private byte GetSpritePixelData(out int spriteIndex)
	{
		var xPos = State.Cycle - 1;
		var yPos = State.Scanline - 1;

		spriteIndex = 0;

		if (State.FlagShowSprites == 0)
		{
			return 0;
		}

		if (State.FlagShowSpritesLeft == 0 && xPos < 8)
		{
			return 0;
		}

		// 8x8 sprites all come from the same pattern table as specified by a write to PPUCTRL
		// 8x16 sprites come from a pattern table defined in their OAM data
		var _currSpritePatternTableAddr = State.SpritePatternTableAddress;

		// Get sprite pattern bitfield
		for (var i = 0; i < State.NumSprites * 4; i += 4)
		{
			int spriteXLeft = State.Sprites[i + 3];
			var offset = xPos - spriteXLeft;

			if (offset <= 7 && offset >= 0)
			{
				// Found intersecting sprite
				var yOffset = yPos - State.Sprites[i];

				byte patternIndex;

				// Set the pattern table and index according to whether or not sprites
				// ar 8x8 or 8x16
				if (State.FlagSpriteSize == 1)
				{
					_currSpritePatternTableAddr = (ushort)((State.Sprites[i + 1] & 1) * 0x1000);
					patternIndex = (byte)(State.Sprites[i + 1] & 0xFE);
				}
				else
				{
					patternIndex = State.Sprites[i + 1];
				}

				var patternAddress = (ushort)(_currSpritePatternTableAddr + (patternIndex * 16));

				var flipHoriz = (State.Sprites[i + 2] & 0x40) != 0;
				var flipVert = (State.Sprites[i + 2] & 0x80) != 0;
				var colorNum = GetSpritePatternPixel(patternAddress, offset, yOffset, flipHoriz, flipVert);

				// Handle transparent sprites
				if (colorNum == 0)
				{
					continue;
				}
				else // Non transparent sprite, return data
				{
					var paletteNum = (byte)(State.Sprites[i + 2] & 0x03);
					spriteIndex = i / 4;
					return (byte)(((paletteNum << 2) | colorNum) & 0xF);
				}
			}
		}

		return 0x00; // No sprite
	}

	private void CopyHorizPositionData()
	{
		// v: ....F.. ...EDCBA = t: ....F.. ...EDCBA
		State.V = (ushort)((State.V & 0x7BE0) | (State.T & 0x041F));
	}

	private void CopyVertPositionData()
	{
		// v: IHGF.ED CBA..... = t: IHGF.ED CBA.....
		State.V = (ushort)((State.V & 0x041F) | (State.T & 0x7BE0));
	}

	private int CoarseX()
	{
		return State.V & 0x1f;
	}

	private int CoarseY()
	{
		return (State.V >> 5) & 0x1f;
	}

	private int FineY()
	{
		return (State.V >> 12) & 0x7;
	}

	private int GetSpritePatternPixel(ushort patternAddr, int xPos, int yPos, bool flipHoriz = false, bool flipVert = false)
	{
		var h = State.FlagSpriteSize == 0 ? 7 : 15;

		// Flip x and y if needed
		xPos = flipHoriz ? 7 - xPos : xPos;
		yPos = flipVert ? h - yPos : yPos;

		// First byte in bitfield, wrapping accordingly for y > 7 (8x16 sprites)
		ushort yAddr;
		if (yPos <= 7)
		{
			yAddr = (ushort)(patternAddr + yPos);
		}
		else
		{
			yAddr = (ushort)(patternAddr + 16 + (yPos - 8)); // Go to next tile for 8x16 sprites
		}

		// Read the 2 bytes in the bitfield for the y coordinate
		var pattern = new byte[2];
		pattern[0] = _memory.Read(yAddr);
		pattern[1] = _memory.Read((ushort)(yAddr + 8));

		// Extract correct bits based on x coordinate
		var loBit = (byte)((pattern[0] >> (7 - xPos)) & 1);
		var hiBit = (byte)((pattern[1] >> (7 - xPos)) & 1);

		return ((hiBit << 1) | loBit) & 0x03;
	}

	private void IncrementX()
	{
		if ((State.V & 0x001F) == 31)
		{
			State.V = (ushort)(State.V & (~0x001F)); // Reset Coarse X
			State.V = (ushort)(State.V ^ 0x0400); // Switch horizontal nametable
		}
		else
		{
			State.V++; // Increment Coarse X
		}
	}

	private void IncrementY()
	{
		if ((State.V & 0x7000) != 0x7000)
		{ // if fine Y < 7
			State.V += 0x1000; // increment fine Y
		}
		else
		{
			State.V = (ushort)(State.V & ~0x7000); // Set fine Y to 0
			var y = (State.V & 0x03E0) >> 5; // y = coarse Y
			if (y == 29)
			{
				y = 0; // coarse Y = 0
				State.V = (ushort)(State.V ^ 0x0800); // switch vertical nametable
			}
			else if (y == 31)
			{
				y = 0; // coarse Y = 0, nametable not switched
			}
			else
			{
				y += 1; // Increment coarse Y
			}
			State.V = (ushort)((State.V & ~0x03E0) | (y << 5)); // Put coarse Y back into v
		}
	}

	private void EvalSprites()
	{
		Array.Clear(State.Sprites, 0, State.Sprites.Length);
		Array.Clear(State.SpriteIndicies, 0, State.SpriteIndicies.Length);

		// 8x8 or 8x16 sprites
		var h = State.FlagSpriteSize == 0 ? 7 : 15;

		State.NumSprites = 0;
		var yPos = State.Scanline;

		// Sprite evaluation starts at the current OAM address and goes to the end of OAM (256 bytes)
		for (int i = State.OamAddr; i < 256; i += 4)
		{
			var spriteYTop = State.Oam[i];

			var offset = yPos - spriteYTop;

			// If this sprite is on the next State.Scanline, copy it to the State._sprites array for rendering
			if (offset <= h && offset >= 0)
			{
				if (State.NumSprites == 8)
				{
					State.FlagSpriteOverflow = 1;
					break;
				}
				else
				{
					Array.Copy(State.Oam, i, State.Sprites, State.NumSprites * 4, 4);
					State.SpriteIndicies[State.NumSprites] = (i - State.OamAddr) / 4;
					State.NumSprites++;

				}
			}
		}
	}

	private void RenderPixel()
	{
		// Get pixel data (4 bits of tile shift register as specified by x)
		var bgPixelData = GetBgPixelData();

		var spritePixelData = GetSpritePixelData(out var spriteScanlineIndex);
		var isSpriteZero = State.FlagSpriteZeroHit == 0 && State.FlagShowBackground == 1 && State.SpriteIndicies[spriteScanlineIndex] == 0;

		var bgColorNum = bgPixelData & 0x03;
		var spriteColorNum = spritePixelData & 0x03;

		byte color;

		if (bgColorNum == 0)
		{
			color = spriteColorNum == 0 ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
		}
		else
		{
			if (spriteColorNum == 0)
			{
				color = LookupBackgroundColor(bgPixelData);
			}
			else // Both pixels opaque, choose depending on sprite priority
			{
				// Set sprite zero hit flag
				if (isSpriteZero)
				{
					State.FlagSpriteZeroHit = 1;
				}

				// Get sprite priority
				var priority = (State.Sprites[(spriteScanlineIndex * 4) + 2] >> 5) & 1;
				color = priority == 1 ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
			}
		}

		BitmapData[(State.Scanline * 256) + (State.Cycle - 1)] = color;
	}

	private void FetchNametableByte()
	{
		var address = (ushort)(0x2000 | (State.V & 0x0FFF));
		State.NameTableByte = _memory.Read(address);
	}

	private void FetchAttributeTableByte()
	{
		var address = (ushort)(0x23C0 | (State.V & 0x0C00) | ((State.V >> 4) & 0x38) | ((State.V >> 2) & 0x07));
		State.AttributeTableByte = _memory.Read(address);
	}

	private void FetchTileBitfieldLo()
	{
		var address = (ushort)(State.BgPatternTableAddress + (State.NameTableByte * 16) + FineY());
		State.TileBitfieldLo = _memory.Read(address);
	}

	private void FetchTileBitfieldHi()
	{
		var address = (ushort)(State.BgPatternTableAddress + (State.NameTableByte * 16) + FineY() + 8);
		State.TileBitfieldHi = _memory.Read(address);
	}

	// Stores data for the next 8 pixels in the upper 32 bits of State._tileShiftReg
	private void StoreTileData()
	{
		var _palette = (byte)((State.AttributeTableByte >> ((CoarseX() & 0x2) | ((CoarseY() & 0x2) << 1))) & 0x3);

		// Upper 32 bits to add to State._tileShiftReg
		ulong data = 0;

		for (var i = 0; i < 8; i++)
		{
			// Get color number
			var loColorBit = (byte)((State.TileBitfieldLo >> (7 - i)) & 1);
			var hiColorBit = (byte)((State.TileBitfieldHi >> (7 - i)) & 1);
			var colorNum = (byte)((hiColorBit << 1) | ((loColorBit) & 0x03));

			// Add palette number
			var fullPixelData = (byte)(((_palette << 2) | colorNum) & 0xF);

			data |= (uint)(fullPixelData << (4 * i));
		}

		State.TileShiftReg &= 0xFFFFFFFF;
		State.TileShiftReg |= data << 32;
	}

	// Updates State.Scanline and State.Cycle counters, triggers NMI's if needed.
	private void UpdateCounters()
	{
		// Trigger an NMI at the start of _State.Scanline 241 if VBLANK NMI's are enabled
		if (State.Scanline == 241 && State.Cycle == 1)
		{
			State.NmiOccurred = 1;
			if (State.NmiOutput != 0)
			{
				_console.Cpu.TriggerNmi();
			}
		}

		var renderingEnabled = (State.FlagShowBackground != 0) || (State.FlagShowSprites != 0);

		// Skip last State.Cycle of prerender State.Scanline on odd frames
		if (renderingEnabled)
		{
			if (State.Scanline == 261 && State.F == 1 && State.Cycle == 339)
			{
				State.F ^= 1;
				State.Scanline = 0;
				State.Cycle = -1;
				_console.DrawFrame();
				return;
			}
		}
		State.Cycle++;

		// Reset State.Cycle (and State.Scanline if State.Scanline == 260)
		// Also set to next frame if at end of last _State.Scanline
		if (State.Cycle > 340)
		{
			if (State.Scanline == 261) // Last State.Scanline, reset to upper left corner
			{
				State.F ^= 1;
				State.Scanline = 0;
				State.Cycle = -1;
				_console.DrawFrame();
			}
			else // Not on last State.Scanline
			{
				State.Cycle = -1;
				State.Scanline++;
			}
		}
	}

	/// <summary>
	/// Executes a single PPU step.
	/// </summary>
	public void Step()
	{
		UpdateCounters();

		// State.Cycle types
		var renderingEnabled = (State.FlagShowBackground != 0) || (State.FlagShowSprites != 0);
		var renderCycle = State.Cycle > 0 && State.Cycle <= 256;
		var preFetchCycle = State.Cycle >= 321 && State.Cycle <= 336;
		var fetchCycle = renderCycle || preFetchCycle;

		// State.Scanline types
		var renderScanline = State.Scanline >= 0 && State.Scanline < 240;
		var idleScanline = State.Scanline == 240;
		var vBlankScanline = State.Scanline > 240;
		var preRenderScanline = State.Scanline == 261;

		// nmiOccurred flag cleared on prerender State.Scanline at State.Cycle 1
		if (preRenderScanline && State.Cycle == 1)
		{
			State.NmiOccurred = 0;
			State.FlagSpriteOverflow = 0;
			State.FlagSpriteZeroHit = 0;
		}

		if (renderingEnabled)
		{
			// Evaluate sprites at State.Cycle 257 of each render State.Scanline
			if (State.Cycle == 257)
			{
				if (renderScanline)
				{
					EvalSprites();
				}
				else
				{
					State.NumSprites = 0;
				}
			}

			if (renderCycle && renderScanline)
			{
				RenderPixel();
			}

			// Read rendering data into internal latches and update State._tileShiftReg
			// with those latches every 8 State.Cycles
			// https://wiki.nesdev.com/w/images/d/d1/Ntsc_timing.png
			if (fetchCycle && (renderScanline || preRenderScanline))
			{
				State.TileShiftReg >>= 4;
				switch (State.Cycle % 8)
				{
					case 1:
						FetchNametableByte();
						break;
					case 3:
						FetchAttributeTableByte();
						break;
					case 5:
						FetchTileBitfieldLo();
						break;
					case 7:
						FetchTileBitfieldHi();
						break;
					case 0:
						StoreTileData();
						IncrementX();
						if (State.Cycle == 256)
						{
							IncrementY();
						}

						break;
				}
			}

			// OAMADDR is set to 0 during each of ticks 257-320 (the sprite tile loading interval) of the pre-render and visible State.Scanlines. 
			if (State.Cycle > 257 && State.Cycle <= 320 && (preRenderScanline || renderScanline))
			{
				State.OamAddr = 0;
			}

			// Copy horizontal position data from t to v on _State.Cycle 257 of each State.Scanline if rendering enabled
			if (State.Cycle == 257 && (renderScanline || preRenderScanline))
			{
				CopyHorizPositionData();
			}

			// Copy vertical position data from t to v repeatedly from State.Cycle 280 to 304 (if rendering is enabled)
			if (State.Cycle >= 280 && State.Cycle <= 304 && State.Scanline == 261)
			{
				CopyVertPositionData();
			}
		}
	}

	/// <summary>
	/// Reads a byte from the register at the specified address.
	/// </summary>
	/// <returns>A byte read from the register at the specified address</returns>
	/// <param name="address">The address of the register to read from</param>
	public byte ReadFromRegister(ushort address)
	{
		byte data;
		switch (address)
		{
			case 0x2002:
				data = ReadPpuStatus();
				break;
			case 0x2004:
				data = ReadOamData();
				break;
			case 0x2007:
				data = ReadPpuData();
				break;
			default:
				//System.Diagnostics.Debug.WriteLine($"Invalid PPU Register read from register: {address.ToString("X4")}");
				//data = 0x00;
				//break;
				throw new Exception("Invalid PPU Register read from register: " + address.ToString("X4"));
		}

		return data;
	}

	/// <summary>
	/// Writes a byte to the register at the specified address.
	/// </summary>
	/// <param name="address">The address of the register to write to</param>
	/// <param name="data">The byte to write to the register</param>
	public void WriteToRegister(ushort address, byte data)
	{
		State.LastRegisterWrite = data;
		switch (address)
		{
			case 0x2000:
				WritePpuCtrl(data);
				break;
			case 0x2001:
				WritePpuMask(data);
				break;
			case 0x2003:
				WriteOamAddr(data);
				break;
			case 0x2004:
				WriteOamData(data);
				break;
			case 0x2005:
				WritePpuScroll(data);
				break;
			case 0x2006:
				WritePpuAddr(data);
				break;
			case 0x2007:
				WritePpuData(data);
				break;
			case 0x4014:
				WriteOamDma(data);
				break;
			default:
				throw new Exception("Invalid PPU Register write to register: " + address.ToString("X4"));
		}
	}

	// $2000
	private void WritePpuCtrl(byte data)
	{
		State.FlagBaseNametableAddr = (byte)(data & 0x3);
		State.FlagVRamIncrement = (byte)((data >> 2) & 1);
		State.FlagSpritePatternTableAddr = (byte)((data >> 3) & 1);
		State.FlagBgPatternTableAddr = (byte)((data >> 4) & 1);
		State.FlagSpriteSize = (byte)((data >> 5) & 1);
		State.FlagMasterSlaveSelect = (byte)((data >> 6) & 1);
		State.NmiOutput = (byte)((data >> 7) & 1);

		// Set values based off flags
		State.BaseNametableAddress = (ushort)(0x2000 + (0x400 * State.FlagBaseNametableAddr));
		State.VRamIncrement = (State.FlagVRamIncrement == 0) ? 1 : 32;
		State.BgPatternTableAddress = (ushort)(State.FlagBgPatternTableAddr == 0 ? 0x0000 : 0x1000);
		State.SpritePatternTableAddress = (ushort)(0x1000 * State.FlagSpritePatternTableAddr);

		// t: ...BA.. ........ = d: ......BA
		State.T = (ushort)((State.T & 0xF3FF) | ((data & 0x03) << 10));
	}

	// $2001
	private void WritePpuMask(byte data)
	{
		State.FlagGreyscale = (byte)(data & 1);
		State.FlagShowBackgroundLeft = (byte)((data >> 1) & 1);
		State.FlagShowSpritesLeft = (byte)((data >> 2) & 1);
		State.FlagShowBackground = (byte)((data >> 3) & 1);
		State.FlagShowSprites = (byte)((data >> 4) & 1);
		State.FlagEmphasizeRed = (byte)((data >> 5) & 1);
		State.FlagEmphasizeGreen = (byte)((data >> 6) & 1);
		State.FlagEmphasizeBlue = (byte)((data >> 7) & 1);
	}

	// $4014
	private void WriteOamAddr(byte data)
	{
		State.OamAddr = data;
	}

	// $2004
	private void WriteOamData(byte data)
	{
		State.Oam[State.OamAddr] = data;
		State.OamAddr++;
	}

	// $2005
	private void WritePpuScroll(byte data)
	{
		if (State.W == 0) // First write
		{
			// t: ....... ...HGFED = d: HGFED...
			// x:              CBA = d: .....CBA
			// w:                  = 1
			State.T = (ushort)((State.T & 0xFFE0) | (data >> 3));
			State.X = (byte)(data & 0x07);
			State.W = 1;
		}
		else
		{
			// t: CBA..HG FED..... = d: HGFEDCBA
			// w:                  = 0
			State.T = (ushort)(State.T & 0xC1F);
			State.T |= (ushort)((data & 0x07) << 12); // CBA
			State.T |= (ushort)((data & 0xF8) << 2); // HG FED
			State.W = 0;
		}
	}

	// $2006
	private void WritePpuAddr(byte data)
	{
		if (State.W == 0)  // First write
		{
			// t: .FEDCBA ........ = d: ..FEDCBA
			// t: X...... ........ = 0
			// w:                  = 1
			State.T = (ushort)((State.T & 0x00FF) | (data << 8));
			State.W = 1;
		}
		else
		{
			// t: ....... HGFEDCBA = d: HGFEDCBA
			// v                   = t
			// w:                  = 0
			State.T = (ushort)((State.T & 0xFF00) | data);
			State.V = State.T;
			State.W = 0;
		}
	}

	// $2007
	private void WritePpuData(byte data)
	{
		_memory.Write(State.V, data);
		State.V += (ushort)State.VRamIncrement;
	}

	// $4014
	private void WriteOamDma(byte data)
	{
		var startAddr = (ushort)(data << 8);
		_console.CpuMemory.ReadBufWrapping(State.Oam, State.OamAddr, startAddr, 256);

		// OAM DMA always takes at least 513 CPU State.Cycles
		_console.Cpu.AddIdleCycles(513);

		// OAM DMA takes an extra CPU State.Cycle if executed on an odd CPU State.Cycle
		if (_console.Cpu.State.Cycles % 2 == 1)
		{
			_console.Cpu.AddIdleCycles(1);
		}
	}

	// $2002
	private byte ReadPpuStatus()
	{
		byte retVal = 0;
		retVal |= (byte)(State.LastRegisterWrite & 0x1F); // Least signifigant 5 bits of last register write
		retVal |= (byte)(State.FlagSpriteOverflow << 5);
		retVal |= (byte)(State.FlagSpriteZeroHit << 6);
		retVal |= (byte)(State.NmiOccurred << 7);

		State.NmiOccurred = 0;
		State.W = 0;
		return retVal;
	}

	// $2004
	private byte ReadOamData()
	{
		return State.Oam[State.OamAddr];
	}

	// $2007
	private byte ReadPpuData()
	{
		var data = _memory.Read(State.V);

		// Buffered read emulation
		// https://wiki.nesdev.com/w/index.php/PPU_registers#The_PPUDATA_read_buffer_.28post-fetch.29
		if (State.V < 0x3F00)
		{
			var bufferedData = State.PpuDataBuffer;
			State.PpuDataBuffer = data;
			data = bufferedData;
		}
		else
		{
			State.PpuDataBuffer = _memory.Read((ushort)(State.V - 0x1000));
		}

		State.V += (ushort)State.VRamIncrement;
		return data;
	}

	#region Save/Load state

	public object SaveState()
	{
		//lock (_console.CpuState.CycleLock)
		{
			//state.PpuMemory = _memory.SaveState();

			return State;
		}
	}

	public void LoadState(object stateObj)
	{
		//lock (_console.CpuState.CycleLock)
		{
			var state = stateObj as PpuState;
			State = state;
			//_memory.LoadState(state.PpuMemory);
		}
	}

	#endregion
}