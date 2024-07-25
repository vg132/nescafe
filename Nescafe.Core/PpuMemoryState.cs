namespace Nescafe.Core;

[Serializable]
public class PpuMemoryState
{
	public byte[] VRam = new byte[2048];
	public byte[] PaletteRam= new byte[32];
}
