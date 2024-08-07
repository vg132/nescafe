namespace Nescafe.Core;

/// <summary>
/// Represents a NTSC PPU.
/// </summary>
public class VGPpu : IPpu
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

	private const int _scanlineCount = 261;
	private const int _cyclesPerLine = 341;

	//public VGPpuState State { get; private set; }
	private VGPpuState _state;

	/// <summary>
	/// Is <c>true</c> if rendering is currently enabled.
	/// </summary>
	/// <value><c>true</c> if rendering is enabled; otherwise, <c>false</c>.</value>
	public bool RenderingEnabled => _state.ShowSprites || _state.ShowBackground;

	PpuState IPpu.State => _state;

	/// <summary>
	/// Constructs a new PPU.
	/// </summary>
	/// <param name="console">Console that this PPU is a part of</param>
	public VGPpu(Console console)
	{
		_memory = console.PpuMemory;
		_console = console;
		_state = new VGPpuState();

		BitmapData = new byte[_width * _height];

		_state.Oam = new byte[256];
		_state.Sprites = new byte[32];
		_state.SpriteIndicies = new int[8];
	}

	/// <summary>
	/// Resets this PPU to its startup state.
	/// </summary>
	public void Reset()
	{
		Array.Clear(BitmapData, 0, BitmapData.Length);
		_state.Reset();
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
		var xPos = _state.Cycle - 1;
		if (_state.ShowBackground && (_state.ShowBackgroundLeft || xPos >= 8))
		{
			return (byte)((_state.TileShiftReg >> (_state.X * 4)) & 0xF);
		}
		return (byte)0;
	}

	private byte GetSpritePixelData(out int spriteIndex)
	{
		var xPos = _state.Cycle - 1;
		var yPos = _state.Scanline - 1;

		spriteIndex = 0;

		if (!_state.ShowSprites)
		{
			return 0;
		}

		if (!_state.ShowSpritesLeft && xPos < 8)
		{
			return 0;
		}

		// 8x8 sprites all come from the same pattern table as specified by a write to PPUCTRL
		// 8x16 sprites come from a pattern table defined in their OAM data
		var _currSpritePatternTableAddr = _state.SpritePatternTableAddress;

		// Get sprite pattern bitfield
		for (var i = 0; i < _state.NumSprites * 4; i += 4)
		{
			int spriteXLeft = _state.Sprites[i + 3];
			var offset = xPos - spriteXLeft;

			if (offset <= 7 && offset >= 0)
			{
				// Found intersecting sprite
				var yOffset = yPos - _state.Sprites[i];

				byte patternIndex;

				// Set the pattern table and index according to whether or not sprites
				// ar 8x8 or 8x16
				if (_state.FlagLargeSprites)
				{
					_currSpritePatternTableAddr = (ushort)((_state.Sprites[i + 1] & 1) * 0x1000);
					patternIndex = (byte)(_state.Sprites[i + 1] & 0xFE);
				}
				else
				{
					patternIndex = _state.Sprites[i + 1];
				}

				var patternAddress = (ushort)(_currSpritePatternTableAddr + (patternIndex * 16));

				var flipHoriz = (_state.Sprites[i + 2] & 0x40) != 0;
				var flipVert = (_state.Sprites[i + 2] & 0x80) != 0;
				var colorNum = GetSpritePatternPixel(patternAddress, offset, yOffset, flipHoriz, flipVert);

				// Handle transparent sprites
				if (colorNum == 0)
				{
					continue;
				}
				else // Non transparent sprite, return data
				{
					var paletteNum = (byte)(_state.Sprites[i + 2] & 0x03);
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
		_state.V = (ushort)((_state.V & 0x7BE0) | (_state.T & 0x041F));
	}

	private void CopyVertPositionData()
	{
		// v: IHGF.ED CBA..... = t: IHGF.ED CBA.....
		_state.V = (ushort)((_state.V & 0x041F) | (_state.T & 0x7BE0));
	}

	private int CoarseX()
	{
		return _state.V & 0x1f;
	}

	private int CoarseY()
	{
		return (_state.V >> 5) & 0x1f;
	}

	private int FineY()
	{
		return (_state.V >> 12) & 0x7;
	}

	private int GetSpritePatternPixel(ushort patternAddr, int xPos, int yPos, bool flipHoriz = false, bool flipVert = false)
	{
		var h = _state.FlagLargeSprites ? 15 : 7;

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
		if ((_state.V & 0x001F) == 31)
		{
			_state.V = (ushort)(_state.V & (~0x001F)); // Reset Coarse X
			_state.V = (ushort)(_state.V ^ 0x0400); // Switch horizontal nametable
		}
		else
		{
			_state.V++; // Increment Coarse X
		}
	}

	private void IncrementY()
	{
		if ((_state.V & 0x7000) != 0x7000)
		{ // if fine Y < 7
			_state.V += 0x1000; // increment fine Y
		}
		else
		{
			_state.V = (ushort)(_state.V & ~0x7000); // Set fine Y to 0
			var y = (_state.V & 0x03E0) >> 5; // y = coarse Y
			if (y == 29)
			{
				y = 0; // coarse Y = 0
				_state.V = (ushort)(_state.V ^ 0x0800); // switch vertical nametable
			}
			else if (y == 31)
			{
				y = 0; // coarse Y = 0, nametable not switched
			}
			else
			{
				y += 1; // Increment coarse Y
			}
			_state.V = (ushort)((_state.V & ~0x03E0) | (y << 5)); // Put coarse Y back into v
		}
	}

	private void EvalSprites()
	{
		Array.Clear(_state.Sprites, 0, _state.Sprites.Length);
		Array.Clear(_state.SpriteIndicies, 0, _state.SpriteIndicies.Length);

		// 8x8 or 8x16 sprites
		var h = _state.FlagLargeSprites ? 15 : 7;

		_state.NumSprites = 0;
		var yPos = _state.Scanline;

		// Sprite evaluation starts at the current OAM address and goes to the end of OAM (256 bytes)
		for (int i = _state.OamAddr; i < 256; i += 4)
		{
			var spriteYTop = _state.Oam[i];

			var offset = yPos - spriteYTop;

			// If this sprite is on the next _state.Scanline, copy it to the _state._sprites array for rendering
			if (offset <= h && offset >= 0)
			{
				if (_state.NumSprites == 8)
				{
					_state.FlagSpriteOverflow = true;
					break;
				}
				else
				{
					Array.Copy(_state.Oam, i, _state.Sprites, _state.NumSprites * 4, 4);
					_state.SpriteIndicies[_state.NumSprites] = (i - _state.OamAddr) / 4;
					_state.NumSprites++;

				}
			}
		}
	}

	private void RenderPixel()
	{
		// Get pixel data (4 bits of tile shift register as specified by x)
		var bgPixelData = GetBgPixelData();

		var spritePixelData = GetSpritePixelData(out var spriteScanlineIndex);
		var isSpriteZero = _state.FlagSpriteZeroHit == false && _state.ShowBackground && _state.SpriteIndicies[spriteScanlineIndex] == 0;

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
					_state.FlagSpriteZeroHit = true;
				}

				// Get sprite priority
				var priority = (_state.Sprites[(spriteScanlineIndex * 4) + 2] >> 5) & 1;
				color = priority == 1 ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
			}
		}

		BitmapData[(_state.Scanline * 256) + (_state.Cycle - 1)] = color;
	}

	private void FetchNametableByte()
	{
		var address = (ushort)(0x2000 | (_state.V & 0x0FFF));
		_state.NameTableByte = _memory.Read(address);
	}

	private void FetchAttributeTableByte()
	{
		var address = (ushort)(0x23C0 | (_state.V & 0x0C00) | ((_state.V >> 4) & 0x38) | ((_state.V >> 2) & 0x07));
		_state.AttributeTableByte = _memory.Read(address);
	}

	private void FetchTileBitfieldLo()
	{
		var address = (ushort)(_state.BgPatternTableAddress + (_state.NameTableByte * 16) + FineY());
		_state.TileBitfieldLo = _memory.Read(address);
	}

	private void FetchTileBitfieldHi()
	{
		var address = (ushort)(_state.BgPatternTableAddress + (_state.NameTableByte * 16) + FineY() + 8);
		_state.TileBitfieldHi = _memory.Read(address);
	}

	// Stores data for the next 8 pixels in the upper 32 bits of _state._tileShiftReg
	private void StoreTileData()
	{
		var _palette = (byte)((_state.AttributeTableByte >> ((CoarseX() & 0x2) | ((CoarseY() & 0x2) << 1))) & 0x3);

		// Upper 32 bits to add to _state._tileShiftReg
		ulong data = 0;

		for (var i = 0; i < 8; i++)
		{
			// Get color number
			var loColorBit = (byte)((_state.TileBitfieldLo >> (7 - i)) & 1);
			var hiColorBit = (byte)((_state.TileBitfieldHi >> (7 - i)) & 1);
			var colorNum = (byte)((hiColorBit << 1) | ((loColorBit) & 0x03));

			// Add palette number
			var fullPixelData = (byte)(((_palette << 2) | colorNum) & 0xF);

			data |= (uint)(fullPixelData << (4 * i));
		}

		_state.TileShiftReg &= 0xFFFFFFFF;
		_state.TileShiftReg |= data << 32;
	}

	private long cpuCalls = 0;
	private long ppuCalls = 0;
	// Updates scanline and cycle counters, triggers NMI's if needed.
	private void UpdateCounters()
	{
		// Skip last cycle on prerender line
		if (RenderingEnabled && _state.Scanline == (_scanlineCount - 1) && !_state.IsEvenFrame && _state.Cycle == (_cyclesPerLine - 2))
		{
			return;
		}
	}

	private void HandleNMIAndVBlank()
	{
		if (_state.VBlankStarted && cpuClocksSinceVBlank == 2270)
		{
			_state.VBlankStarted = false;
			cpuClocksSinceVBlank = 0;
		}
		// Trigger an NMI at the start of scanline 241 if VBLANK NMI's are enabled
		if (_state.Cycle == 1)
		{
			if (_state.Scanline == 241)
			{
				cpuClocksSinceVBlank = 0;
				_state.VBlankStarted = true;
				if (_state.NmiOutput != 0)
				{
					_console.Cpu.TriggerNmi();
				}
			}
			else if (_state.Scanline == -1)
			{
				_state.VBlankStarted = false;
				_state.FlagSpriteOverflow = false;
				_state.FlagSpriteZeroHit = false;
				cpuClocksSinceVBlank = 0;
			}
		}
	}

	// Render Frame
	public void Step()
	{
		for (; _state.Scanline < _scanlineCount; _state.Scanline++)
		{
			ProcessScanline(_state.Scanline);
		}
		_state.FrameCounter++;
		_state.Scanline = -1;
		_console.DrawFrame();
	}

	private void ProcessScanline(int line)
	{
		for (; _state.Cycle < _cyclesPerLine; _state.Cycle++)
		{
			ProcessCycle(_state.Scanline, _state.Cycle);
		}
		_state.Cycle = 0;
	}

	int cpuSyncCounter = 0;
	int cpuClocksSinceVBlank = 0;
	private void ProcessCycle(int line, int cycle)
	{
		ppuCalls++;
		OldStep();
		HandleNMIAndVBlank();
		_console.Mapper.Step();
		if (++cpuSyncCounter == 3)
		{
			if (_state.VBlankStarted)
			{
				cpuClocksSinceVBlank++;
			}
			cpuCalls++;
			_console.Cpu.Step();
			cpuSyncCounter = 0;
		}
	}

	/// <summary>
	/// Executes a single PPU step.
	/// </summary>
	private void OldStep()
	{
		UpdateCounters();

		// cycle types
		var renderCycle = _state.Cycle > 0 && _state.Cycle <= 256;
		var preFetchCycle = _state.Cycle >= 321 && _state.Cycle <= 336;
		var fetchCycle = renderCycle || preFetchCycle;

		// scanline types
		var renderScanline = _state.Scanline >= 0 && _state.Scanline < 240;
		var preRenderScanline = _state.Scanline == (_scanlineCount - 1);

		if (RenderingEnabled)
		{
			// Evaluate sprites at cycle 257 of each render scanline
			if (_state.Cycle == 257)
			{
				if (renderScanline)
				{
					EvalSprites();
				}
				else
				{
					_state.NumSprites = 0;
				}
			}

			if (renderCycle && renderScanline)
			{
				RenderPixel();
			}

			// Read rendering data into internal latches and update tileShiftReg
			// with those latches every 8 cycles
			// https://wiki.nesdev.com/w/images/d/d1/Ntsc_timing.png
			if (fetchCycle && (renderScanline || preRenderScanline))
			{
				_state.TileShiftReg >>= 4;
				switch (_state.Cycle % 8)
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
						if (_state.Cycle == 256)
						{
							IncrementY();
						}

						break;
				}
			}

			// OAMADDR is set to 0 during each of ticks 257-320 (the sprite tile loading interval) of the pre-render and visible _state.Scanlines. 
			if (_state.Cycle > 257 && _state.Cycle <= 320 && (preRenderScanline || renderScanline))
			{
				_state.OamAddr = 0;
			}

			// Copy horizontal position data from t to v on cycle 257 of each scanline if rendering enabled
			if (_state.Cycle == 257 && (renderScanline || preRenderScanline))
			{
				CopyHorizPositionData();
			}

			// Copy vertical position data from t to v repeatedly from cycle 280 to 304 (if rendering is enabled)
			if (_state.Cycle >= 280 && _state.Cycle <= 304 && _state.Scanline == (_scanlineCount - 1))
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
				//DebugEventService.Warning($"Invalid PPU Register read from register: {address.ToString("X4")}");
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
		_state.LastRegisterWrite = data;
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
	private void WritePpuCtrl(byte data)=> _state.PpuControl = data;

	// $2001
	private void WritePpuMask(byte data) => _state.PpuMask = data;

	// $2004
	private void WriteOamData(byte data)
	{
		_state.Oam[_state.OamAddr] = data;
		_state.OamAddr++;
	}

	// $4014
	private void WriteOamAddr(byte data) => _state.OamAddr = data;

	// $2005
	private void WritePpuScroll(byte data)
	{
		if (_state.W == 0) // First write
		{
			// t: ....... ...HGFED = d: HGFED...
			// x:              CBA = d: .....CBA
			// w:                  = 1
			_state.T = (ushort)((_state.T & 0xFFE0) | (data >> 3));
			_state.X = (byte)(data & 0x07);
			_state.W = 1;
		}
		else
		{
			// t: CBA..HG FED..... = d: HGFEDCBA
			// w:                  = 0
			_state.T = (ushort)(_state.T & 0xC1F);
			_state.T |= (ushort)((data & 0x07) << 12); // CBA
			_state.T |= (ushort)((data & 0xF8) << 2); // HG FED
			_state.W = 0;
		}
	}

	// $2006
	private void WritePpuAddr(byte data)
	{
		if (_state.W == 0)  // First write
		{
			// t: .FEDCBA ........ = d: ..FEDCBA
			// t: X...... ........ = 0
			// w:                  = 1
			_state.T = (ushort)((_state.T & 0x00FF) | (data << 8));
			_state.W = 1;
		}
		else
		{
			// t: ....... HGFEDCBA = d: HGFEDCBA
			// v                   = t
			// w:                  = 0
			_state.T = (ushort)((_state.T & 0xFF00) | data);
			_state.V = _state.T;
			_state.W = 0;
		}
	}

	// $2007
	private void WritePpuData(byte data)
	{
		_memory.Write(_state.V, data);
		_state.V += (ushort)_state.VRamIncrement;
	}

	// $4014
	private void WriteOamDma(byte data)
	{
		var startAddr = (ushort)(data << 8);
		_console.CpuMemory.ReadBufWrapping(_state.Oam, _state.OamAddr, startAddr, 256);

		// OAM DMA always takes at least 513 CPU cycles
		_console.Cpu.AddIdleCycles(513);

		// OAM DMA takes an extra CPU cycle if executed on an odd CPU cycle
		if (_console.Cpu.State.Cycles % 2 == 1)
		{
			_console.Cpu.AddIdleCycles(1);
		}
	}

	// $2002
	private byte ReadPpuStatus()
	{
		var retVal = _state.PpuStatus;

		_state.VBlankStarted = false;
		_state.W = 0;
		return retVal;
	}

	// $2004
	private byte ReadOamData() => _state.Oam[_state.OamAddr];

	// $2007
	private byte ReadPpuData()
	{
		var data = _memory.Read(_state.V);

		// Buffered read emulation
		// https://wiki.nesdev.com/w/index.php/PPU_registers#The_PPUDATA_read_buffer_.28post-fetch.29
		if (_state.V < 0x3F00)
		{
			var bufferedData = _state.PpuDataBuffer;
			_state.PpuDataBuffer = data;
			data = bufferedData;
		}
		else
		{
			_state.PpuDataBuffer = _memory.Read((ushort)(_state.V - 0x1000));
		}

		_state.V += (ushort)_state.VRamIncrement;
		return data;
	}

	#region Save/Load state

	public object SaveState()
	{
		//lock (_console.CpuState.CycleLock)
		{
			//state.PpuMemory = _memory.SaveState();

			return _state;
		}
	}

	public void LoadState(object stateObj)
	{
		//lock (_console.CpuState.CycleLock)
		{
			var state = stateObj as VGPpuState;
			_state = state;
			//_memory.LoadState(state.PpuMemory);
		}
	}

	#endregion
}
