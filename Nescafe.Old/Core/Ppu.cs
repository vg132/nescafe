using System;

namespace Nescafe.Core
{
	/// <summary>
	/// Represents a NTSC PPU.
	/// </summary>
	public class Ppu
	{
		/// <summary>
		/// Gets an array containing bitmap data currently drawn to the screen.
		/// </summary>
		/// <value>The bitmap data.</value>
		public byte[] BitmapData { get; }
		public byte[] NewBitmapData { get; }

		private readonly uint[] _palette = {
			0xA8A8A8, 0x003CE8, 0x102090, 0x600088, 0x880064, 0xB80060, 0xA80000, 0x783000,
			0x405400, 0x103C00, 0x008000, 0x007800, 0x00643C, 0x000000, 0x000000, 0x000000,
			0x989898, 0x104CC4, 0x6064EC, 0xB81ED4, 0x1100B0, 0x1A00C8, 0xF02020, 0x781E00,
			0x549A00, 0x286200, 0x106800, 0x003C28, 0x003C78, 0x000000, 0x000000, 0x000000,
			0xECEFEC, 0x4C9AEC, 0x784EC0, 0xD862EC, 0xE454EC, 0xEC58B4, 0xEC6A64, 0xD48820,
			0xA0AA00, 0x74C400, 0x4CD020, 0x38CC6C, 0x38B4CC, 0x3C3C3C, 0x000000, 0x000000,
			0xECEFEC, 0xA8CCEC, 0xBCBCBC, 0xD4B2EC, 0xECACEC, 0xECAFEC, 0xECB4B0, 0xE4C490,
			0xCCD278, 0xB4DE78, 0xA8E290, 0x98E2B4, 0xA0D6E4, 0xA0A2A0, 0x000000, 0x000000
		};

		readonly PpuMemory _memory;
		readonly Console _console;

		// OAM / Sprite rendering
		byte[] _oam;
		byte _oamAddr;
		byte[] _sprites;
		int[] _spriteIndicies;
		int _numSprites;

		/// <summary>
		/// Gets the current scanline number.
		/// </summary>
		/// <value>The current scanline number.</value>
		public int Scanline { get; private set; }

		/// <summary>
		/// Gets the cycle number on the current scanline.
		/// </summary>
		/// <value>The cycle number on the current scanline.</value>
		public int Cycle { get; private set; }

		// Base background nametable address
		ushort _baseNametableAddress;

		// Address of pattern table used for background
		ushort _bgPatternTableAddress;

		// Base sprite pattern table address
		ushort _spritePatternTableAddress;

		// Vram increment per write to PPUDATA
		int _vRamIncrement;

		// Last value written to a PPU register
		byte _lastRegisterWrite;

		// Sprite related flags
		byte _flagSpriteOverflow;
		byte _flagSpriteZeroHit;

		// PPUCTRL Register flags
		byte _flagBaseNametableAddr;
		byte _flagVRamIncrement;
		byte _flagSpritePatternTableAddr;
		byte _flagBgPatternTableAddr;
		byte _flagSpriteSize;
		byte _flagMasterSlaveSelect;
		byte _nmiOutput;

		// NMI Occurred flag
		byte _nmiOccurred;

		// PPUMASK Register flags
		byte _flagGreyscale;
		byte _flagShowBackgroundLeft;
		byte _flagShowSpritesLeft;
		byte _flagShowBackground;
		byte _flagShowSprites;
		byte _flagEmphasizeRed;
		byte _flagEmphasizeGreen;
		byte _flagEmphasizeBlue;

		// Internal PPU Registers
		ushort v; // Current VRAM address (15 bits)
		ushort t; // Temporary VRAM address (15 bits)
		byte x; // Fine X scroll (3 bits)
		byte w; // First or second write toggle (1 bit)
		byte f; // Even odd flag (even = 0, odd = 1)

		// Tile shift register and variables (latches) that feed it every 8 cycles
		ulong _tileShiftReg;
		byte _nameTableByte;
		byte _attributeTableByte;
		byte _tileBitfieldLo;
		byte _tileBitfieldHi;

