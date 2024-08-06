namespace Nescafe.Core;

public partial class Cpu
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	private class CpuInstructionAttribute : Attribute
	{
		private readonly byte _id;
		private readonly Cpu.AddressMode _addressMode;
		private readonly int _instructionSize;
		private readonly int _instructionCycles;
		private readonly int _instructionPageCycles;

		public CpuInstructionAttribute(byte id, Cpu.AddressMode addressMode, int instructionSize, int instructionCycles, int instructionPageCycles)
		{
			_id = id;
			_addressMode = addressMode;
			_instructionSize = instructionSize;
			_instructionCycles = instructionCycles;
			_instructionPageCycles = instructionPageCycles;
		}

		public byte Id => _id;
		public Cpu.AddressMode AddressMode => _addressMode;
		public int InstructionSize => _instructionSize;
		public int InstructionCycles => _instructionCycles;
		public int InstructionPageCycles => _instructionPageCycles;
	}
}