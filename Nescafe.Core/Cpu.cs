using Nescafe.Services;

namespace Nescafe.Core;

/// <summary>
/// Represents a MOS Technologies 6502 CPU with Decimal Mode disabled (as
/// is the case with the NES).
/// </summary>
public partial class Cpu
{
	private readonly CpuMemory _memory;
	private readonly Console _console;

	public enum AddressMode
	{
		Absolute = 1,    // 1
		AbsoluteX,       // 2
		AbsoluteY,       // 3
		Accumulator,     // 4
		Immediate,       // 5
		Implied,         // 6
		IndexedIndirect, // 7
		Indirect,        // 8
		IndirectIndexed, // 9
		Relative,        // 10
		ZeroPage,        // 11
		ZeroPageX,       // 12
		ZeroPageY        // 13
	};

	public CpuState State { get; private set; }

	/// <summary>
	/// Initializes a new <see cref="T:Nescafe.Cpu"/> CPU.
	/// </summary>
	/// <param name="console">The Console that this CPU is a part of</param>
	public Cpu(Console console)
	{
		_memory = console.CpuMemory;
		_console = console;
		State = new CpuState();
		InitializeOpcodes();
	}

	/// <summary>
	/// Resets this CPU to its power on state.
	/// </summary>
	public void Reset()
	{
		State.PC = _memory.Read16(0xFFFC);
		State.S = 0xFD;
		State.A = 0;
		State.X = 0;
		State.Y = 0;
		SetProcessorFlags(0x24);

		State.Cycles = 0;
		State.Idle = 0;

		State.NmiInterrupt = false;
	}

	/// <summary>
	/// Triggers a non maskable interrupt on this CPU.
	/// </summary>
	public void TriggerNmi()
	{
		State.NmiInterrupt = true;
	}

	/// <summary>
	/// Triggers an interrupt on this CPU if the interrupt disable flag
	/// is not set.
	/// </summary>
	public void TriggerIrq()
	{
		if (!State.I)
		{
			State.IrqInterrupt = true;
		}
	}

	/// <summary>
	/// Instructs the CPU to idle for the specified number of cycles.
	/// </summary>
	/// <param name="idleCycles">Idle cycles.</param>
	public void AddIdleCycles(int idleCycles)
	{
		State.Idle += idleCycles;
	}

	/// <summary>
	/// Executes the next CPU instruction specified by the Program Counter.
	/// </summary>
	public void Step()
	{
		if (State.Idle > 0)
		{
			State.Idle--;
			return;
		}

		if (State.IrqInterrupt)
		{
			Irq();
		}

		State.IrqInterrupt = false;

		if (State.NmiInterrupt)
		{
			Nmi();
		}

		State.NmiInterrupt = false;

		var cyclesOrig = State.Cycles;
		var data = _memory.Read(State.PC);
		var currentInstruction = _cpuInstructions[data];
		LoggingService.LogEvent(NESEvents.Cpu, $"cycle: {State.Cycles}, instruction: {currentInstruction.Name}, memory pointer: {State.PC.ToString("x4")}, data: {data.ToString("x4")}, s: {State.S.ToString("x4")}");

		// Get address to operate on
		var pageCrossed = false;
		var address = GetMemoryAddress(currentInstruction, out pageCrossed);
		State.PC += (ushort)currentInstruction.InstructionSize;
		State.Cycles += currentInstruction.InstructionCycles;
		if (pageCrossed)
		{
			State.Cycles += currentInstruction.InstructionPageCycles;
		}
		currentInstruction.Invoke(address);
		State.Idle += State.Cycles - cyclesOrig;
	}

