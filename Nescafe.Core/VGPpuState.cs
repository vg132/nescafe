namespace Nescafe.Core;

[Serializable]
public class VGPpuState : PpuState
{
	private byte _ppuControl;
	private byte _ppuMask;

	public long FrameCounter { get; set; } = 0;
	public bool IsEvenFrame => (FrameCounter % 2) == 0;

	public byte PpuStatus
	{
		get
		{
			byte retVal = 0;
			retVal |= (byte)(LastRegisterWrite & 0x1F); // Least signifigant 5 bits of last register write
			retVal |= (byte)((FlagSpriteOverflow.AsByte()) << 5);
			retVal |= (byte)((FlagSpriteZeroHit.AsByte()) << 6);
			retVal |= (byte)((NmiOccurred.AsByte()) << 7);
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
				FlagBaseNametableAddr = (byte)(value & 0x3);
				FlagVRamIncrement = (byte)((value >> 2) & 1);
				FlagSpritePatternTableAddr = (byte)((value >> 3) & 1);
				FlagBgPatternTableAddr = (byte)((value >> 4) & 1);
				FlagSpriteSize = (byte)((value >> 5) & 1);
				FlagMasterSlaveSelect = (byte)((value >> 6) & 1);
				NmiOutput = (byte)((value >> 7) & 1);
			}
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
				FlagGreyscale = (byte)(value & 1);
				FlagShowBackgroundLeft = (byte)((value >> 1) & 1);
				FlagShowSpritesLeft = (byte)((value >> 2) & 1);
				FlagShowBackground = (byte)((value >> 3) & 1);
				FlagShowSprites = (byte)((value >> 4) & 1);
				FlagEmphasizeRed = (byte)((value >> 5) & 1);
				FlagEmphasizeGreen = (byte)((value >> 6) & 1);
				FlagEmphasizeBlue = (byte)((value >> 7) & 1);
			}
		}
	}
}