		// PPUDATA buffer
		byte _ppuDataBuffer;

		/// <summary>
		/// Is <c>true</c> if rendering is currently enabled.
		/// </summary>
		/// <value><c>true</c> if rendering is enabled; otherwise, <c>false</c>.</value>
		public bool RenderingEnabled => _flagShowSprites != 0 || _flagShowBackground != 0;

		/// <summary>
		/// Constructs a new PPU.
		/// </summary>
		/// <param name="console">Console that this PPU is a part of</param>
		public Ppu(Console console)
		{
			_memory = console.PpuMemory;
			_console = console;

			BitmapData = new byte[256 * 240];
			NewBitmapData = new byte[256 * 240];

			_oam = new byte[256];
			_sprites = new byte[32];
			_spriteIndicies = new int[8];
		}

		/// <summary>
		/// Resets this PPU to its startup state.
		/// </summary>
		public void Reset()
		{
			Array.Clear(BitmapData, 0, BitmapData.Length);
			Array.Clear(NewBitmapData, 0, NewBitmapData.Length);

			Scanline = 240;
			Cycle = 340;

			_nmiOccurred = 0;
			_nmiOutput = 0;

			w = 0;
			f = 0;

			Array.Clear(_oam, 0, _oam.Length);
			Array.Clear(_sprites, 0, _sprites.Length);
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
			var xPos = Cycle - 1;

			return _flagShowBackground == 0 ? (byte)0 : _flagShowBackgroundLeft == 0 && xPos < 8 ? (byte)0 : (byte)((_tileShiftReg >> (x * 4)) & 0xF);
		}

