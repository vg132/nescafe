using System.Reflection;

namespace Nescafe.Core;
public partial class Cpu
{
	private class CPUInstruction
	{
		public CPUInstruction(CpuInstructionAttribute cpuInstructionAttribute, MethodInfo method, Cpu cpu)
		{
			Id = cpuInstructionAttribute.Id;
			AddressMode = cpuInstructionAttribute.AddressMode;
			InstructionSize = cpuInstructionAttribute.InstructionSize;
			InstructionCycles = cpuInstructionAttribute.InstructionCycles;
			InstructionPageCycles = cpuInstructionAttribute.InstructionPageCycles;
			Method = method;
			Cpu = cpu;
			_opCodeParameters[0] = AddressMode;
		}

		public byte Id { get; set; }
		public AddressMode AddressMode { get; set; }
		public int InstructionSize { get; set; }
		public int InstructionCycles { get; set; }
		public int InstructionPageCycles { get; set; }
		public MethodInfo Method { get; }
		public Cpu Cpu { get; }
		private object[] _opCodeParameters = new object[2];

		public string Name => Method.Name;

		public void InvokeOpCode(ushort address)
		{
			_opCodeParameters[1] = address;
			Method.Invoke(Cpu, _opCodeParameters);
		}
	}
}