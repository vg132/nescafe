namespace Nescafe.Core;

[Serializable]
public class PpuState
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
}