		private byte GetSpritePixelData(out int spriteIndex)
		{
			var xPos = Cycle - 1;
			var yPos = Scanline - 1;

			spriteIndex = 0;

			if (_flagShowSprites == 0)
			{
				return 0;
			}

			if (_flagShowSpritesLeft == 0 && xPos < 8)
			{
				return 0;
			}

			// 8x8 sprites all come from the same pattern table as specified by a write to PPUCTRL
			// 8x16 sprites come from a pattern table defined in their OAM data
			var _currSpritePatternTableAddr = _spritePatternTableAddress;

			// Get sprite pattern bitfield
			for (var i = 0; i < _numSprites * 4; i += 4)
			{
				int spriteXLeft = _sprites[i + 3];
				var offset = xPos - spriteXLeft;

				if (offset <= 7 && offset >= 0)
				{
					// Found intersecting sprite
					var yOffset = yPos - _sprites[i];

					byte patternIndex;

					// Set the pattern table and index according to whether or not sprites
					// ar 8x8 or 8x16
					if (_flagSpriteSize == 1)
					{
						_currSpritePatternTableAddr = (ushort)((_sprites[i + 1] & 1) * 0x1000);
						patternIndex = (byte)(_sprites[i + 1] & 0xFE);
					}
					else
					{
						patternIndex = _sprites[i + 1];
					}

					var patternAddress = (ushort)(_currSpritePatternTableAddr + (patternIndex * 16));

					var flipHoriz = (_sprites[i + 2] & 0x40) != 0;
					var flipVert = (_sprites[i + 2] & 0x80) != 0;
					var colorNum = GetSpritePatternPixel(patternAddress, offset, yOffset, flipHoriz, flipVert);

					// Handle transparent sprites
					if (colorNum == 0)
					{
						continue;
					}
					else // Non transparent sprite, return data
					{
						var paletteNum = (byte)(_sprites[i + 2] & 0x03);
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
			v = (ushort)((v & 0x7BE0) | (t & 0x041F));
		}

		private void CopyVertPositionData()
		{
			// v: IHGF.ED CBA..... = t: IHGF.ED CBA.....
			v = (ushort)((v & 0x041F) | (t & 0x7BE0));
		}

		private int CoarseX()
		{
			return v & 0x1f;
		}

		private int CoarseY()
		{
			return (v >> 5) & 0x1f;
		}

		private int FineY()
		{
			return (v >> 12) & 0x7;
		}

		private int GetSpritePatternPixel(ushort patternAddr, int xPos, int yPos, bool flipHoriz = false, bool flipVert = false)
		{
			var h = _flagSpriteSize == 0 ? 7 : 15;

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
			if ((v & 0x001F) == 31)
			{
				v = (ushort)(v & (~0x001F)); // Reset Coarse X
				v = (ushort)(v ^ 0x0400); // Switch horizontal nametable
			}
			else
			{
				v++; // Increment Coarse X
			}
		}

		private void IncrementY()
		{
			if ((v & 0x7000) != 0x7000)
			{ // if fine Y < 7
				v += 0x1000; // increment fine Y
			}
			else
			{
				v = (ushort)(v & ~0x7000); // Set fine Y to 0
				var y = (v & 0x03E0) >> 5; // y = coarse Y
				if (y == 29)
				{
					y = 0; // coarse Y = 0
					v = (ushort)(v ^ 0x0800); // switch vertical nametable
				}
				else if (y == 31)
				{
					y = 0; // coarse Y = 0, nametable not switched
				}
				else
				{
					y += 1; // Increment coarse Y
				}
				v = (ushort)((v & ~0x03E0) | (y << 5)); // Put coarse Y back into v
			}
		}

		private void EvalSprites()
		{
			Array.Clear(_sprites, 0, _sprites.Length);
			Array.Clear(_spriteIndicies, 0, _spriteIndicies.Length);

			// 8x8 or 8x16 sprites
			var h = _flagSpriteSize == 0 ? 7 : 15;

			_numSprites = 0;
			var yPos = Scanline;

			// Sprite evaluation starts at the current OAM address and goes to the end of OAM (256 bytes)
			for (int i = _oamAddr; i < 256; i += 4)
			{
				var spriteYTop = _oam[i];

				var offset = yPos - spriteYTop;

				// If this sprite is on the next scanline, copy it to the _sprites array for rendering
				if (offset <= h && offset >= 0)
				{
					if (_numSprites == 8)
					{
						_flagSpriteOverflow = 1;
						break;
					}
					else
					{
						Array.Copy(_oam, i, _sprites, _numSprites * 4, 4);
						_spriteIndicies[_numSprites] = (i - _oamAddr) / 4;
						_numSprites++;

					}
				}
			}
		}

		private void RenderPixel()
		{
			// Get pixel data (4 bits of tile shift register as specified by x)
			var bgPixelData = GetBgPixelData();

			var spritePixelData = GetSpritePixelData(out var spriteScanlineIndex);
			var isSpriteZero = _flagSpriteZeroHit == 0 && _flagShowBackground == 1 && _spriteIndicies[spriteScanlineIndex] == 0;

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
						_flagSpriteZeroHit = 1;
					}

					// Get sprite priority
					var priority = (_sprites[(spriteScanlineIndex * 4) + 2] >> 5) & 1;
					color = priority == 1 ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
				}
			}

			BitmapData[(Scanline * 256) + (Cycle - 1)] = color;
			NewBitmapData[(Scanline * 256) + (Cycle - 1)] = color;
		}

		private void FetchNametableByte()
		{
			var address = (ushort)(0x2000 | (v & 0x0FFF));
			_nameTableByte = _memory.Read(address);
		}

		private void FetchAttributeTableByte()
		{
			var address = (ushort)(0x23C0 | (v & 0x0C00) | ((v >> 4) & 0x38) | ((v >> 2) & 0x07));
			_attributeTableByte = _memory.Read(address);
		}

		private void FetchTileBitfieldLo()
		{
			var address = (ushort)(_bgPatternTableAddress + (_nameTableByte * 16) + FineY());
			_tileBitfieldLo = _memory.Read(address);
		}

		private void FetchTileBitfieldHi()
		{
			var address = (ushort)(_bgPatternTableAddress + (_nameTableByte * 16) + FineY() + 8);
			_tileBitfieldHi = _memory.Read(address);
		}