	private ushort GetMemoryAddress(CPUInstruction currentInstruction, out bool pageCrossed)
	{
		ushort address = 0;
		pageCrossed = false;
		switch (currentInstruction.AddressMode)
		{
			case AddressMode.Implied:
				break;
			case AddressMode.Immediate:
				address = (ushort)(State.PC + 1);
				break;
			case AddressMode.Absolute:
				address = _memory.Read16((ushort)(State.PC + 1));
				break;
			case AddressMode.AbsoluteX:
				address = (ushort)(_memory.Read16((ushort)(State.PC + 1)) + State.X);
				pageCrossed = IsPageCross((ushort)(address - State.X), State.X);
				break;
			case AddressMode.AbsoluteY:
				address = (ushort)(_memory.Read16((ushort)(State.PC + 1)) + State.Y);
				pageCrossed = IsPageCross((ushort)(address - State.Y), State.Y);
				break;
			case AddressMode.Accumulator:
				break;
			case AddressMode.Relative:
				address = (ushort)(State.PC + (sbyte)_memory.Read((ushort)(State.PC + 1)) + 2);
				break;
			case AddressMode.ZeroPage:
				address = _memory.Read((ushort)(State.PC + 1));
				break;
			case AddressMode.ZeroPageY:
				address = (ushort)((_memory.Read((ushort)(State.PC + 1)) + State.Y) & 0xFF);
				break;
			case AddressMode.ZeroPageX:
				address = (ushort)((_memory.Read((ushort)(State.PC + 1)) + State.X) & 0xFF);
				break;
			case AddressMode.Indirect:
				// Must wrap if at the end of a page to emulate a 6502 bug present in the JMP instruction
				address = _memory.Read16WrapPage(_memory.Read16((ushort)(State.PC + 1)));
				break;
			case AddressMode.IndexedIndirect:
				// Zeropage address of lower nibble of target address (& 0xFF to wrap at 255)
				var lowerNibbleAddress = (ushort)((_memory.Read((ushort)(State.PC + 1)) + State.X) & 0xFF);
				// Target address (Must wrap to 0x00 if at 0xFF)
				address = _memory.Read16WrapPage(lowerNibbleAddress);
				break;
			case AddressMode.IndirectIndexed:
				// Zeropage address of the value to add the Y register to to get the target address
				var valueAddress = (ushort)_memory.Read((ushort)(State.PC + 1));

				// Target address (Must wrap to 0x00 if at 0xFF)
				address = (ushort)(_memory.Read16WrapPage(valueAddress) + State.Y);
				pageCrossed = IsPageCross((ushort)(address - State.Y), address);
				break;
		}
		return address;
	}

	private void SetZn(byte value)
	{
		State.Z = value == 0;
		State.N = ((value >> 7) & 1) == 1;
	}

	private void SetCarry(uint value)
	{
		State.C = value > 0xff;
	}

	private void SetCarrySubstract(uint value)
	{
		State.C = value < 0x100;
	}

	private bool IsBitSet(byte value, int index)
	{
		return (value & (1 << index)) != 0;
	}

	private byte PullStack()
	{
		State.S++;
		var data = _memory.Read((ushort)(0x0100 | State.S));
		return data;
	}

	private void PushStack(byte data)
	{
		_memory.Write((ushort)(0x100 | State.S), data);
		State.S--;
	}

	private ushort PullStack16()
	{
		var lo = PullStack();
		var hi = PullStack();
		return (ushort)((hi << 8) | lo);
	}

	private void PushStack16(ushort data)
	{
		var lo = (byte)(data & 0xFF);
		var hi = (byte)((data >> 8) & 0xFF);

		PushStack(hi);
		PushStack(lo);
	}

	private byte GetStatusFlags()
	{
		byte flags = 0;

		if (State.C)
		{
			flags |= 1 << 0; // Carry flag, bit 0
		}

		if (State.Z)
		{
			flags |= 1 << 1; // Zero flag, bit 1
		}

		if (State.I)
		{
			flags |= 1 << 2; // Interrupt disable flag, bit 2
		}

		if (State.D)
		{
			flags |= 1 << 3; // Decimal mode flag, bit 3
		}

		if (State.B)
		{
			flags |= 1 << 4; // Break mode, bit 4
		}

		flags |= 1 << 5; // Bit 5, always set
		if (State.V)
		{
			flags |= 1 << 6; // Overflow flag, bit 6
		}

		if (State.N)
		{
			flags |= 1 << 7; // Negative flag, bit 7
		}

		return flags;
	}

	private void SetProcessorFlags(byte flags)
	{
		State.C = IsBitSet(flags, 0);
		State.Z = IsBitSet(flags, 1);
		State.I = IsBitSet(flags, 2);
		State.D = IsBitSet(flags, 3);
		State.B = IsBitSet(flags, 4);
		State.V = IsBitSet(flags, 6);
		State.N = IsBitSet(flags, 7);
	}

	private bool IsPageCross(ushort a, ushort b)
	{
		return (a & 0xFF) != (b & 0xFF);
	}

	private void HandleBranchCycles(ushort origPc, ushort branchPc)
	{
		State.Cycles++;
		State.Cycles += IsPageCross(origPc, branchPc) ? 1 : 0;
	}

	private void Nmi()
	{
		PushStack16(State.PC);
		PushStack(GetStatusFlags());
		State.PC = _memory.Read16(0xFFFA);
		State.I = true;
	}

	private void Irq()
	{
		PushStack16(State.PC);
		PushStack(GetStatusFlags());
		State.PC = _memory.Read16(0xFFFE);
		State.I = true;
	}

	#region Save/Load state

	[Serializable]
	public class CpuState2
	{
		public CpuState State { get; set; }
		public object CpuMemory;
	}

	public object SaveState()
	{
		lock (_console.FrameLock)
		{
			return new CpuState2
			{
				State = State,
				CpuMemory = _memory.SaveState()
			};
		}
	}

	public void LoadState(object stateObj)
	{
		lock (_console.FrameLock)
		{
			var state = stateObj as CpuState2;
			State = state.State;
			_memory.LoadState(state.CpuMemory);
		}
	}

	#endregion
}