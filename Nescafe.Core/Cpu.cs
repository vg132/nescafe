using Nescafe.Services;
using System.Runtime.CompilerServices;

namespace Nescafe.Core;

/// <summary>
/// Represents a MOS Technologies 6502 CPU with Decimal Mode disabled (as
/// is the case with the NES).
/// </summary>
public partial class Cpu
{
	protected readonly CpuMemory _memory;
	protected readonly Console _console;

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

	public CpuState State => _state;
	private CpuState _state;

	/// <summary>
	/// Initializes a new <see cref="T:Nescafe.Cpu"/> CPU.
	/// </summary>
	/// <param name="console">The Console that this CPU is a part of</param>
	public Cpu(Console console)
	{
		_memory = console.CpuMemory;
		_console = console;
		_state = new CpuState();
		InitializeOpcodes();
	}

	/// <summary>
	/// Resets this CPU to its power on state.
	/// </summary>
	public void Reset()
	{
		_state.PC = _memory.Read16(0xFFFC);
		_state.S = 0xFD;
		_state.A = 0;
		_state.X = 0;
		_state.Y = 0;
		SetProcessorFlags(0x24);

		_state.Cycles = 0;

		_state.NmiInterrupt = false;
	}

	/// <summary>
	/// Triggers a non maskable interrupt on this CPU.
	/// </summary>
	public void TriggerNmi()
	{
		LoggingService.LogEvent(NESEvents.Cpu, "mni triggered");
		_state.NmiInterrupt = true;
	}

	/// <summary>
	/// Triggers an interrupt on this CPU if the interrupt disable flag
	/// is not set.
	/// </summary>
	public void TriggerIrq()
	{
		if (!_state.I)
		{
			_state.IrqInterrupt = true;
		}
	}