		// Stores data for the next 8 pixels in the upper 32 bits of _tileShiftReg
		private void StoreTileData()
		{
			var _palette = (byte)((_attributeTableByte >> ((CoarseX() & 0x2) | ((CoarseY() & 0x2) << 1))) & 0x3);

			// Upper 32 bits to add to _tileShiftReg
			ulong data = 0;

			for (var i = 0; i < 8; i++)
			{
				// Get color number
				var loColorBit = (byte)((_tileBitfieldLo >> (7 - i)) & 1);
				var hiColorBit = (byte)((_tileBitfieldHi >> (7 - i)) & 1);
				var colorNum = (byte)((hiColorBit << 1) | ((loColorBit) & 0x03));

				// Add palette number
				var fullPixelData = (byte)(((_palette << 2) | colorNum) & 0xF);

				data |= (uint)(fullPixelData << (4 * i));
			}

			_tileShiftReg &= 0xFFFFFFFF;
			_tileShiftReg |= data << 32;
		}

		// Updates scanline and cycle counters, triggers NMI's if needed.
		private void UpdateCounters()
		{
			// Trigger an NMI at the start of _scanline 241 if VBLANK NMI's are enabled
			if (Scanline == 241 && Cycle == 1)
			{
				_nmiOccurred = 1;
				if (_nmiOutput != 0)
				{
					_console.Cpu.TriggerNmi();
				}
			}

			var renderingEnabled = (_flagShowBackground != 0) || (_flagShowSprites != 0);

			// Skip last cycle of prerender scanline on odd frames
			if (renderingEnabled)
			{
				if (Scanline == 261 && f == 1 && Cycle == 339)
				{
					f ^= 1;
					Scanline = 0;
					Cycle = -1;
					_console.DrawFrame();
					return;
				}
			}
			Cycle++;

			// Reset cycle (and scanline if scanline == 260)
			// Also set to next frame if at end of last _scanline
			if (Cycle > 340)
			{
				if (Scanline == 261) // Last scanline, reset to upper left corner
				{
					f ^= 1;
					Scanline = 0;
					Cycle = -1;
					_console.DrawFrame();
				}
				else // Not on last scanline
				{
					Cycle = -1;
					Scanline++;
				}
			}
		}

