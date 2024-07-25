using System.Reflection;

namespace Nescafe.Core;

public partial class Cpu
{
	#region Private class definitions

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

	private class CPUInstruction
	{
		public CPUInstruction(CpuInstructionAttribute cpuInstructionAttribute, Delegate instruction)
		{
			Id = cpuInstructionAttribute.Id;
			AddressMode = cpuInstructionAttribute.AddressMode;
			InstructionSize = cpuInstructionAttribute.InstructionSize;
			InstructionCycles = cpuInstructionAttribute.InstructionCycles;
			InstructionPageCycles = cpuInstructionAttribute.InstructionPageCycles;
			Instruction = instruction;
		}

		public byte Id { get; set; }
		public AddressMode AddressMode { get; set; }
		public int InstructionSize { get; set; }
		public int InstructionCycles { get; set; }
		public int InstructionPageCycles { get; set; }
		private Delegate Instruction { get; set; }

		public void Invoke(ushort address)
		{
			Instruction.DynamicInvoke(new object[] { AddressMode, address });
		}
	}

	#endregion

	private readonly IDictionary<byte, CPUInstruction> _cpuInstructions = new Dictionary<byte, CPUInstruction>();

	private void InitializeOpcodes()
	{
		var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(item => item.GetCustomAttributes<CpuInstructionAttribute>(false).Any());
		foreach (var method in methods)
		{
			var invokeMethod = Delegate.CreateDelegate(typeof(Action<AddressMode, ushort>), this, method.Name);
			foreach (var attribute in method.GetCustomAttributes<CpuInstructionAttribute>(false))
			{
				_cpuInstructions.Add(attribute.Id, new CPUInstruction(attribute, invokeMethod));
			}
		}
	}