	/// <summary>
	/// Executes the next CPU instruction specified by the Program Counter.
	/// </summary>
	public void Step()
	{
		if (_state.Cycles-- > 0)
		{
			return;
		}

		if (_state.IrqInterrupt)
		{
			Irq();
		}

		_state.IrqInterrupt = false;

		if (_state.NmiInterrupt)
		{
			if (_state.NmiDelay <= 0)
			{
				LoggingService.LogEvent(NESEvents.Cpu, "handle nmi");
				Nmi();
				_state.NmiInterrupt = false;
				_state.NmiDelay = 0;
			}
			else
			{
				_state.NmiDelay--;
			}
		}

		var cyclesOrig = _state.Cycles;
		var data = _memory.Read(_state.PC);
		var currentInstruction = _cpuInstructions[data];
		LoggingService.LogEvent(NESEvents.Cpu, $"cycle: {_console.Ppu.State.CpuCallCount}, instruction: {currentInstruction.Name}, memory pointer: {_state.PC.ToString("x4")}, data: {data.ToString("x4")}, s: {_state.S.ToString("x4")}");
		// Get address to operate on
		var pageCrossed = false;
		var address = GetMemoryAddress(currentInstruction, out pageCrossed);
		_state.PC += (ushort)currentInstruction.InstructionSize;
		_state.Cycles += currentInstruction.InstructionCycles;
		if (pageCrossed)
		{
			_state.Cycles += currentInstruction.InstructionPageCycles;
		}
		currentInstruction.InvokeOpCode(address);
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
				address = (ushort)(_state.PC + 1);
				break;
			case AddressMode.Absolute:
				address = _memory.Read16((ushort)(_state.PC + 1));
				break;
			case AddressMode.AbsoluteX:
				address = _memory.Read16((ushort)(_state.PC + 1));
				pageCrossed = IsPageCross(address, (ushort)(address + _state.X));
				address += _state.X;
				break;
			case AddressMode.AbsoluteY:
				address = _memory.Read16((ushort)(_state.PC + 1));
				pageCrossed = IsPageCross(address, (ushort)(address + _state.Y));
				address += _state.Y;
				break;
			case AddressMode.Accumulator:
				break;
			case AddressMode.Relative:
				address = (ushort)(_state.PC + (sbyte)_memory.Read((ushort)(_state.PC + 1)) + 2);
				break;
			case AddressMode.ZeroPage:
				address = _memory.Read((ushort)(_state.PC + 1));
				break;
			case AddressMode.ZeroPageY:
				address = (ushort)((_memory.Read((ushort)(_state.PC + 1)) + _state.Y) & 0xFF);
				break;
			case AddressMode.ZeroPageX:
				address = (ushort)((_memory.Read((ushort)(_state.PC + 1)) + _state.X) & 0xFF);
				break;
			case AddressMode.Indirect:
				// Must wrap if at the end of a page to emulate a 6502 bug present in the JMP instruction
				address = _memory.Read16WrapPage(_memory.Read16((ushort)(_state.PC + 1)));
				break;
			case AddressMode.IndexedIndirect:
				// Zeropage address of lower nibble of target address (& 0xFF to wrap at 255)
				var lowerNibbleAddress = (ushort)((_memory.Read((ushort)(_state.PC + 1)) + _state.X) & 0xFF);
				// Target address (Must wrap to 0x00 if at 0xFF)
				address = _memory.Read16WrapPage(lowerNibbleAddress);
				break;
			case AddressMode.IndirectIndexed:
				// Zeropage address of the value to add the Y register to to get the target address
				var valueAddress = (ushort)_memory.Read((ushort)(_state.PC + 1));

				// Target address (Must wrap to 0x00 if at 0xFF)
				address = _memory.Read16WrapPage(valueAddress);
				pageCrossed = IsPageCross(address, (ushort)(address + _state.Y));
				address += _state.Y;
				break;
		}
		return address;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetZn(byte value)
	{
		_state.Z = value == 0;
		_state.N = ((value >> 7) & 1) == 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetCarry(uint value)
	{
		_state.C = value > 0xff;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetCarrySubstract(uint value)
	{
		_state.C = value < 0x100;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsBitSet(byte value, int index)
	{
		return (value & (1 << index)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte PullStack()
	{
		_state.S++;
		var data = _memory.Read((ushort)(0x0100 | _state.S));
		return data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PushStack(byte data)
	{
		_memory.Write((ushort)(0x100 | _state.S), data);
		_state.S--;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ushort PullStack16()
	{
		var lo = PullStack();
		var hi = PullStack();
		return (ushort)((hi << 8) | lo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PushStack16(ushort data)
	{
		var lo = (byte)(data & 0xFF);
		var hi = (byte)((data >> 8) & 0xFF);

		PushStack(hi);
		PushStack(lo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte GetStatusFlags()
	{
		byte flags = 0;

		if (_state.C)
		{
			flags |= 1 << 0; // Carry flag, bit 0
		}

		if (_state.Z)
		{
			flags |= 1 << 1; // Zero flag, bit 1
		}

		if (_state.I)
		{
			flags |= 1 << 2; // Interrupt disable flag, bit 2
		}

		if (_state.D)
		{
			flags |= 1 << 3; // Decimal mode flag, bit 3
		}

		if (_state.B)
		{
			flags |= 1 << 4; // Break mode, bit 4
		}

		flags |= 1 << 5; // Bit 5, always set
		if (_state.V)
		{
			flags |= 1 << 6; // Overflow flag, bit 6
		}

		if (_state.N)
		{
			flags |= 1 << 7; // Negative flag, bit 7
		}

		return flags;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetProcessorFlags(byte flags)
	{
		_state.C = IsBitSet(flags, 0);
		_state.Z = IsBitSet(flags, 1);
		_state.I = IsBitSet(flags, 2);
		_state.D = IsBitSet(flags, 3);
		_state.B = IsBitSet(flags, 4);
		_state.V = IsBitSet(flags, 6);
		_state.N = IsBitSet(flags, 7);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsPageCross(ushort a, ushort b) => (a & 0xFF00) != (b & 0xFF00);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void HandleBranchCycles(ushort origPc, ushort branchPc)
	{
		_state.Cycles++;
		_state.Cycles += IsPageCross(origPc, branchPc) ? 1 : 0;
	}

	private void Nmi()
	{
		PushStack16(_state.PC);
		PushStack(GetStatusFlags());
		_state.PC = _memory.Read16(0xFFFA);
		_state.I = true;
	}

	private void Irq()
	{
		PushStack16(_state.PC);
		PushStack(GetStatusFlags());
		_state.PC = _memory.Read16(0xFFFE);
		_state.I = true;
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
			_state = state.State;
			_memory.LoadState(state.CpuMemory);
		}
	}

	#endregion
}