		/// <summary>
		/// Executes a single PPU step.
		/// </summary>
		public void Step()
		{
			UpdateCounters();

			// Cycle types
			var renderingEnabled = (_flagShowBackground != 0) || (_flagShowSprites != 0);
			var renderCycle = Cycle > 0 && Cycle <= 256;
			var preFetchCycle = Cycle >= 321 && Cycle <= 336;
			var fetchCycle = renderCycle || preFetchCycle;

			// Scanline types
			var renderScanline = Scanline >= 0 && Scanline < 240;
			var idleScanline = Scanline == 240;
			var vBlankScanline = Scanline > 240;
			var preRenderScanline = Scanline == 261;

			// nmiOccurred flag cleared on prerender scanline at cycle 1
			if (preRenderScanline && Cycle == 1)
			{
				_nmiOccurred = 0;
				_flagSpriteOverflow = 0;
				_flagSpriteZeroHit = 0;
			}

			if (renderingEnabled)
			{
				// Evaluate sprites at cycle 257 of each render scanline
				if (Cycle == 257)
				{
					if (renderScanline)
					{
						EvalSprites();
					}
					else
					{
						_numSprites = 0;
					}
				}

				if (renderCycle && renderScanline)
				{
					RenderPixel();
				}

				// Read rendering data into internal latches and update _tileShiftReg
				// with those latches every 8 cycles
				// https://wiki.nesdev.com/w/images/d/d1/Ntsc_timing.png
				if (fetchCycle && (renderScanline || preRenderScanline))
				{
					_tileShiftReg >>= 4;
					switch (Cycle % 8)
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
							if (Cycle == 256)
							{
								IncrementY();
							}

							break;
					}
				}

				// OAMADDR is set to 0 during each of ticks 257-320 (the sprite tile loading interval) of the pre-render and visible scanlines. 
				if (Cycle > 257 && Cycle <= 320 && (preRenderScanline || renderScanline))
				{
					_oamAddr = 0;
				}

				// Copy horizontal position data from t to v on _cycle 257 of each scanline if rendering enabled
				if (Cycle == 257 && (renderScanline || preRenderScanline))
				{
					CopyHorizPositionData();
				}

				// Copy vertical position data from t to v repeatedly from cycle 280 to 304 (if rendering is enabled)
				if (Cycle >= 280 && Cycle <= 304 && Scanline == 261)
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
			_lastRegisterWrite = data;
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
			_flagBaseNametableAddr = (byte)(data & 0x3);
			_flagVRamIncrement = (byte)((data >> 2) & 1);
			_flagSpritePatternTableAddr = (byte)((data >> 3) & 1);
			_flagBgPatternTableAddr = (byte)((data >> 4) & 1);
			_flagSpriteSize = (byte)((data >> 5) & 1);
			_flagMasterSlaveSelect = (byte)((data >> 6) & 1);
			_nmiOutput = (byte)((data >> 7) & 1);

			// Set values based off flags
			_baseNametableAddress = (ushort)(0x2000 + (0x400 * _flagBaseNametableAddr));
			_vRamIncrement = (_flagVRamIncrement == 0) ? 1 : 32;
			_bgPatternTableAddress = (ushort)(_flagBgPatternTableAddr == 0 ? 0x0000 : 0x1000);
			_spritePatternTableAddress = (ushort)(0x1000 * _flagSpritePatternTableAddr);

			// t: ...BA.. ........ = d: ......BA
			t = (ushort)((t & 0xF3FF) | ((data & 0x03) << 10));
		}

		// $2001
		private void WritePpuMask(byte data)
		{
			_flagGreyscale = (byte)(data & 1);
			_flagShowBackgroundLeft = (byte)((data >> 1) & 1);
			_flagShowSpritesLeft = (byte)((data >> 2) & 1);
			_flagShowBackground = (byte)((data >> 3) & 1);
			_flagShowSprites = (byte)((data >> 4) & 1);
			_flagEmphasizeRed = (byte)((data >> 5) & 1);
			_flagEmphasizeGreen = (byte)((data >> 6) & 1);
			_flagEmphasizeBlue = (byte)((data >> 7) & 1);
		}

		// $4014
		private void WriteOamAddr(byte data)
		{
			_oamAddr = data;
		}

		// $2004
		private void WriteOamData(byte data)
		{
			_oam[_oamAddr] = data;
			_oamAddr++;
		}

		// $2005
		private void WritePpuScroll(byte data)
		{
			if (w == 0) // First write
			{
				// t: ....... ...HGFED = d: HGFED...
				// x:              CBA = d: .....CBA
				// w:                  = 1
				t = (ushort)((t & 0xFFE0) | (data >> 3));
				x = (byte)(data & 0x07);
				w = 1;
			}
			else
			{
				// t: CBA..HG FED..... = d: HGFEDCBA
				// w:                  = 0
				t = (ushort)(t & 0xC1F);
				t |= (ushort)((data & 0x07) << 12); // CBA
				t |= (ushort)((data & 0xF8) << 2); // HG FED
				w = 0;
			}
		}

		// $2006
		private void WritePpuAddr(byte data)
		{
			if (w == 0)  // First write
			{
				// t: .FEDCBA ........ = d: ..FEDCBA
				// t: X...... ........ = 0
				// w:                  = 1
				t = (ushort)((t & 0x00FF) | (data << 8));
				w = 1;
			}
			else
			{
				// t: ....... HGFEDCBA = d: HGFEDCBA
				// v                   = t
				// w:                  = 0
				t = (ushort)((t & 0xFF00) | data);
				v = t;
				w = 0;
			}
		}

		// $2007
		private void WritePpuData(byte data)
		{
			_memory.Write(v, data);
			v += (ushort)_vRamIncrement;
		}

