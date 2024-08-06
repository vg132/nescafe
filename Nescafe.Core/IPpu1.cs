namespace Nescafe.Core;

public interface IPpu
{
	byte[] BitmapData { get; }
	bool RenderingEnabled { get; }
	PpuState State { get; }

	void LoadState(object stateObj);
	byte ReadFromRegister(ushort address);
	void Reset();
	object SaveState();
	void Step();
	void WriteToRegister(ushort address, byte data);
}