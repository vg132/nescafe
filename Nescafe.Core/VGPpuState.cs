namespace Nescafe.Core;

[Serializable]
public class VGPpuState : PpuState
{
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