		// $4014
		private void WriteOamDma(byte data)
		{
			var startAddr = (ushort)(data << 8);
			_console.CpuMemory.ReadBufWrapping(_oam, _oamAddr, startAddr, 256);

			// OAM DMA always takes at least 513 CPU cycles
			_console.Cpu.AddIdleCycles(513);

			// OAM DMA takes an extra CPU cycle if executed on an odd CPU cycle
			if (_console.Cpu.Cycles % 2 == 1)
			{
				_console.Cpu.AddIdleCycles(1);
			}
		}

		// $2002
		private byte ReadPpuStatus()
		{
			byte retVal = 0;
			retVal |= (byte)(_lastRegisterWrite & 0x1F); // Least signifigant 5 bits of last register write
			retVal |= (byte)(_flagSpriteOverflow << 5);
			retVal |= (byte)(_flagSpriteZeroHit << 6);
			retVal |= (byte)(_nmiOccurred << 7);

			_nmiOccurred = 0;
			w = 0;
			return retVal;
		}

		// $2004
		private byte ReadOamData()
		{
			return _oam[_oamAddr];
		}

		// $2007
		private byte ReadPpuData()
		{
			var data = _memory.Read(v);

			// Buffered read emulation
			// https://wiki.nesdev.com/w/index.php/PPU_registers#The_PPUDATA_read_buffer_.28post-fetch.29
			if (v < 0x3F00)
			{
				var bufferedData = _ppuDataBuffer;
				_ppuDataBuffer = data;
				data = bufferedData;
			}
			else
			{
				_ppuDataBuffer = _memory.Read((ushort)(v - 0x1000));
			}

			v += (ushort)_vRamIncrement;
			return data;
		}

		#region Save/Load state

		[Serializable]
		public class PpuState
		{
			public byte[] _oam;
			public byte _oamAddr;
			public byte[] _sprites;
			public int[] _spriteIndicies;
			public int _numSprites;
			public int Scanline;
			public int Cycle;
			public ushort _baseNametableAddress;
			public ushort _bgPatternTableAddress;
			public ushort _spritePatternTableAddress;
			public int _vRamIncrement;
			public byte _lastRegisterWrite;
			public byte _flagSpriteOverflow;
			public byte _flagSpriteZeroHit;
			public byte _flagBaseNametableAddr;
			public byte _flagVRamIncrement;
			public byte _flagSpritePatternTableAddr;
			public byte _flagBgPatternTableAddr;
			public byte _flagSpriteSize;
			public byte _flagMasterSlaveSelect;
			public byte _nmiOutput;
			public byte _nmiOccurred;
			public byte _flagGreyscale;
			public byte _flagShowBackgroundLeft;
			public byte _flagShowSpritesLeft;
			public byte _flagShowBackground;
			public byte _flagShowSprites;
			public byte _flagEmphasizeRed;
			public byte _flagEmphasizeGreen;
			public byte _flagEmphasizeBlue;
			public ushort v;
			public ushort t;
			public byte x;
			public byte w;
			public byte f;
			public ulong _tileShiftReg;
			public byte _nameTableByte;
			public byte _attributeTableByte;
			public byte _tileBitfieldLo;
			public byte _tileBitfieldHi;
			public byte _ppuDataBuffer;
			public object PpuMemory;
		}

		public object SaveState()
		{
			var state = new PpuState();