	// Illegal opcode, throw exception
	[CpuInstruction(2, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(18, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(34, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(50, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(66, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(82, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(98, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(114, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(146, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(178, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(210, AddressMode.Implied, 0, 2, 0)]
	[CpuInstruction(242, AddressMode.Implied, 0, 2, 0)]
	private void ___(AddressMode mode, ushort address)
	{
		throw new Exception("Illegal Opcode");
	}

	// INSTRUCTIONS FOLLOW
	// BRK - Force Interrupt
	[CpuInstruction(0, AddressMode.Implied, 1, 7, 0)]
	private void brk(AddressMode mode, ushort address)
	{
		PushStack16(State.PC);
		PushStack(GetStatusFlags());
		State.B = true;
		State.PC = _memory.Read16(0xFFFE);
	}

	// ROR - Rotate Right
	[CpuInstruction(102, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(106, AddressMode.Accumulator, 1, 2, 0)]
	[CpuInstruction(110, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(118, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(126, AddressMode.AbsoluteX, 3, 7, 0)]
	private void ror(AddressMode mode, ushort address)
	{
		var Corig = State.C;
		if (mode == AddressMode.Accumulator)
		{
			State.C = IsBitSet(State.A, 0);
			State.A >>= 1;
			State.A |= (byte)(Corig ? 0x80 : 0);

			SetZn(State.A);
		}
		else
		{
			var data = _memory.Read(address);
			State.C = IsBitSet(data, 0);

			data >>= 1;
			data |= (byte)(Corig ? 0x80 : 0);

			_memory.Write(address, data);

			SetZn(data);
		}
	}

	// RTI - Return from Interrupt
	[CpuInstruction(64, AddressMode.Implied, 1, 6, 0)]
	private void rti(AddressMode mode, ushort address)
	{
		SetProcessorFlags(PullStack());
		State.PC = PullStack16();
	}

	// TXS - Transfer X to Stack Pointer
	[CpuInstruction(154, AddressMode.Implied, 1, 2, 0)]
	private void txs(AddressMode mode, ushort address)
	{
		State.S = State.X;
	}

	// TSX - Transfer Stack Pointer to X
	[CpuInstruction(186, AddressMode.Implied, 1, 2, 0)]
	private void tsx(AddressMode mode, ushort address)
	{
		State.X = State.S;
		SetZn(State.X);
	}

	// TXA - Transfer X to Accumulator
	[CpuInstruction(138, AddressMode.Implied, 1, 2, 0)]
	private void txa(AddressMode mode, ushort address)
	{
		State.A = State.X;
		SetZn(State.A);
	}

	// TYA - Transfer Y to Accumulator
	[CpuInstruction(152, AddressMode.Implied, 1, 2, 0)]
	private void tya(AddressMode mode, ushort address)
	{
		State.A = State.Y;
		SetZn(State.A);
	}

	// TAY - Transfer Accumulator to Y
	[CpuInstruction(168, AddressMode.Implied, 1, 2, 0)]
	private void tay(AddressMode mode, ushort address)
	{
		State.Y = State.A;
		SetZn(State.Y);
	}

	// TAX  - Transfer Accumulator to X
	[CpuInstruction(170, AddressMode.Implied, 1, 2, 0)]
	private void tax(AddressMode mode, ushort address)
	{
		State.X = State.A;
		SetZn(State.X);
	}

	// DEX - Deincrement X
	[CpuInstruction(202, AddressMode.Implied, 1, 2, 0)]
	private void dex(AddressMode mode, ushort address)
	{
		State.X--;
		SetZn(State.X);
	}

	// DEY - Deincrement Y
	[CpuInstruction(136, AddressMode.Implied, 1, 2, 0)]
	private void dey(AddressMode mode, ushort address)
	{
		State.Y--;
		SetZn(State.Y);
	}

	// INX - Increment X
	[CpuInstruction(232, AddressMode.Implied, 1, 2, 0)]
	private void inx(AddressMode mode, ushort address)
	{
		State.X++;
		SetZn(State.X);
	}

	// INY - Increment Y
	[CpuInstruction(200, AddressMode.Implied, 1, 2, 0)]
	private void iny(AddressMode mode, ushort address)
	{
		State.Y++;
		SetZn(State.Y);
	}

	// STY - Store Y Register
	[CpuInstruction(132, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(140, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(148, AddressMode.ZeroPageX, 2, 4, 0)]
	private void sty(AddressMode mode, ushort address)
	{
		_memory.Write(address, State.Y);
	}

	// CPX - Compare X Register
	[CpuInstruction(224, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(228, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(236, AddressMode.Absolute, 3, 4, 0)]
	private void cpx(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		SetZn((byte)(State.X - data));
		State.C = State.X >= data;
	}

	// CPX - Compare Y Register
	[CpuInstruction(192, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(196, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(204, AddressMode.Absolute, 3, 4, 0)]
	private void cpy(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		SetZn((byte)(State.Y - data));
		State.C = State.Y >= data;
	}

	// SBC - Subtract with Carry
	[CpuInstruction(225, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(229, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(233, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(235, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(237, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(241, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(245, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(249, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(253, AddressMode.AbsoluteX, 3, 4, 1)]
	private void sbc(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		var notCarry = !State.C ? 1 : 0;

		var result = (byte)(State.A - data - notCarry);
		SetZn(result);

		// If an overflow occurs (result actually less than 0)
		// the carry flag is cleared
		State.C = (State.A - data - notCarry) >= 0 ? true : false;

		State.V = ((State.A ^ data) & (State.A ^ result) & 0x80) != 0;

		State.A = result;
	}

	// ADC - Add with Carry
	[CpuInstruction(97, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(101, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(105, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(109, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(113, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(117, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(121, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(125, AddressMode.AbsoluteX, 3, 4, 1)]
	private void adc(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		var carry = State.C ? 1 : 0;

		var sum = (byte)(State.A + data + carry);
		SetZn(sum);

		State.C = (State.A + data + carry) > 0xFF;

		// Sign bit is wrong if sign bit of operands is same
		// and sign bit of result is different
		// if <A and data> differ in sign and <A and sum> have the same sign, set the overflow flag
		// https://stackoverflow.com/questions/29193303/6502-emulation-proper-way-to-implement-adc-and-sbc
		State.V = (~(State.A ^ data) & (State.A ^ sum) & 0x80) != 0;

		State.A = sum;
	}

	// EOR - Exclusive OR
	[CpuInstruction(65, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(69, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(73, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(77, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(81, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(85, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(89, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(93, AddressMode.AbsoluteX, 3, 4, 1)]
	private void eor(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.A ^= data;
		SetZn(State.A);
	}

	// CLV - Clear Overflow Flag
	[CpuInstruction(184, AddressMode.Implied, 1, 2, 0)]
	private void clv(AddressMode mode, ushort address)
	{
		State.V = false;
	}

	// BMI - Branch if Minus
	[CpuInstruction(48, AddressMode.Relative, 2, 2, 1)]
	private void bmi(AddressMode mode, ushort address)
	{
		State.PC = State.N ? address : State.PC;
	}

	// PLP - Pull Processor Status
	[CpuInstruction(40, AddressMode.Implied, 1, 4, 0)]
	private void plp(AddressMode mode, ushort address)
	{
		SetProcessorFlags((byte)(PullStack() & ~0x10));
	}

	// CLD - Clear Decimal Mode
	[CpuInstruction(216, AddressMode.Implied, 1, 2, 0)]
	private void cld(AddressMode mode, ushort address)
	{
		State.D = false;
	}

	// CMP - Compare
	[CpuInstruction(193, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(197, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(201, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(205, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(209, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(213, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(217, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(221, AddressMode.AbsoluteX, 3, 4, 1)]
	private void cmp(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.C = State.A >= data;
		SetZn((byte)(State.A - data));
	}

	// AND - Logical AND
	[CpuInstruction(33, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(37, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(41, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(45, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(49, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(53, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(57, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(61, AddressMode.AbsoluteX, 3, 4, 1)]
	private void and(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.A &= data;
		SetZn(State.A);
	}

	// PLA - Pull Accumulator
	[CpuInstruction(104, AddressMode.Implied, 1, 4, 0)]
	private void pla(AddressMode mode, ushort address)
	{
		State.A = PullStack();
		SetZn(State.A);
	}

	// PHP - Push Processor Status
	[CpuInstruction(8, AddressMode.Implied, 1, 3, 0)]
	private void php(AddressMode mode, ushort address)
	{
		PushStack((byte)(GetStatusFlags() | 0x10));
	}

	// SED - Set Decimal Flag
	[CpuInstruction(248, AddressMode.Implied, 1, 2, 0)]
	private void sed(AddressMode mode, ushort address)
	{
		State.D = true;
	}

	// CLI - Clear Interrupt Disable
	[CpuInstruction(88, AddressMode.Implied, 1, 2, 0)]
	private void cli(AddressMode mode, ushort address)
	{
		State.I = false;
	}

	// SEI - Set Interrupt Disable
	[CpuInstruction(120, AddressMode.Implied, 1, 2, 0)]
	private void sei(AddressMode mode, ushort address)
	{
		State.I = true;
	}

	// DEC - Deincrement Memory
	[CpuInstruction(198, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(206, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(214, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(222, AddressMode.AbsoluteX, 3, 7, 0)]
	private void dec(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		data--;
		_memory.Write(address, data);
		SetZn(data);
	}

	// INC - Increment Memory
	[CpuInstruction(230, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(238, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(246, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(254, AddressMode.AbsoluteX, 3, 7, 0)]
	private void inc(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		data++;
		_memory.Write(address, data);
		SetZn(data);
	}

	// RTS - Return from Subroutine
	[CpuInstruction(96, AddressMode.Implied, 1, 6, 0)]
	private void rts(AddressMode mode, ushort address)
	{
		State.PC = (ushort)(PullStack16() + 1);
	}

	// JSR - Jump to Subroutine
	[CpuInstruction(32, AddressMode.Absolute, 3, 6, 0)]
	private void jsr(AddressMode mode, ushort address)
	{
		PushStack16((ushort)(State.PC - 1));
		State.PC = address;
	}

	// BPL - Branch if Positive
	[CpuInstruction(16, AddressMode.Relative, 2, 2, 1)]
	private void bpl(AddressMode mode, ushort address)
	{
		if (!State.N)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// BVC - Branch if Overflow Clear
	[CpuInstruction(80, AddressMode.Relative, 2, 2, 1)]
	private void bvc(AddressMode mode, ushort address)
	{
		if (!State.V)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// BVS - Branch if Overflow Set
	[CpuInstruction(112, AddressMode.Relative, 2, 2, 1)]
	private void bvs(AddressMode mode, ushort address)
	{
		if (State.V)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// BIT - Bit Test
	[CpuInstruction(36, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(44, AddressMode.Absolute, 3, 4, 0)]
	private void bit(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.N = IsBitSet(data, 7);
		State.V = IsBitSet(data, 6);
		State.Z = (data & State.A) == 0;
	}

	// BNE - Branch if Not Equal
	[CpuInstruction(208, AddressMode.Relative, 2, 2, 1)]
	private void bne(AddressMode mode, ushort address)
	{
		if (!State.Z)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// BEQ - Branch if Equal
	[CpuInstruction(240, AddressMode.Relative, 2, 2, 1)]
	private void beq(AddressMode mode, ushort address)
	{
		if (State.Z)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// CLC - Clear Carry Flag
	[CpuInstruction(24, AddressMode.Implied, 1, 2, 0)]
	private void clc(AddressMode mode, ushort address)
	{
		State.C = false;
	}

	// BCC - Branch if Carry Clear
	[CpuInstruction(144, AddressMode.Relative, 2, 2, 1)]
	private void bcc(AddressMode mode, ushort address)
	{
		if (!State.C)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// BCs - Branch if Carry Set
	[CpuInstruction(176, AddressMode.Relative, 2, 2, 1)]
	private void bcs(AddressMode mode, ushort address)
	{
		if (State.C)
		{
			HandleBranchCycles(State.PC, address);
			State.PC = address;
		}
	}

	// SEC - Set Carry Flag
	[CpuInstruction(56, AddressMode.Implied, 1, 2, 0)]
	private void sec(AddressMode mode, ushort address)
	{
		State.C = true;
	}

	// NOP - No Operation
	[CpuInstruction(4, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(12, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(20, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(26, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(28, AddressMode.AbsoluteX, 3, 4, 1)]
	[CpuInstruction(52, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(58, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(60, AddressMode.AbsoluteX, 3, 4, 1)]
	[CpuInstruction(68, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(84, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(90, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(92, AddressMode.AbsoluteX, 3, 4, 1)]
	[CpuInstruction(100, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(116, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(122, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(124, AddressMode.AbsoluteX, 3, 4, 1)]
	[CpuInstruction(128, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(130, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(137, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(194, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(212, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(218, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(220, AddressMode.AbsoluteX, 3, 4, 1)]
	[CpuInstruction(226, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(234, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(244, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(250, AddressMode.Implied, 1, 2, 0)]
	[CpuInstruction(252, AddressMode.AbsoluteX, 3, 4, 1)]
	private void nop(AddressMode mode, ushort address) { }

	// STX - Store X Register
	[CpuInstruction(134, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(142, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(150, AddressMode.ZeroPageY, 2, 4, 0)]
	private void stx(AddressMode mode, ushort address)
	{
		_memory.Write(address, State.X);
	}

	// LAX (Load Accumulator and X Register with Memory)
	[CpuInstruction(163, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(167, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(175, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(179, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(183, AddressMode.ZeroPageY, 2, 4, 0)]
	[CpuInstruction(191, AddressMode.AbsoluteY, 3, 4, 1)]
	private void lax(AddressMode mode, ushort address)
	{
		// Fetch absolute address
		State.A = _memory.Read(address);
		// Load X register with the same value as A
		State.X = State.A;
		SetZn(State.A);
	}

	// LDY - Load Y Register
	[CpuInstruction(160, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(164, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(172, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(180, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(188, AddressMode.AbsoluteX, 3, 4, 1)]
	private void ldy(AddressMode mode, ushort address)
	{
		State.Y = _memory.Read(address);
		SetZn(State.Y);
	}

	// LDX - Load X Register
	[CpuInstruction(162, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(166, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(174, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(182, AddressMode.ZeroPageY, 2, 4, 0)]
	[CpuInstruction(190, AddressMode.AbsoluteY, 3, 4, 1)]

	private void ldx(AddressMode mode, ushort address)
	{
		State.X = _memory.Read(address);
		SetZn(State.X);
	}

	// JMP - Jump
	[CpuInstruction(76, AddressMode.Absolute, 3, 3, 0)]
	[CpuInstruction(108, AddressMode.Indirect, 3, 5, 0)]
	private void jmp(AddressMode mode, ushort address)
	{
		State.PC = address;
	}

	// STA - Store Accumulator
	[CpuInstruction(129, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(133, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(141, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(145, AddressMode.IndirectIndexed, 2, 6, 0)]
	[CpuInstruction(149, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(153, AddressMode.AbsoluteY, 3, 5, 0)]
	[CpuInstruction(157, AddressMode.AbsoluteX, 3, 5, 0)]
	private void sta(AddressMode mode, ushort address)
	{
		_memory.Write(address, State.A);
	}

	// ORA - Logical Inclusive OR
	[CpuInstruction(1, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(5, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(9, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(13, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(17, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(21, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(25, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(29, AddressMode.AbsoluteX, 3, 4, 1)]
	private void ora(AddressMode mode, ushort address)
	{
		State.A |= _memory.Read(address);
		SetZn(State.A);
	}

	// LDA - Load A Register
	[CpuInstruction(161, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(165, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(169, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(173, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(177, AddressMode.IndirectIndexed, 2, 5, 1)]
	[CpuInstruction(181, AddressMode.ZeroPageX, 2, 4, 0)]
	[CpuInstruction(185, AddressMode.AbsoluteY, 3, 4, 1)]
	[CpuInstruction(189, AddressMode.AbsoluteX, 3, 4, 1)]
	private void lda(AddressMode mode, ushort address)
	{
		State.A = _memory.Read(address);
		SetZn(State.A);
	}

	// PHA - Push Accumulator
	[CpuInstruction(72, AddressMode.Implied, 1, 3, 0)]
	private void pha(AddressMode mode, ushort address)
	{
		PushStack(State.A);
	}

	// ASL - Arithmetic Shift Left
	[CpuInstruction(6, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(10, AddressMode.Accumulator, 1, 2, 0)]
	[CpuInstruction(14, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(22, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(30, AddressMode.AbsoluteX, 3, 7, 0)]
	private void asl(AddressMode mode, ushort address)
	{
		if (mode == AddressMode.Accumulator)
		{
			State.C = IsBitSet(State.A, 7);
			State.A <<= 1;
			SetZn(State.A);
		}
		else
		{
			var data = _memory.Read(address);
			State.C = IsBitSet(data, 7);
			var dataUpdated = (byte)(data << 1);
			_memory.Write(address, dataUpdated);
			SetZn(dataUpdated);
		}
	}

	// ROL - Rotate Left
	[CpuInstruction(38, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(42, AddressMode.Accumulator, 1, 2, 0)]
	[CpuInstruction(46, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(54, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(62, AddressMode.AbsoluteX, 3, 7, 0)]
	private void rol(AddressMode mode, ushort address)
	{
		var Corig = State.C;
		if (mode == AddressMode.Accumulator)
		{
			State.C = IsBitSet(State.A, 7);
			State.A <<= 1;
			State.A |= (byte)(Corig ? 1 : 0);

			SetZn(State.A);
		}
		else
		{
			var data = _memory.Read(address);
			State.C = IsBitSet(data, 7);

			data <<= 1;
			data |= (byte)(Corig ? 1 : 0);

			_memory.Write(address, data);

			SetZn(data);
		}
	}

	// LSR - Logical Shift Right
	[CpuInstruction(70, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(74, AddressMode.Accumulator, 1, 2, 0)]
	[CpuInstruction(78, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(86, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(94, AddressMode.AbsoluteX, 3, 7, 0)]
	private void lsr(AddressMode mode, ushort address)
	{
		if (mode == AddressMode.Accumulator)
		{
			State.C = (State.A & 1) == 1;
			State.A >>= 1;

			SetZn(State.A);
		}
		else
		{
			var value = _memory.Read(address);
			State.C = (value & 1) == 1;

			var updatedValue = (byte)(value >> 1);

			_memory.Write(address, updatedValue);

			SetZn(updatedValue);
		}
	}

	#region Invalid opcodes

	// RLA (Rotate Left then AND with Accumulator)
	[CpuInstruction(35, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(39, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(47, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(51, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(55, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(59, AddressMode.AbsoluteY, 3, 7, 0)]
	[CpuInstruction(63, AddressMode.AbsoluteX, 3, 7, 0)]
	private void rla(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		uint rotatedValue = (uint)((data << 1) | (State.C ? 1 : 0));
		//C = (data & 0x80) != 0;
		SetCarry(rotatedValue);
		rotatedValue &= 0xff;
		State.A &= (byte)rotatedValue;
		SetZn(State.A);
		_memory.Write(address, (byte)rotatedValue);
	}

	// SRE (LSR then EOR) Absolute addressing mode
	[CpuInstruction(67, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(71, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(79, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(83, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(87, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(91, AddressMode.AbsoluteY, 3, 7, 0)]
	[CpuInstruction(95, AddressMode.AbsoluteX, 3, 7, 0)]
	private void sre(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.C = (data & 0x01) != 0;
		// Shift right operation
		byte shiftedValue = (byte)(data >> 1);
		// Perform Exclusive OR (EOR) operation with Accumulator
		State.A ^= shiftedValue;

		// Set flags based on the result (e.g., update zero, negative, carry flags)
		SetZn(State.A);
		_memory.Write(address, shiftedValue);
	}

	// RRA (ROR then ADC) Absolute addressing mode
	[CpuInstruction(99, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(103, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(111, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(115, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(119, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(123, AddressMode.AbsoluteY, 3, 7, 0)]
	[CpuInstruction(127, AddressMode.AbsoluteX, 3, 7, 0)]
	private void rra(AddressMode mode, ushort address)
	{
		byte value = _memory.Read(address);

		// Rotate Right
		bool oldCarry = State.C;
		State.C = (value & 0x01) != 0;
		value = (byte)((value >> 1) | (oldCarry ? 0x80 : 0x00));
		_memory.Write(address, value);

		// ADC operation
		int result = State.A + value + (State.C ? 1 : 0);
		State.C = result > 0xFF;
		State.V = ((State.A ^ result) & (value ^ result) & 0x80) != 0;
		State.A = (byte)result;
		SetZn(State.A);
	}

	// DCP (DCM) Absolute addressing mode
	[CpuInstruction(195, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(199, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(207, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(211, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(215, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(219, AddressMode.AbsoluteY, 3, 7, 0)]
	[CpuInstruction(223, AddressMode.AbsoluteX, 3, 7, 0)]
	private void dcp(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		data--;
		_memory.Write(address, data);
		var result = State.A - data;
		State.C = State.A >= data;
		SetZn((byte)result);
	}

	// ISC Zero Page addressing mode
	[CpuInstruction(255, AddressMode.AbsoluteX, 3, 7, 0)]
	[CpuInstruction(227, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(231, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(239, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(243, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(247, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(251, AddressMode.AbsoluteY, 3, 7, 0)]
	private void isc(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		data++;
		_memory.Write(address, data);

		int temp = State.A - data - (State.C ? 0 : 1);

		// Set carry flag: if no borrow occurred, set carry flag; otherwise, clear it
		State.C = temp >= 0;

		// Set zero flag: if result == 0, set zero flag; otherwise, clear it
		State.Z = (temp & 0xFF) == 0;

		// Set overflow flag: if result is out of signed byte range, set overflow flag; otherwise, clear it
		State.V = ((State.A ^ temp) & 0x80) != 0 && ((State.A ^ data) & 0x80) != 0;

		// Set negative flag: if the high bit of result is set, set negative flag; otherwise, clear it
		State.N = (temp & 0x80) != 0;

		// Store result in Accumulator
		State.A = (byte)(temp & 0xFF);
	}

	// ALR Immediate addressing mode
	[CpuInstruction(75, AddressMode.Immediate, 2, 2, 0)]
	private void alr(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		// 1. Perform AND with the accumulator
		State.A &= data;

		// 2. Perform a logical shift right on the result
		// Set carry flag to the LSB of the result
		State.C = (State.A & 0x01) != 0;
		State.A >>= 1;
		SetZn(State.A);
	}

	[CpuInstruction(107, AddressMode.Immediate, 2, 2, 0)]
	private void arr(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.A &= data;

		State.A = (byte)((State.A >> 1) | (State.C ? 0x80 : 0));
		State.C = (State.A & 0x1) != 0;
		SetZn(State.A);

		State.V = State.C = false;
		switch (State.A & 0x60)
		{
			case 0x20:
				State.V = true;
				break;
			case 0x40:
				State.V = State.C = true;
				break;
			case 0x60:
				State.C = true;
				break;
		}

		//// Fetch immediate operand
		//byte data = _memory.Read(address);

		//// Perform AND operation
		//A &= data;

		//// Rotate right A
		//var Corig = C;
		//C = (A & 0x01) != 0; // New carry flag is the old bit 0 of A
		//A >>= 1;
		//if (Corig)
		//{
		//	A |= 0x80; // Set bit 7 if old carry was set
		//}
		//SetZn(A);
	}

	// ANE Immediate addressing mode
	[CpuInstruction(139, AddressMode.Immediate, 0, 2, 0)]
	private void ane(AddressMode mode, ushort address)
	{
		byte data = _memory.Read(address);
		// Perform AND with the accumulator and X register, then AND with 0xEF
		State.A = (byte)((State.A & data & State.X) & 0xEF);
		SetZn(State.A);
	}

	// OAL Immediate addressing mode
	[CpuInstruction(171, AddressMode.Absolute, 2, 4, 0)]
	private void oal(AddressMode mode, ushort address)
	{

		uint immediateValue = _memory.Read(address);
		immediateValue |= (uint)(_memory.Read((ushort)(address + 1)) >> 8);
		//(cpuMemory->Read8(newoffset) << 8)

		// Perform the AND operation
		State.A &= (byte)immediateValue;
		// Set the Zero flag if the result is 0
		State.Z = (State.A == 0);
		// Set the Negative flag based on the result
		State.N = (State.A & 0x80) != 0;
		// Set the Carry flag based on the result (same as Negative flag in this case)
		State.C = (State.A & 0x80) != 0;
		State.X = State.A;

		//var data = _memory.Read(address);
		//CurrentState.A |= 0xFF;
		//SetZn(CurrentState.A);
		//CurrentState.A &= data;
		//SetZn(CurrentState.A);
		//CurrentState.X = CurrentState.A;
		//SetZn(CurrentState.X);








		//var data = _memory.Read(address);
		//// Perform AND with the accumulator and the immediate value, then transfer to X register
		//CurrentState.A = (byte)((CurrentState.A | 0xFF) & data);
		//CurrentState.X = CurrentState.A;
		//SetZn(CurrentState.A);
	}

	// SAX (Store Accumulator AND X) Absolute addressing mode
	[CpuInstruction(131, AddressMode.IndexedIndirect, 2, 6, 0)]
	[CpuInstruction(135, AddressMode.ZeroPage, 2, 3, 0)]
	[CpuInstruction(143, AddressMode.Absolute, 3, 4, 0)]
	[CpuInstruction(151, AddressMode.ZeroPageY, 2, 4, 0)]
	private void sax(AddressMode mode, ushort address)
	{
		// Perform bitwise AND of Accumulator (A) and X register
		var result = (byte)(State.A & State.X);
		// Store the result in memory at the specified address
		_memory.Write(address, result);
	}

	[CpuInstruction(203, AddressMode.Immediate, 2, 2, 0)]
	private void sbx(AddressMode mode, ushort address)
	{
		//var data = (byte)(CurrentState.A & CurrentState.X);
		//_memory.Write(address, data);

		// Fetch immediate value
		var data = _memory.Read(address);

		// Perform AND operation between A and X, then subtract immediateValue
		var result = (State.A & State.X) - data;

		// Store the result in X register
		State.X = (byte)result;
		State.C = result > data;
		//_memory.Write(address, (byte)result);
		// Update flags
		//SetZn(CurrentState.X);
	}

	[CpuInstruction(155, AddressMode.AbsoluteY, 3, 5, 0)]
	private void tas(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		State.S = (byte)(State.X & State.A);
		_memory.Write(address, 0);
	}

	[CpuInstruction(156, AddressMode.AbsoluteX, 3, 5, 0)]
	private void shy(AddressMode mode, ushort address)
	{
		//var data  = _memory.Read(address);
		var data = (byte)(((State.Y & 0xFF00) >> 8) + 1);
		_memory.Write(address, data);
	}

	[CpuInstruction(147, AddressMode.IndirectIndexed, 0, 6, 0)]
	[CpuInstruction(159, AddressMode.AbsoluteY, 3, 5, 0)]
	private void sha(AddressMode mode, ushort address)
	{
		_memory.Write(address, 0);
	}

	// ANC (And with Carry) with Immediate Addressing
	[CpuInstruction(11, AddressMode.Immediate, 2, 2, 0)]
	[CpuInstruction(43, AddressMode.Immediate, 2, 2, 0)]
	private void anc(AddressMode mode, ushort address)
	{
		// Fetch the immediate operand
		var data = _memory.Read(address);

		// Perform the AND operation
		State.A &= data;

		// Set the carry flag based on the result
		SetCarry(State.A);

		// Set other flags as needed (ZeroFlag, NegativeFlag, OverflowFlag, etc.)
		// Update CPU cycles, memory access, etc. as per the 6502 specification
		SetZn(State.A);
	}

	[CpuInstruction(187, AddressMode.AbsoluteY, 3, 4, 1)]
	private void las(AddressMode mode, ushort address)
	{
		var data = _memory.Read(address);
		byte result = (byte)(data & State.S);

		// Set A, X, and Stack Pointer (SP) to the result
		State.A = result;
		State.X = result;
		State.S = result;
		SetZn(result);
	}

	[CpuInstruction(158, AddressMode.AbsoluteY, 3, 5, 0)]
	private void shx(AddressMode mode, ushort address)
	{
		_memory.Write(address, 0);
	}

	// ASO (Arithmetic Shift Left followed by OR with Accumulator) Absolute addressing mode
	[CpuInstruction(3, AddressMode.IndexedIndirect, 2, 8, 0)]
	[CpuInstruction(7, AddressMode.ZeroPage, 2, 5, 0)]
	[CpuInstruction(15, AddressMode.Absolute, 3, 6, 0)]
	[CpuInstruction(19, AddressMode.IndirectIndexed, 2, 8, 0)]
	[CpuInstruction(23, AddressMode.ZeroPageX, 2, 6, 0)]
	[CpuInstruction(27, AddressMode.AbsoluteY, 3, 7, 0)]
	[CpuInstruction(31, AddressMode.AbsoluteX, 3, 7, 0)]
	private void slo(AddressMode mode, ushort address)
	{
		// Read data from memory at the specified address
		byte value = _memory.Read(address);
		State.C = (value & 0x80) != 0;  // Set Carry flag if the bit 7 is set
																					 // Perform ASL (Arithmetic Shift Left) on the value
		var shiftedValue = (byte)(value << 1);

		// Perform OR with Accumulator
		State.A |= shiftedValue;

		// Set flags based on the result (e.g., update zero, negative, carry flags)
		SetZn(State.A);

		// Write back the result to memory (if necessary)
		_memory.Write(address, shiftedValue);
		//WriteMemory(address, shiftedValue);
	}


	#endregion
}
