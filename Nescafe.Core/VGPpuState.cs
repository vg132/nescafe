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

	public bool FlagSpriteOverflow;
	public bool FlagSpriteZeroHit;

	public byte FlagBaseNametableAddr;
	public byte FlagVRamIncrement;
	public byte FlagSpritePatternTableAddr;
	public byte FlagBgPatternTableAddr;
	public byte FlagSpriteSize;
	public byte FlagMasterSlaveSelect;
	public byte NmiOutput;

	public bool NmiOccurred;

	public byte FlagGreyscale;
	public byte FlagShowBackgroundLeft;
	public byte FlagShowSpritesLeft;
	public byte FlagShowBackground;
	public byte FlagShowSprites;
	public byte FlagEmphasizeRed;
	public byte FlagEmphasizeGreen;
	public byte FlagEmphasizeBlue;
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


	// NEW

	public long PpuCalls = 0;
	public long CpuCalls = 0;














	public void Reset()
	{
		Scanline = -1;
		Cycle = 0;

		VBlankStarted = false;
		FlagSpriteOverflow = false;
		FlagSpriteZeroHit = false;

		TriggerNmi = false;
		NmiTriggered = false;

		NmiOutput = 0;
		FrameCounter = 0;
		PpuControl = 0;
		PpuMask = 0;

		LastRegisterWrite = 0;
		NameTableByte = 0;
		BaseNametableAddress = 0;
		AttributeTableByte = 0;
		VRamIncrement = 1;
		FlagVRamIncrement = 0;
		TileBitfieldHi = 0;
		TileBitfieldLo = 0;
		TileShiftReg = 0;

		T = 0;
		F = 0;
		V = 0;
		W = 0;
		X = 0;

		Array.Clear(Oam, 0, Oam.Length);
		Array.Clear(Sprites, 0, Sprites.Length);
	}

	// New flags
	public bool VBlankStarted;
	public bool ShowBackground;
	public bool ShowBackgroundLeft;
	public bool ShowSpritesLeft;
	public bool ShowSprites;
	public bool EmphasizeRed;
	public bool EmphasizeGreen;
	public bool EmphasizeBlue;
	public bool Greyscale;

	public bool TriggerNmi;
	public bool NmiTriggered;

	public bool FlagLargeSprites;

	private byte _ppuControl;
	private byte _ppuMask;

	public long FrameCounter = 0;
	public bool IsEvenFrame => (FrameCounter % 2) == 0;

	public byte PpuStatus
	{
		get
		{
			byte retVal = 0;
			retVal |= (byte)(LastRegisterWrite & 0x1F); // Least signifigant 5 bits of last register write
			retVal |= (byte)((FlagSpriteOverflow.AsByte()) << 5);
			retVal |= (byte)((FlagSpriteZeroHit.AsByte()) << 6);
			retVal |= (byte)((VBlankStarted.AsByte()) << 7);
			return retVal;
		}
	}

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
				FlagSpriteSize = (byte)((value >> 5) & 1);
				FlagLargeSprites = (byte)((value >> 5) & 1) != 0;
				FlagMasterSlaveSelect = (byte)((value >> 6) & 1);
				TriggerNmi = (byte)((value >> 7) & 1) != 0;
				NmiTriggered = false;
			}
			T = (ushort)((T & 0xF3FF) | ((value & 0x03) << 10));
		}
	}


	public byte PpuMask
	{
		get => _ppuMask;
		set
		{
			if (_ppuMask != value)
			{
				_ppuMask = value;
				Greyscale = (byte)(value & 1) != 0;
				ShowBackgroundLeft = (byte)((value >> 1) & 1) != 0;
				ShowSpritesLeft = (byte)((value >> 2) & 1) != 0;
				ShowBackground = (byte)((value >> 3) & 1) != 0;
				ShowSprites = (byte)((value >> 4) & 1) != 0;
				EmphasizeRed = (byte)((value >> 5) & 1) != 0;
				EmphasizeGreen = (byte)((value >> 6) & 1) != 0;
				EmphasizeBlue = (byte)((value >> 7) & 1) != 0;
			}
		}
	}
}