			state._oam = _oam;
			state._oamAddr = _oamAddr;
			state._sprites = _sprites;
			state._spriteIndicies = _spriteIndicies;
			state._numSprites = _numSprites;
			state.Scanline = Scanline;
			state.Cycle = Cycle;
			state._baseNametableAddress = _baseNametableAddress;
			state._bgPatternTableAddress = _bgPatternTableAddress;
			state._spritePatternTableAddress = _spritePatternTableAddress;
			state._vRamIncrement = _vRamIncrement;
			state._lastRegisterWrite = _lastRegisterWrite;
			state._flagSpriteOverflow = _flagSpriteOverflow;
			state._flagSpriteZeroHit = _flagSpriteZeroHit;
			state._flagBaseNametableAddr = _flagBaseNametableAddr;
			state._flagVRamIncrement = _flagVRamIncrement;
			state._flagSpritePatternTableAddr = _flagSpritePatternTableAddr;
			state._flagBgPatternTableAddr = _flagBgPatternTableAddr;
			state._flagSpriteSize = _flagSpriteSize;
			state._flagMasterSlaveSelect = _flagMasterSlaveSelect;
			state._nmiOutput = _nmiOutput;
			state._nmiOccurred = _nmiOccurred;
			state._flagGreyscale = _flagGreyscale;
			state._flagShowBackgroundLeft = _flagShowBackgroundLeft;
			state._flagShowSpritesLeft = _flagShowSpritesLeft;
			state._flagShowBackground = _flagShowBackground;
			state._flagShowSprites = _flagShowSprites;
			state._flagEmphasizeRed = _flagEmphasizeRed;
			state._flagEmphasizeGreen = _flagEmphasizeGreen;
			state._flagEmphasizeBlue = _flagEmphasizeBlue;
			state.v = v;
			state.t = t;
			state.x = x;
			state.w = w;
			state.f = f;
			state._tileShiftReg = _tileShiftReg;
			state._nameTableByte = _nameTableByte;
			state._attributeTableByte = _attributeTableByte;
			state._tileBitfieldLo = _tileBitfieldLo;
			state._tileBitfieldHi = _tileBitfieldHi;
			state._ppuDataBuffer = _ppuDataBuffer;
			state.PpuMemory = _memory.SaveState();

			return state;
		}

		public void LoadState(object stateObj)
		{
			var state = stateObj as PpuState;
			_oam = state._oam;
			_oamAddr = state._oamAddr;
			_sprites = state._sprites;
			_spriteIndicies = state._spriteIndicies;
			_numSprites = state._numSprites;
			Scanline = state.Scanline;
			Cycle = state.Cycle;
			_baseNametableAddress = state._baseNametableAddress;
			_bgPatternTableAddress = state._bgPatternTableAddress;
			_spritePatternTableAddress = state._spritePatternTableAddress;
			_vRamIncrement = state._vRamIncrement;
			_lastRegisterWrite = state._lastRegisterWrite;
			_flagSpriteOverflow = state._flagSpriteOverflow;
			_flagSpriteZeroHit = state._flagSpriteZeroHit;
			_flagBaseNametableAddr = state._flagBaseNametableAddr;
			_flagVRamIncrement = state._flagVRamIncrement;
			_flagSpritePatternTableAddr = state._flagSpritePatternTableAddr;
			_flagBgPatternTableAddr = state._flagBgPatternTableAddr;
			_flagSpriteSize = state._flagSpriteSize;
			_flagMasterSlaveSelect = state._flagMasterSlaveSelect;
			_nmiOutput = state._nmiOutput;
			_nmiOccurred = state._nmiOccurred;
			_flagGreyscale = state._flagGreyscale;
			_flagShowBackgroundLeft = state._flagShowBackgroundLeft;
			_flagShowSpritesLeft = state._flagShowSpritesLeft;
			_flagShowBackground = state._flagShowBackground;
			_flagShowSprites = state._flagShowSprites;
			_flagEmphasizeRed = state._flagEmphasizeRed;
			_flagEmphasizeGreen = state._flagEmphasizeGreen;
			_flagEmphasizeBlue = state._flagEmphasizeBlue;
			v = state.v;
			t = state.t;
			x = state.x;
			w = state.w;
			f = state.f;
			_tileShiftReg = state._tileShiftReg;
			_nameTableByte = state._nameTableByte;
			_attributeTableByte = state._attributeTableByte;
			_tileBitfieldLo = state._tileBitfieldLo;
			_tileBitfieldHi = state._tileBitfieldHi;
			_ppuDataBuffer = state._ppuDataBuffer;

			_memory.LoadState(state.PpuMemory);
		}

		#endregion
	}
}