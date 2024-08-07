namespace Nescafe.Core;

[Serializable]
public class CpuState
{
	// Registers
	public byte A;    // Accumulator
	public byte X;
	public byte Y;
	public byte S;    // Stack Pointer
	public ushort PC; // Program Counter (16 bits)

	// Status flag register (implemented as several booleans)
	public bool C; // Carry flag
	public bool Z; // Zero flag
	public bool I; // Interrpt Disable
	public bool D; // Decimal Mode (Not used)
	public bool B; // Break command
	public bool V; // Overflow flag
	public bool N; // Negative flag

	// Interrupts
	public bool IrqInterrupt;
	public bool NmiInterrupt;

	// If positive, idle 1 cycle and deincrement each step
	//public int Idle;

	public int Cycles;
}