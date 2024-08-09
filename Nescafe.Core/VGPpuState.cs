namespace Nescafe.Core;

[Serializable]
public class VGPpuState
{
	public byte[] Oam;
	public byte OamAddr;
	public byte[] Sprites;
	public int[] SpriteIndicies;
	public int NumSprites;
	public int Scanline;
	public int Cycle;
	public ushort BaseNametableAddress;
	public ushort BgPatternTableAddress;
	public ushort SpritePatternTableAddress;
	public int VRamIncrement;
	public byte LastRegisterWrite;

	// Flags
	public bool FlagSpriteOverflow;
	public bool FlagSpriteZeroHit;
	public bool FlagVBlankStarted;
	public bool FlagShowBackground;
	public bool FlagShowBackgroundLeft;
	public bool FlagShowSpritesLeft;
	public bool FlagShowSprites;
	public bool FlagEmphasizeRed;
	public bool FlagEmphasizeGreen;
	public bool FlagEmphasizeBlue;
	public bool FlagGreyscale;
	public bool FlagMasterSlaveSelect;
	public bool FlagTriggerNmi;
	public bool FlagNmiTriggered;
	public bool FlagLargeSprites;

	// Registers
	public ushort V;
	public ushort T;
	public byte X;
	public byte W;
	public byte F;
	public ulong TileShiftReg;
	public byte NameTableByte;
	public byte AttributeTableByte;
	public byte TileBitfieldLo;
	public byte TileBitfieldHi;
	public byte PpuDataBuffer;

	// Counters
	public long PpuCallCount = 0;
	public long CpuCallCount = 0;
	public long FrameCounter = 0;

	public bool IsEvenFrame => (FrameCounter % 2) == 0;

	public void Reset()
	{
		OamAddr = 0;
		NumSprites = 0;
		Scanline = -1;
		Cycle = 0;
		BaseNametableAddress = 0;
		BgPatternTableAddress = 0;
		SpritePatternTableAddress = 0;
		VRamIncrement = 0;
		LastRegisterWrite = 0;

		// Flags
		FlagSpriteOverflow = false;
		FlagSpriteZeroHit = false;
		FlagVBlankStarted = false;
		FlagShowBackground = false;
		FlagShowBackgroundLeft = false;
		FlagShowSpritesLeft = false;
		FlagShowSprites = false;
		FlagEmphasizeRed = false;
		FlagEmphasizeGreen = false;
		FlagEmphasizeBlue = false;
		FlagGreyscale = false;
		FlagMasterSlaveSelect = false;
		FlagTriggerNmi = false;
		FlagNmiTriggered = false;
		FlagLargeSprites = false;

		// Registers
		V = 0;
		T = 0;
		X = 0;
		W = 0;
		F = 0;
		TileShiftReg = 0;
		NameTableByte = 0;
		AttributeTableByte = 0;
		TileBitfieldLo = 0;
		TileBitfieldHi = 0;
		PpuDataBuffer = 0;

		// Counters
		PpuCallCount = 0;
		CpuCallCount = 0;
		FrameCounter = 0;

		Array.Clear(Oam, 0, Oam.Length);
		Array.Clear(Sprites, 0, Sprites.Length);
		Array.Clear(SpriteIndicies, 0, SpriteIndicies.Length);
	}

	public byte PpuStatus
	{
		get
		{
			byte retVal = 0;
			retVal |= (byte)(LastRegisterWrite & 0x1F); // Least signifigant 5 bits of last register write
			retVal |= (byte)((FlagSpriteOverflow.AsByte()) << 5);
			retVal |= (byte)((FlagSpriteZeroHit.AsByte()) << 6);
			retVal |= (byte)((FlagVBlankStarted.AsByte()) << 7);
			return retVal;
		}
	}

	private byte _ppuControl;
	public byte PpuControl
	{
		get => _ppuControl;
		set
		{
			if (_ppuControl != value)
			{
				_ppuControl = value;

				BaseNametableAddress = (ushort)(0x2000 + (0x400 * (byte)(value & 0x3)));
				VRamIncrement = (byte)((value >> 2) & 1) == 0 ? 1 : 32;
				SpritePatternTableAddress = (ushort)(0x1000 * (byte)((value >> 3) & 1));
				BgPatternTableAddress = (ushort)(((value >> 4) & 1) == 0 ? 0x00 : 0x1000);
				FlagLargeSprites = (byte)((value >> 5) & 1) != 0;
				FlagMasterSlaveSelect = (byte)((value >> 6) & 1) != 0;
				FlagTriggerNmi = (byte)((value >> 7) & 1) != 0;
				FlagNmiTriggered = false;
			}
			T = (ushort)((T & 0xF3FF) | ((value & 0x03) << 10));
		}
	}

	private byte _ppuMask;
	public byte PpuMask
	{
		get => _ppuMask;
		set
		{
			if (_ppuMask != value)
			{
				_ppuMask = value;
				FlagGreyscale = (byte)(value & 1) != 0;
				FlagShowBackgroundLeft = (byte)((value >> 1) & 1) != 0;
				FlagShowSpritesLeft = (byte)((value >> 2) & 1) != 0;
				FlagShowBackground = (byte)((value >> 3) & 1) != 0;
				FlagShowSprites = (byte)((value >> 4) & 1) != 0;
				FlagEmphasizeRed = (byte)((value >> 5) & 1) != 0;
				FlagEmphasizeGreen = (byte)((value >> 6) & 1) != 0;
				FlagEmphasizeBlue = (byte)((value >> 7) & 1) != 0;
			}
		}
	}
}