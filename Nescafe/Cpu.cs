using System;

namespace Nescafe
{
	/// <summary>
	/// Represents a MOS Technologies 6502 CPU with Decimal Mode disabled (as
	/// is the case with the NES).
	/// </summary>
	public class Cpu
	{
		readonly CpuMemory _memory;

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

		public class CPUInstruction
		{
			public CPUInstruction(int id, AddressMode addressMode, int instructionSize, int instructionCycles, int instructionPageCycles, Action<AddressMode, ushort> instruction)
			{
				AddressMode = addressMode;
				InstructionSize = instructionSize;
				InstructionCycles = instructionCycles;
				InstructionPageCycles = instructionPageCycles;
				Instruction = instruction;
				Id = id;
			}

			public int Id { get; set; }
			public AddressMode AddressMode { get; set; }
			public int InstructionSize { get; set; }
			public int InstructionCycles { get; set; }
			public int InstructionPageCycles { get; set; }
			public Action<AddressMode, ushort> Instruction { get; set; }
		}

		// Registers
		private byte A;    // Accumulator
		private byte X;
		private byte Y;
		private byte S;    // Stack Pointer
		private ushort PC; // Program Counter (16 bits)

		// Status flag register (implemented as several booleans)
		private bool C; // Carry flag
		private bool Z; // Zero flag
		private bool I; // Interrpt Disable
		private bool D; // Decimal Mode (Not used)
		private bool B; // Break command
		private bool V; // Overflow flag
		private bool N; // Negative flag

		// Interrupts
		private bool irqInterrupt;
		private bool nmiInterrupt;

		/// <summary>
		/// Gets the current number of cycles executed by the CPU.
		/// </summary>
		/// <value>The current number of cycles executed by the CPU.</value>
		public int Cycles { get; private set; }

		// If positive, idle 1 cycle and deincrement each step
		private int _idle;

		private CPUInstruction[] _instructions;

		/// <summary>
		/// Initializes a new <see cref="T:Nescafe.Cpu"/> CPU.
		/// </summary>
		/// <param name="console">The Console that this CPU is a part of</param>
		public Cpu(Console console)
		{
			_memory = console.CpuMemory;
			_instructions = new CPUInstruction[256]
			{
				new CPUInstruction(0, AddressMode.Implied, 1, 7, 0, brk), // 00
				new CPUInstruction(1, AddressMode.IndexedIndirect, 2, 6, 0, ora), // 01
				new CPUInstruction(2, AddressMode.Implied, 0, 2, 0, ___), // 02
				new CPUInstruction(3, AddressMode.IndexedIndirect, 2, 8, 0, aso), // 03
				new CPUInstruction(4, AddressMode.ZeroPage, 2, 3, 0, nop), // 04
				new CPUInstruction(5, AddressMode.ZeroPage, 2, 3, 0, ora), // 05
				new CPUInstruction(6, AddressMode.ZeroPage, 2, 5, 0, asl), // 06
				new CPUInstruction(7, AddressMode.ZeroPage, 2, 5, 0, aso), // 07
				new CPUInstruction(8, AddressMode.Implied, 1, 3, 0, php), // 08
				new CPUInstruction(9, AddressMode.Immediate, 2, 2, 0, ora), // 09
				new CPUInstruction(10, AddressMode.Accumulator, 1, 2, 0, asl), // 0A
				new CPUInstruction(11, AddressMode.Immediate, 2, 2, 0, anc), // 0B
				new CPUInstruction(12, AddressMode.Absolute, 3, 4, 0, nop), // 0C
				new CPUInstruction(13, AddressMode.Absolute, 3, 4, 0, ora), // 0D
				new CPUInstruction(14, AddressMode.Absolute, 3, 6, 0, asl), // 0E
				new CPUInstruction(15, AddressMode.Absolute, 3, 6, 0, aso), // 0F
				new CPUInstruction(16, AddressMode.Relative, 2, 2, 1, bpl), // 10
				new CPUInstruction(17, AddressMode.IndirectIndexed, 2, 5, 1, ora), // 11
				new CPUInstruction(18, AddressMode.Implied, 0, 2, 0, ___), // 12
				new CPUInstruction(19, AddressMode.IndirectIndexed, 2, 8, 0, aso), // 13
				new CPUInstruction(20, AddressMode.ZeroPageX, 2, 4, 0, nop), // 14
				new CPUInstruction(21, AddressMode.ZeroPageX, 2, 4, 0, ora), // 15
				new CPUInstruction(22, AddressMode.ZeroPageX, 2, 6, 0, asl), // 16
				new CPUInstruction(23, AddressMode.ZeroPageX, 2, 6, 0, aso), // 17
				new CPUInstruction(24, AddressMode.Implied, 1, 2, 0, clc), // 18
				new CPUInstruction(25, AddressMode.AbsoluteY, 3, 4, 1, ora), // 19
				new CPUInstruction(26, AddressMode.Implied, 1, 2, 0, nop), // 1A
				new CPUInstruction(27, AddressMode.AbsoluteY, 3, 7, 0, aso), // 1B
				new CPUInstruction(28, AddressMode.AbsoluteX, 3, 4, 1, nop), // 1C
				new CPUInstruction(29, AddressMode.AbsoluteX, 3, 4, 1, ora), // 1D
				new CPUInstruction(30, AddressMode.AbsoluteX, 3, 7, 0, asl), // 1E
				new CPUInstruction(31, AddressMode.AbsoluteX, 3, 7, 0, aso), // 1F
				new CPUInstruction(32, AddressMode.Absolute, 3, 6, 0, jsr), // 20
				new CPUInstruction(33, AddressMode.IndexedIndirect, 2, 6, 0, and), // 21
				new CPUInstruction(34, AddressMode.Implied, 0, 2, 0, ___), // 22
				new CPUInstruction(35, AddressMode.IndexedIndirect, 2, 8, 0, rla), // 23
				new CPUInstruction(36, AddressMode.ZeroPage, 2, 3, 0, bit), // 24
				new CPUInstruction(37, AddressMode.ZeroPage, 2, 3, 0, and), // 25
				new CPUInstruction(38, AddressMode.ZeroPage, 2, 5, 0, rol), // 26
				new CPUInstruction(39, AddressMode.ZeroPage, 2, 5, 0, rla), // 27
				new CPUInstruction(40, AddressMode.Implied, 1, 4, 0, plp), // 28
				new CPUInstruction(41, AddressMode.Immediate, 2, 2, 0, and), // 29
				new CPUInstruction(42, AddressMode.Accumulator, 1, 2, 0, rol), // 2A
				new CPUInstruction(43, AddressMode.Immediate, 2, 2, 0, anc), // 2B
				new CPUInstruction(44, AddressMode.Absolute, 3, 4, 0, bit), // 2C
				new CPUInstruction(45, AddressMode.Absolute, 3, 4, 0, and), // 2D
				new CPUInstruction(46, AddressMode.Absolute, 3, 6, 0, rol), // 2E
				new CPUInstruction(47, AddressMode.Absolute, 3, 6, 0, rla), // 2F
				new CPUInstruction(48, AddressMode.Relative, 2, 2, 1, bmi), // 30
				new CPUInstruction(49, AddressMode.IndirectIndexed, 2, 5, 1, and), // 31
				new CPUInstruction(50, AddressMode.Implied, 0, 2, 0, ___), // 32
				new CPUInstruction(51, AddressMode.IndirectIndexed, 2, 8, 0, rla), // 33
				new CPUInstruction(52, AddressMode.ZeroPageX, 2, 4, 0, nop), // 34
				new CPUInstruction(53, AddressMode.ZeroPageX, 2, 4, 0, and), // 35
				new CPUInstruction(54, AddressMode.ZeroPageX, 2, 6, 0, rol), // 36
				new CPUInstruction(55, AddressMode.ZeroPageX, 2, 6, 0, rla), // 37
				new CPUInstruction(56, AddressMode.Implied, 1, 2, 0, sec), // 38
				new CPUInstruction(57, AddressMode.AbsoluteY, 3, 4, 1, and), // 39
				new CPUInstruction(58, AddressMode.Implied, 1, 2, 0, nop), // 3A
				new CPUInstruction(59, AddressMode.AbsoluteY, 3, 7, 0, rla), // 3B
				new CPUInstruction(60, AddressMode.AbsoluteX, 3, 4, 1, nop), // 3C
				new CPUInstruction(61, AddressMode.AbsoluteX, 3, 4, 1, and), // 3D
				new CPUInstruction(62, AddressMode.AbsoluteX, 3, 7, 0, rol), // 3E
				new CPUInstruction(63, AddressMode.AbsoluteX, 3, 7, 0, rla), // 3F
				new CPUInstruction(64, AddressMode.Implied, 1, 6, 0, rti), // 40
				new CPUInstruction(65, AddressMode.IndexedIndirect, 2, 6, 0, eor), // 41
				new CPUInstruction(66, AddressMode.Implied, 0, 2, 0, ___), // 42
				new CPUInstruction(67, AddressMode.IndexedIndirect, 2, 8, 0, lse), // 43
				new CPUInstruction(68, AddressMode.ZeroPage, 2, 3, 0, nop), // 44
				new CPUInstruction(69, AddressMode.ZeroPage, 2, 3, 0, eor), // 45
				new CPUInstruction(70, AddressMode.ZeroPage, 2, 5, 0, lsr), // 46
				new CPUInstruction(71, AddressMode.ZeroPage, 2, 5, 0, lse), // 47
				new CPUInstruction(72, AddressMode.Implied, 1, 3, 0, pha), // 48
				new CPUInstruction(73, AddressMode.Immediate, 2, 2, 0, eor), // 49
				new CPUInstruction(74, AddressMode.Accumulator, 1, 2, 0, lsr), // 4A
				new CPUInstruction(75, AddressMode.Immediate, 2, 2, 0, alr), // 4B
				new CPUInstruction(76, AddressMode.Absolute, 3, 3, 0, jmp), // 4C
				new CPUInstruction(77, AddressMode.Absolute, 3, 4, 0, eor), // 4D
				new CPUInstruction(78, AddressMode.Absolute, 3, 6, 0, lsr), // 4E
				new CPUInstruction(79, AddressMode.Absolute, 3, 6, 0, lse), // 4F
				new CPUInstruction(80, AddressMode.Relative, 2, 2, 1, bvc), // 50
				new CPUInstruction(81, AddressMode.IndirectIndexed, 2, 5, 1, eor), // 51
				new CPUInstruction(82, AddressMode.Implied, 0, 2, 0, ___), // 52
				new CPUInstruction(83, AddressMode.IndirectIndexed, 2, 8, 0, lse), // 53
				new CPUInstruction(84, AddressMode.ZeroPageX, 2, 4, 0, nop), // 54
				new CPUInstruction(85, AddressMode.ZeroPageX, 2, 4, 0, eor), // 55
				new CPUInstruction(86, AddressMode.ZeroPageX, 2, 6, 0, lsr), // 56
				new CPUInstruction(87, AddressMode.ZeroPageX, 2, 6, 0, lse), // 57
				new CPUInstruction(88, AddressMode.Implied, 1, 2, 0, cli), // 58
				new CPUInstruction(89, AddressMode.AbsoluteY, 3, 4, 1, eor), // 59
				new CPUInstruction(90, AddressMode.Implied, 1, 2, 0, nop), // 5A
				new CPUInstruction(91, AddressMode.AbsoluteY, 3, 7, 0, lse), // 5B
				new CPUInstruction(92, AddressMode.AbsoluteX, 3, 4, 1, nop), // 5C
				new CPUInstruction(93, AddressMode.AbsoluteX, 3, 4, 1, eor), // 5D
				new CPUInstruction(94, AddressMode.AbsoluteX, 3, 7, 0, lsr), // 5E
				new CPUInstruction(95, AddressMode.AbsoluteX, 3, 7, 0, lse), // 5F
				new CPUInstruction(96, AddressMode.Implied, 1, 6, 0, rts), // 60
				new CPUInstruction(97, AddressMode.IndexedIndirect, 2, 6, 0, adc), // 61
				new CPUInstruction(98, AddressMode.Implied, 0, 2, 0, ___), // 62
				new CPUInstruction(99, AddressMode.IndexedIndirect, 2, 8, 0, rra), // 63
				new CPUInstruction(100, AddressMode.ZeroPage, 2, 3, 0, nop), // 64
				new CPUInstruction(101, AddressMode.ZeroPage, 2, 3, 0, adc), // 65
				new CPUInstruction(102, AddressMode.ZeroPage, 2, 5, 0, ror), // 66
				new CPUInstruction(103, AddressMode.ZeroPage, 2, 5, 0, rra), // 67
				new CPUInstruction(104, AddressMode.Implied, 1, 4, 0, pla), // 68
				new CPUInstruction(105, AddressMode.Immediate, 2, 2, 0, adc), // 69
				new CPUInstruction(106, AddressMode.Accumulator, 1, 2, 0, ror), // 6A
				new CPUInstruction(107, AddressMode.Immediate, 2, 2, 0, arr), // 6B
				new CPUInstruction(108, AddressMode.Indirect, 3, 5, 0, jmp), // 6C
				new CPUInstruction(109, AddressMode.Absolute, 3, 4, 0, adc), // 6D
				new CPUInstruction(110, AddressMode.Absolute, 3, 6, 0, ror), // 6E
				new CPUInstruction(111, AddressMode.Absolute, 3, 6, 0, rra), // 6F
				new CPUInstruction(112, AddressMode.Relative, 2, 2, 1, bvs), // 70
				new CPUInstruction(113, AddressMode.IndirectIndexed, 2, 5, 1, adc), // 71
				new CPUInstruction(114, AddressMode.Implied, 0, 2, 0, ___), // 72
				new CPUInstruction(115, AddressMode.IndirectIndexed, 2, 8, 0, rra), // 73
				new CPUInstruction(116, AddressMode.ZeroPageX, 2, 4, 0, nop), // 74
				new CPUInstruction(117, AddressMode.ZeroPageX, 2, 4, 0, adc), // 75
				new CPUInstruction(118, AddressMode.ZeroPageX, 2, 6, 0, ror), // 76
				new CPUInstruction(119, AddressMode.ZeroPageX, 2, 6, 0, rra), // 77
				new CPUInstruction(120, AddressMode.Implied, 1, 2, 0, sei), // 78
				new CPUInstruction(121, AddressMode.AbsoluteY, 3, 4, 1, adc), // 79
				new CPUInstruction(122, AddressMode.Implied, 1, 2, 0, nop), // 7A
				new CPUInstruction(123, AddressMode.AbsoluteY, 3, 7, 0, rra), // 7B
				new CPUInstruction(124, AddressMode.AbsoluteX, 3, 4, 1, nop), // 7C
				new CPUInstruction(125, AddressMode.AbsoluteX, 3, 4, 1, adc), // 7D
				new CPUInstruction(126, AddressMode.AbsoluteX, 3, 7, 0, ror), // 7E
				new CPUInstruction(127, AddressMode.AbsoluteX, 3, 7, 0, rra), // 7F
				new CPUInstruction(128, AddressMode.Immediate, 2, 2, 0, nop), // 80
				new CPUInstruction(129, AddressMode.IndexedIndirect, 2, 6, 0, sta), // 81
				new CPUInstruction(130, AddressMode.Immediate, 2, 2, 0, nop), // 82
				new CPUInstruction(131, AddressMode.IndexedIndirect, 2, 6, 0, sax), // 83
				new CPUInstruction(132, AddressMode.ZeroPage, 2, 3, 0, sty), // 84
				new CPUInstruction(133, AddressMode.ZeroPage, 2, 3, 0, sta), // 85
				new CPUInstruction(134, AddressMode.ZeroPage, 2, 3, 0, stx), // 86
				new CPUInstruction(135, AddressMode.ZeroPage, 2, 3, 0, sax), // 87
				new CPUInstruction(136, AddressMode.Implied, 1, 2, 0, dey), // 88
				new CPUInstruction(137, AddressMode.Immediate, 2, 2, 0, nop), // 89
				new CPUInstruction(138, AddressMode.Implied, 1, 2, 0, txa), // 8A
				new CPUInstruction(139, AddressMode.Immediate, 0, 2, 0, ane), // 8B
				new CPUInstruction(140, AddressMode.Absolute, 3, 4, 0, sty), // 8C
				new CPUInstruction(141, AddressMode.Absolute, 3, 4, 0, sta), // 8D
				new CPUInstruction(142, AddressMode.Absolute, 3, 4, 0, stx), // 8E
				new CPUInstruction(143, AddressMode.Absolute, 3, 4, 0, sax), // 8F
				new CPUInstruction(144, AddressMode.Relative, 2, 2, 1, bcc), // 90
				new CPUInstruction(145, AddressMode.IndirectIndexed, 2, 6, 0, sta), // 91
				new CPUInstruction(146, AddressMode.Implied, 0, 2, 0, ___), // 92
				new CPUInstruction(147, AddressMode.IndirectIndexed, 0, 6, 0, sha), // 93
				new CPUInstruction(148, AddressMode.ZeroPageX, 2, 4, 0, sty), // 94
				new CPUInstruction(149, AddressMode.ZeroPageX, 2, 4, 0, sta), // 95
				new CPUInstruction(150, AddressMode.ZeroPageY, 2, 4, 0, stx), // 96
				new CPUInstruction(151, AddressMode.ZeroPageY, 2, 4, 0, sax), // 97
				new CPUInstruction(152, AddressMode.Implied, 1, 2, 0, tya), // 98
				new CPUInstruction(153, AddressMode.AbsoluteY, 3, 5, 0, sta), // 99
				new CPUInstruction(154, AddressMode.Implied, 1, 2, 0, txs), // 9A
				new CPUInstruction(155, AddressMode.AbsoluteY, 3, 5, 0, tas), // 9B
				new CPUInstruction(156, AddressMode.AbsoluteX, 3, 5, 0, shy), // 9C
				new CPUInstruction(157, AddressMode.AbsoluteX, 3, 5, 0, sta), // 9D
				new CPUInstruction(158, AddressMode.AbsoluteY, 0, 5, 0, shx), // 9E
				new CPUInstruction(159, AddressMode.AbsoluteY, 0, 5, 0, sha), // 9F
				new CPUInstruction(160, AddressMode.Immediate, 2, 2, 0, ldy), // A0
				new CPUInstruction(161, AddressMode.IndexedIndirect, 2, 6, 0, lda), // A1
				new CPUInstruction(162, AddressMode.Immediate, 2, 2, 0, ldx), // A2
				new CPUInstruction(163, AddressMode.IndexedIndirect, 2, 6, 0, lax), // A3
				new CPUInstruction(164, AddressMode.ZeroPage, 2, 3, 0, ldy), // A4
				new CPUInstruction(165, AddressMode.ZeroPage, 2, 3, 0, lda), // A5
				new CPUInstruction(166, AddressMode.ZeroPage, 2, 3, 0, ldx), // A6
				new CPUInstruction(167, AddressMode.ZeroPage, 2, 3, 0, lax), // A7
				new CPUInstruction(168, AddressMode.Implied, 1, 2, 0, tay), // A8
				new CPUInstruction(169, AddressMode.Immediate, 2, 2, 0, lda), // A9
				new CPUInstruction(170, AddressMode.Implied, 1, 2, 0, tax), // AA
				new CPUInstruction(171, AddressMode.Absolute, 2, 4, 0, oal), // AB
				new CPUInstruction(172, AddressMode.Absolute, 3, 4, 0, ldy), // AC
				new CPUInstruction(173, AddressMode.Absolute, 3, 4, 0, lda), // AD
				new CPUInstruction(174, AddressMode.Absolute, 3, 4, 0, ldx), // AE
				new CPUInstruction(175, AddressMode.Absolute, 3, 4, 0, lax), // AF
				new CPUInstruction(176, AddressMode.Relative, 2, 2, 1, bcs), // B0
				new CPUInstruction(177, AddressMode.IndirectIndexed, 2, 5, 1, lda), // B1
				new CPUInstruction(178, AddressMode.Implied, 0, 2, 0, ___), // B2
				new CPUInstruction(179, AddressMode.IndirectIndexed, 2, 5, 1, lax), // B3
				new CPUInstruction(180, AddressMode.ZeroPageX, 2, 4, 0, ldy), // B4
				new CPUInstruction(181, AddressMode.ZeroPageX, 2, 4, 0, lda), // B5
				new CPUInstruction(182, AddressMode.ZeroPageY, 2, 4, 0, ldx), // B6
				new CPUInstruction(183, AddressMode.ZeroPageY, 2, 4, 0, lax), // B7
				new CPUInstruction(184, AddressMode.Implied, 1, 2, 0, clv), // B8
				new CPUInstruction(185, AddressMode.AbsoluteY, 3, 4, 1, lda), // B9
				new CPUInstruction(186, AddressMode.Implied, 1, 2, 0, tsx), // BA
				new CPUInstruction(187, AddressMode.AbsoluteY, 3, 4, 1, las), // BB
				new CPUInstruction(188, AddressMode.AbsoluteX, 3, 4, 1, ldy), // BC
				new CPUInstruction(189, AddressMode.AbsoluteX, 3, 4, 1, lda), // BD
				new CPUInstruction(190, AddressMode.AbsoluteY, 3, 4, 1, ldx), // BE
				new CPUInstruction(191, AddressMode.AbsoluteY, 3, 4, 1, lax), // BF
				new CPUInstruction(192, AddressMode.Immediate, 2, 2, 0, cpy), // C0
				new CPUInstruction(193, AddressMode.IndexedIndirect, 2, 6, 0, cmp), // C1
				new CPUInstruction(194, AddressMode.Immediate, 2, 2, 0, nop), // C2
				new CPUInstruction(195, AddressMode.IndexedIndirect, 2, 8, 0, dcp), // C3
				new CPUInstruction(196, AddressMode.ZeroPage, 2, 3, 0, cpy), // C4
				new CPUInstruction(197, AddressMode.ZeroPage, 2, 3, 0, cmp), // C5
				new CPUInstruction(198, AddressMode.ZeroPage, 2, 5, 0, dec), // C6
				new CPUInstruction(199, AddressMode.ZeroPage, 2, 5, 0, dcp), // C7
				new CPUInstruction(200, AddressMode.Implied, 1, 2, 0, iny), // C8
				new CPUInstruction(201, AddressMode.Immediate, 2, 2, 0, cmp), // C9
				new CPUInstruction(202, AddressMode.Implied, 1, 2, 0, dex), // CA
				new CPUInstruction(203, AddressMode.Immediate, 2, 2, 0, sbx), // CB
				new CPUInstruction(204, AddressMode.Absolute, 3, 4, 0, cpy), // CC
				new CPUInstruction(205, AddressMode.Absolute, 3, 4, 0, cmp), // CD
				new CPUInstruction(206, AddressMode.Absolute, 3, 6, 0, dec), // CE
				new CPUInstruction(207, AddressMode.Absolute, 3, 6, 0, dcp), // CF
				new CPUInstruction(208, AddressMode.Relative, 2, 2, 1, bne), // D0
				new CPUInstruction(209, AddressMode.IndirectIndexed, 2, 5, 1, cmp), // D1
				new CPUInstruction(210, AddressMode.Implied, 0, 2, 0, ___), // D2
				new CPUInstruction(211, AddressMode.IndirectIndexed, 2, 8, 0, dcp), // D3
				new CPUInstruction(212, AddressMode.ZeroPageX, 2, 4, 0, nop), // D4
				new CPUInstruction(213, AddressMode.ZeroPageX, 2, 4, 0, cmp), // D5
				new CPUInstruction(214, AddressMode.ZeroPageX, 2, 6, 0, dec), // D6
				new CPUInstruction(215, AddressMode.ZeroPageX, 2, 6, 0, dcp), // D7
				new CPUInstruction(216, AddressMode.Implied, 1, 2, 0, cld), // D8
				new CPUInstruction(217, AddressMode.AbsoluteY, 3, 4, 1, cmp), // D9
				new CPUInstruction(218, AddressMode.Implied, 1, 2, 0, nop), // DA
				new CPUInstruction(219, AddressMode.AbsoluteY, 3, 7, 0, dcp), // DB
				new CPUInstruction(220, AddressMode.AbsoluteX, 3, 4, 1, nop), // DC
				new CPUInstruction(221, AddressMode.AbsoluteX, 3, 4, 1, cmp), // DD
				new CPUInstruction(222, AddressMode.AbsoluteX, 3, 7, 0, dec), // DE
				new CPUInstruction(223, AddressMode.AbsoluteX, 3, 7, 0, dcp), // DF
				new CPUInstruction(224, AddressMode.Immediate, 2, 2, 0, cpx), // E0
				new CPUInstruction(225, AddressMode.IndexedIndirect, 2, 6, 0, sbc), // E1
				new CPUInstruction(226, AddressMode.Immediate, 2, 2, 0, nop), // E2
				new CPUInstruction(227, AddressMode.IndexedIndirect, 2, 8, 0, isc), // E3
				new CPUInstruction(228, AddressMode.ZeroPage, 2, 3, 0, cpx), // E4
				new CPUInstruction(229, AddressMode.ZeroPage, 2, 3, 0, sbc), // E5
				new CPUInstruction(230, AddressMode.ZeroPage, 2, 5, 0, inc), // E6
				new CPUInstruction(231, AddressMode.ZeroPage, 2, 5, 0, isc), // E7
				new CPUInstruction(232, AddressMode.Implied, 1, 2, 0, inx), // E8
				new CPUInstruction(233, AddressMode.Immediate, 2, 2, 0, sbc), // E9
				new CPUInstruction(234, AddressMode.Implied, 1, 2, 0, nop), // EA
				new CPUInstruction(235, AddressMode.Immediate, 2, 2, 0, sbc), // EB
				new CPUInstruction(236, AddressMode.Absolute, 3, 4, 0, cpx), // EC
				new CPUInstruction(237, AddressMode.Absolute, 3, 4, 0, sbc), // ED
				new CPUInstruction(238, AddressMode.Absolute, 3, 6, 0, inc), // EE
				new CPUInstruction(239, AddressMode.Absolute, 3, 6, 0, isc), // EF
				new CPUInstruction(240, AddressMode.Relative, 2, 2, 1, beq), // F0
				new CPUInstruction(241, AddressMode.IndirectIndexed, 2, 5, 1, sbc), // F1
				new CPUInstruction(242, AddressMode.Implied, 0, 2, 0, ___), // F2
				new CPUInstruction(243, AddressMode.IndirectIndexed, 2, 8, 0, isc), // F3
				new CPUInstruction(244, AddressMode.ZeroPageX, 2, 4, 0, nop), // F4
				new CPUInstruction(245, AddressMode.ZeroPageX, 2, 4, 0, sbc), // F5
				new CPUInstruction(246, AddressMode.ZeroPageX, 2, 6, 0, inc), // F6
				new CPUInstruction(247, AddressMode.ZeroPageX, 2, 6, 0, isc), // F7
				new CPUInstruction(248, AddressMode.Implied, 1, 2, 0, sed), // F8
				new CPUInstruction(249, AddressMode.AbsoluteY, 3, 4, 1, sbc), // F9
				new CPUInstruction(250, AddressMode.Implied, 1, 2, 0, nop), // FA
				new CPUInstruction(251, AddressMode.AbsoluteY, 3, 7, 0, isc), // FB
				new CPUInstruction(252, AddressMode.AbsoluteX, 3, 4, 1, nop), // FC
				new CPUInstruction(253, AddressMode.AbsoluteX, 3, 4, 1, sbc), // FD
				new CPUInstruction(254, AddressMode.AbsoluteX, 3, 7, 0, inc), // FE
				new CPUInstruction(255, AddressMode.AbsoluteX, 3, 7, 0, isc), // FF
			};
		}

		/// <summary>
		/// Resets this CPU to its power on state.
		/// </summary>
		public void Reset()
		{
			PC = _memory.Read16(0xFFFC);
			S = 0xFD;
			A = 0;
			X = 0;
			Y = 0;
			SetProcessorFlags(0x24);

			Cycles = 0;
			_idle = 0;

			nmiInterrupt = false;
		}

		/// <summary>
		/// Triggers a non maskable interrupt on this CPU.
		/// </summary>
		public void TriggerNmi()
		{
			nmiInterrupt = true;
		}

		/// <summary>
		/// Triggers an interrupt on this CPU if the interrupt disable flag
		/// is not set.
		/// </summary>
		public void TriggerIrq()
		{
			if (!I)
			{
				irqInterrupt = true;
			}
		}

		/// <summary>
		/// Instructs the CPU to idle for the specified number of cycles.
		/// </summary>
		/// <param name="idleCycles">Idle cycles.</param>
		public void AddIdleCycles(int idleCycles)
		{
			_idle += idleCycles;
		}

		/// <summary>
		/// Executes the next CPU instruction specified by the Program Counter.
		/// </summary>
		/// <returns>The number of CPU cycles excuted</returns>
		public int Step()
		{
			if (_idle > 0)
			{
				_idle--;
				return 1;
			}

			if (irqInterrupt)
			{
				Irq();
			}

			irqInterrupt = false;

			if (nmiInterrupt)
			{
				Nmi();
			}

			nmiInterrupt = false;

			var cyclesOrig = Cycles;
			var opCode = _memory.Read(PC);
			var currentInstruction = _instructions[opCode];
			var mode = currentInstruction.AddressMode;

			// Get address to operate on
			ushort address = 0;
			var pageCrossed = false;
			switch (mode)
			{
				case AddressMode.Implied:
					break;
				case AddressMode.Immediate:
					address = (ushort)(PC + 1);
					break;
				case AddressMode.Absolute:
					address = _memory.Read16((ushort)(PC + 1));
					break;
				case AddressMode.AbsoluteX:
					address = (ushort)(_memory.Read16((ushort)(PC + 1)) + X);
					pageCrossed = IsPageCross((ushort)(address - X), X);
					break;
				case AddressMode.AbsoluteY:
					address = (ushort)(_memory.Read16((ushort)(PC + 1)) + Y);
					pageCrossed = IsPageCross((ushort)(address - Y), Y);
					break;
				case AddressMode.Accumulator:
					break;
				case AddressMode.Relative:
					address = (ushort)(PC + (sbyte)_memory.Read((ushort)(PC + 1)) + 2);
					break;
				case AddressMode.ZeroPage:
					address = _memory.Read((ushort)(PC + 1));
					break;
				case AddressMode.ZeroPageY:
					address = (ushort)((_memory.Read((ushort)(PC + 1)) + Y) & 0xFF);
					break;
				case AddressMode.ZeroPageX:
					address = (ushort)((_memory.Read((ushort)(PC + 1)) + X) & 0xFF);
					break;
				case AddressMode.Indirect:
					// Must wrap if at the end of a page to emulate a 6502 bug present in the JMP instruction
					address = _memory.Read16WrapPage(_memory.Read16((ushort)(PC + 1)));
					break;
				case AddressMode.IndexedIndirect:
					// Zeropage address of lower nibble of target address (& 0xFF to wrap at 255)
					var lowerNibbleAddress = (ushort)((_memory.Read((ushort)(PC + 1)) + X) & 0xFF);

					// Target address (Must wrap to 0x00 if at 0xFF)
					address = _memory.Read16WrapPage(lowerNibbleAddress);
					break;
				case AddressMode.IndirectIndexed:
					// Zeropage address of the value to add the Y register to to get the target address
					var valueAddress = (ushort)_memory.Read((ushort)(PC + 1));

					// Target address (Must wrap to 0x00 if at 0xFF)
					address = (ushort)(_memory.Read16WrapPage(valueAddress) + Y);
					pageCrossed = IsPageCross((ushort)(address - Y), address);
					break;
			}
			PC += (ushort)currentInstruction.InstructionSize;
			Cycles += currentInstruction.InstructionCycles;
			if (pageCrossed)
			{
				Cycles += currentInstruction.InstructionPageCycles;
			}

#if DEBUG
			if (currentInstruction.Instruction == ___)
			{
				System.Console.WriteLine($"Invalid opcode: {opCode.ToString("X2")} ({opCode})");
			}
			currentInstruction.Instruction.Invoke(mode, address);
#else
			currentInstruction.Instruction.Invoke(mode, address);
#endif

			return Cycles - cyclesOrig;
		}

		private void SetZn(byte value)
		{
			Z = value == 0;
			N = ((value >> 7) & 1) == 1;
		}

		private void SetCarry(uint value)
		{
			C = value > 0xff;
		}

		private void SetCarrySubstract(uint value)
		{
			C = value < 0x100;
		}

		private bool IsBitSet(byte value, int index)
		{
			return (value & (1 << index)) != 0;
		}

		private byte PullStack()
		{
			S++;
			var data = _memory.Read((ushort)(0x0100 | S));
			return data;
		}

		private void PushStack(byte data)
		{
			_memory.Write((ushort)(0x100 | S), data);
			S--;
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

			if (C)
			{
				flags |= 1 << 0; // Carry flag, bit 0
			}

			if (Z)
			{
				flags |= 1 << 1; // Zero flag, bit 1
			}

			if (I)
			{
				flags |= 1 << 2; // Interrupt disable flag, bit 2
			}

			if (D)
			{
				flags |= 1 << 3; // Decimal mode flag, bit 3
			}

			if (B)
			{
				flags |= 1 << 4; // Break mode, bit 4
			}

			flags |= 1 << 5; // Bit 5, always set
			if (V)
			{
				flags |= 1 << 6; // Overflow flag, bit 6
			}

			if (N)
			{
				flags |= 1 << 7; // Negative flag, bit 7
			}

			return flags;
		}

		private void SetProcessorFlags(byte flags)
		{
			C = IsBitSet(flags, 0);
			Z = IsBitSet(flags, 1);
			I = IsBitSet(flags, 2);
			D = IsBitSet(flags, 3);
			B = IsBitSet(flags, 4);
			V = IsBitSet(flags, 6);
			N = IsBitSet(flags, 7);
		}

		private bool IsPageCross(ushort a, ushort b)
		{
			return (a & 0xFF) != (b & 0xFF);
		}

		private void HandleBranchCycles(ushort origPc, ushort branchPc)
		{
			Cycles++;
			Cycles += IsPageCross(origPc, branchPc) ? 1 : 0;
		}

		private void Nmi()
		{
			PushStack16(PC);
			PushStack(GetStatusFlags());
			PC = _memory.Read16(0xFFFA);
			I = true;
		}

		private void Irq()
		{
			PushStack16(PC);
			PushStack(GetStatusFlags());
			PC = _memory.Read16(0xFFFE);
			I = true;
		}

		// Illegal opcode, throw exception
		private void ___(AddressMode mode, ushort address)
		{
			throw new Exception("Illegal Opcode");
		}

		// INSTRUCTIONS FOLLOW
		// BRK - Force Interrupt
		private void brk(AddressMode mode, ushort address)
		{
			PushStack16(PC);
			PushStack(GetStatusFlags());
			B = true;
			PC = _memory.Read16(0xFFFE);
		}

		// ROR - Rotate Right
		private void ror(AddressMode mode, ushort address)
		{
			var Corig = C;
			if (mode == AddressMode.Accumulator)
			{
				C = IsBitSet(A, 0);
				A >>= 1;
				A |= (byte)(Corig ? 0x80 : 0);

				SetZn(A);
			}
			else
			{
				var data = _memory.Read(address);
				C = IsBitSet(data, 0);

				data >>= 1;
				data |= (byte)(Corig ? 0x80 : 0);

				_memory.Write(address, data);

				SetZn(data);
			}
		}

		// RTI - Return from Interrupt
		private void rti(AddressMode mode, ushort address)
		{
			SetProcessorFlags(PullStack());
			PC = PullStack16();
		}

		#region Invalid opcodes

		// RLA (Rotate Left then AND with Accumulator)
		private void rla(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			C = (data & 0x80) != 0;
			var rotatedValue = (byte)((data << 1) | (C ? 1 : 0));
			A &= rotatedValue;
			SetZn(A);
		}

		// LSE (LSR then EOR) Absolute addressing mode
		private void lse(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);

			C = (data & 0x01) != 0;
			// Shift right operation
			byte shiftedValue = (byte)(data >> 1); 
			// Perform Exclusive OR (EOR) operation with Accumulator
			A ^= shiftedValue;

			// Set flags based on the result (e.g., update zero, negative, carry flags)
			SetZn(A);
		}

		// RRA (ROR then ADC) Absolute addressing mode
		private void rra(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			// Perform Rotate Right (ROR) operation on the value
			byte rotatedValue = (byte)((data >> 1) | (C ? 0x80 : 0x00));
			C = (data & 0x01) != 0; // Update carry flag with the old bit 0

			// Perform Add with Carry (ADC) operation with Accumulator
			int sum = A + rotatedValue + (C ? 1 : 0);
			A = (byte)sum;

			// Update flags based on the result (e.g., update zero, negative, carry flags)
			SetZn(A);
		}

		// DCP (DCM) Absolute addressing mode
		private void dcp(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			data--;
			_memory.Write(address, data);
			var result = A - data;
			C = A >= data;
			SetZn((byte)result);
		}

		// ISC Zero Page addressing mode
		private void isc(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			data++;
			_memory.Write(address, data);

			int temp = A - data - (C ? 0 : 1);

			// Set carry flag: if no borrow occurred, set carry flag; otherwise, clear it
			C = temp >= 0;

			// Set zero flag: if result == 0, set zero flag; otherwise, clear it
			Z = (temp & 0xFF) == 0;

			// Set overflow flag: if result is out of signed byte range, set overflow flag; otherwise, clear it
			V = ((A ^ temp) & 0x80) != 0 && ((A ^ data) & 0x80) != 0;

			// Set negative flag: if the high bit of result is set, set negative flag; otherwise, clear it
			N = (temp & 0x80) != 0;

			// Store result in Accumulator
			A = (byte)(temp & 0xFF);
		}

		// ALR Immediate addressing mode
		private void alr(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			// 1. Perform AND with the accumulator
			A &= data;

			// 2. Perform a logical shift right on the result
			// Set carry flag to the LSB of the result
			C = (A & 0x01) != 0; 
			A >>= 1;
			SetZn(A);
		}

		private void arr(AddressMode mode, ushort address)
		{
			// Fetch immediate operand
			byte data = _memory.Read(address);

			// Perform AND operation
			A &= data;

			// Rotate right A
			var Corig = C;
			C = (A & 0x01) != 0; // New carry flag is the old bit 0 of A
			A >>= 1;
			if (Corig)
			{
				A |= 0x80; // Set bit 7 if old carry was set
			}
			SetZn(A);
		}

		// ANE Immediate addressing mode
		private void ane(AddressMode mode, ushort address)
		{
			byte data = _memory.Read(address);
			// Perform AND with the accumulator and X register, then AND with 0xEF
			A = (byte)((A & data & X) & 0xEF);
			SetZn(A);
		}

		// OAL Immediate addressing mode
		private void oal(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			// Perform AND with the accumulator and the immediate value, then transfer to X register
			A = (byte)((A | 0xFF) & data);
			X = A;
			SetZn(A);
		}

		// SAX (Store Accumulator AND X) Absolute addressing mode
		private void sax(AddressMode mode, ushort address)
		{
			// Perform bitwise AND of Accumulator (A) and X register
			var result = (byte)(A & X);
			// Store the result in memory at the specified address
			_memory.Write(address, result);
			SetZn((byte)result);
		}

		private void sbx(AddressMode mode, ushort address)
		{
			// Fetch immediate value
			var data = _memory.Read(address);

			// Perform AND operation between A and X, then subtract immediateValue
			var result = (A & X) - data;

			// Store the result in X register
			X = (byte)result;

			// Update flags
			SetZn(X);
		}

		private void tas(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			S = (byte)(X & A);
			_memory.Write(address, 0);
		}

		private void shy(AddressMode mode, ushort address)
		{
			var data = (byte)((Y & 0xFF00) >> 8);
			_memory.Write(address, data);
		}

		private void sha(AddressMode mode, ushort address)
		{
			_memory.Write(address, 0);
		}

		// ANC (And with Carry) with Immediate Addressing
		private void anc(AddressMode mode, ushort address)
		{
			// Fetch the immediate operand
			var data = _memory.Read(address);

			// Perform the AND operation
			byte result = (byte)(A & data);

			// Store the result in the accumulator
			A = result;

			// Set the carry flag based on the result
			C = (result != 0) ? true : false;

			// Set other flags as needed (ZeroFlag, NegativeFlag, OverflowFlag, etc.)
			// Update CPU cycles, memory access, etc. as per the 6502 specification
			SetZn(A);
		}

		private void las(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			byte result = (byte)(data & S);

			// Set A, X, and Stack Pointer (SP) to the result
			A = result;
			X = result;
			S = result;
			SetZn(result);
		}

		private void shx(AddressMode mode, ushort address)
		{
			_memory.Write(address, 0);
		}

		#endregion

		// ASO (Arithmetic Shift Left followed by OR with Accumulator) Absolute addressing mode
		private void aso(AddressMode mode, ushort address)
		{
			// Read data from memory at the specified address
			byte value = _memory.Read(address);

			// Perform ASL (Arithmetic Shift Left) on the value
			var shiftedValue = (byte)(value << 1);

			// Perform OR with Accumulator
			A |= shiftedValue;

			// Set flags based on the result (e.g., update zero, negative, carry flags)
			SetZn(shiftedValue);

			// Write back the result to memory (if necessary)
			_memory.Write(address, shiftedValue);
			//WriteMemory(address, shiftedValue);
		}

		// TXS - Transfer X to Stack Pointer
		private void txs(AddressMode mode, ushort address)
		{
			S = X;
		}

		// TSX - Transfer Stack Pointer to X
		private void tsx(AddressMode mode, ushort address)
		{
			X = S;
			SetZn(X);
		}

		// TXA - Transfer X to Accumulator
		private void txa(AddressMode mode, ushort address)
		{
			A = X;
			SetZn(A);
		}

		// TYA - Transfer Y to Accumulator
		private void tya(AddressMode mode, ushort address)
		{
			A = Y;
			SetZn(A);
		}

		// TAY - Transfer Accumulator to Y
		private void tay(AddressMode mode, ushort address)
		{
			Y = A;
			SetZn(Y);
		}

		// TAX  - Transfer Accumulator to X
		private void tax(AddressMode mode, ushort address)
		{
			X = A;
			SetZn(X);
		}

		// DEX - Deincrement X
		private void dex(AddressMode mode, ushort address)
		{
			X--;
			SetZn(X);
		}

		// DEY - Deincrement Y
		private void dey(AddressMode mode, ushort address)
		{
			Y--;
			SetZn(Y);
		}

		// INX - Increment X
		private void inx(AddressMode mode, ushort address)
		{
			X++;
			SetZn(X);
		}

		// INY - Increment Y
		private void iny(AddressMode mode, ushort address)
		{
			Y++;
			SetZn(Y);
		}

		// STY - Store Y Register
		private void sty(AddressMode mode, ushort address)
		{
			_memory.Write(address, Y);
		}

		// CPX - Compare X Register
		private void cpx(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			SetZn((byte)(X - data));
			C = X >= data;
		}

		// CPX - Compare Y Register
		private void cpy(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			SetZn((byte)(Y - data));
			C = Y >= data;
		}

		// SBC - Subtract with Carry
		private void sbc(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			var notCarry = !C ? 1 : 0;

			var result = (byte)(A - data - notCarry);
			SetZn(result);

			// If an overflow occurs (result actually less than 0)
			// the carry flag is cleared
			C = (A - data - notCarry) >= 0 ? true : false;

			V = ((A ^ data) & (A ^ result) & 0x80) != 0;

			A = result;
		}

		// ADC - Add with Carry
		private void adc(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			var carry = C ? 1 : 0;

			var sum = (byte)(A + data + carry);
			SetZn(sum);

			C = (A + data + carry) > 0xFF;

			// Sign bit is wrong if sign bit of operands is same
			// and sign bit of result is different
			// if <A and data> differ in sign and <A and sum> have the same sign, set the overflow flag
			// https://stackoverflow.com/questions/29193303/6502-emulation-proper-way-to-implement-adc-and-sbc
			V = (~(A ^ data) & (A ^ sum) & 0x80) != 0;

			A = sum;
		}

		// EOR - Exclusive OR
		private void eor(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			A ^= data;
			SetZn(A);
		}

		// CLV - Clear Overflow Flag
		private void clv(AddressMode mode, ushort address)
		{
			V = false;
		}

		// BMI - Branch if Minus
		private void bmi(AddressMode mode, ushort address)
		{
			PC = N ? address : PC;
		}

		// PLP - Pull Processor Status
		private void plp(AddressMode mode, ushort address)
		{
			SetProcessorFlags((byte)(PullStack() & ~0x10));
		}

		// CLD - Clear Decimal Mode
		private void cld(AddressMode mode, ushort address)
		{
			D = false;
		}

		// CMP - Compare
		private void cmp(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			C = A >= data;
			SetZn((byte)(A - data));
		}

		// AND - Logical AND
		private void and(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			A &= data;
			SetZn(A);
		}

		// PLA - Pull Accumulator
		private void pla(AddressMode mode, ushort address)
		{
			A = PullStack();
			SetZn(A);
		}

		// PHP - Push Processor Status
		private void php(AddressMode mode, ushort address)
		{
			PushStack((byte)(GetStatusFlags() | 0x10));
		}

		// SED - Set Decimal Flag
		private void sed(AddressMode mode, ushort address)
		{
			D = true;
		}

		// CLI - Clear Interrupt Disable
		private void cli(AddressMode mode, ushort address)
		{
			I = false;
		}

		// SEI - Set Interrupt Disable
		private void sei(AddressMode mode, ushort address)
		{
			I = true;
		}

		// DEC - Deincrement Memory
		private void dec(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			data--;
			_memory.Write(address, data);
			SetZn(data);
		}

		// INC - Increment Memory
		private void inc(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			data++;
			_memory.Write(address, data);
			SetZn(data);
		}

		// RTS - Return from Subroutine
		private void rts(AddressMode mode, ushort address)
		{
			PC = (ushort)(PullStack16() + 1);
		}

		// JSR - Jump to Subroutine
		private void jsr(AddressMode mode, ushort address)
		{
			PushStack16((ushort)(PC - 1));
			PC = address;
		}

		// BPL - Branch if Positive
		private void bpl(AddressMode mode, ushort address)
		{
			if (!N)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// BVC - Branch if Overflow Clear
		private void bvc(AddressMode mode, ushort address)
		{
			if (!V)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// BVS - Branch if Overflow Set
		private void bvs(AddressMode mode, ushort address)
		{
			if (V)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// BIT - Bit Test
		private void bit(AddressMode mode, ushort address)
		{
			var data = _memory.Read(address);
			N = IsBitSet(data, 7);
			V = IsBitSet(data, 6);
			Z = (data & A) == 0;
		}

		// BNE - Branch if Not Equal
		private void bne(AddressMode mode, ushort address)
		{
			if (!Z)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// BEQ - Branch if Equal
		private void beq(AddressMode mode, ushort address)
		{
			if (Z)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// CLC - Clear Carry Flag
		private void clc(AddressMode mode, ushort address)
		{
			C = false;
		}

		// BCC - Branch if Carry Clear
		private void bcc(AddressMode mode, ushort address)
		{
			if (!C)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// BCs - Branch if Carry Set
		private void bcs(AddressMode mode, ushort address)
		{
			if (C)
			{
				HandleBranchCycles(PC, address);
				PC = address;
			}
		}

		// SEC - Set Carry Flag
		private void sec(AddressMode mode, ushort address)
		{
			C = true;
		}

		// NOP - No Operation
		private void nop(AddressMode mode, ushort address)
		{

		}

		// STX - Store X Register
		private void stx(AddressMode mode, ushort address)
		{
			_memory.Write(address, X);
		}

		// LAX (Load Accumulator and X Register with Memory)
		private void lax(AddressMode mode, ushort address)
		{
			// Fetch absolute address
			A = _memory.Read(address);
			// Load X register with the same value as A
			X = A;
			SetZn(A);
		}

		// LDY - Load Y Register
		private void ldy(AddressMode mode, ushort address)
		{
			Y = _memory.Read(address);
			SetZn(Y);
		}

		// LDX - Load X Register
		private void ldx(AddressMode mode, ushort address)
		{
			X = _memory.Read(address);
			SetZn(X);
		}

		// JMP - Jump
		private void jmp(AddressMode mode, ushort address)
		{
			PC = address;
		}

		// STA - Store Accumulator
		private void sta(AddressMode mode, ushort address)
		{
			_memory.Write(address, A);
		}

		// ORA - Logical Inclusive OR
		private void ora(AddressMode mode, ushort address)
		{
			A |= _memory.Read(address);
			SetZn(A);
		}

		// LDA - Load A Register
		private void lda(AddressMode mode, ushort address)
		{
			A = _memory.Read(address);
			SetZn(A);
		}

		// PHA - Push Accumulator
		private void pha(AddressMode mode, ushort address)
		{
			PushStack(A);
		}

		// ASL - Arithmetic Shift Left
		private void asl(AddressMode mode, ushort address)
		{
			if (mode == AddressMode.Accumulator)
			{
				C = IsBitSet(A, 7);
				A <<= 1;
				SetZn(A);
			}
			else
			{
				var data = _memory.Read(address);
				C = IsBitSet(data, 7);
				var dataUpdated = (byte)(data << 1);
				_memory.Write(address, dataUpdated);
				SetZn(dataUpdated);
			}
		}

		// ROL - Rotate Left
		private void rol(AddressMode mode, ushort address)
		{
			var Corig = C;
			if (mode == AddressMode.Accumulator)
			{
				C = IsBitSet(A, 7);
				A <<= 1;
				A |= (byte)(Corig ? 1 : 0);

				SetZn(A);
			}
			else
			{
				var data = _memory.Read(address);
				C = IsBitSet(data, 7);

				data <<= 1;
				data |= (byte)(Corig ? 1 : 0);

				_memory.Write(address, data);

				SetZn(data);
			}
		}

		// LSR - Logical Shift Right
		private void lsr(AddressMode mode, ushort address)
		{
			if (mode == AddressMode.Accumulator)
			{
				C = (A & 1) == 1;
				A >>= 1;

				SetZn(A);
			}
			else
			{
				var value = _memory.Read(address);
				C = (value & 1) == 1;

				var updatedValue = (byte)(value >> 1);

				_memory.Write(address, updatedValue);

				SetZn(updatedValue);
			}
		}

		#region Save/Load state

		[Serializable]
		public class CpuState
		{
			public byte A;
			public byte X;
			public byte Y;
			public byte S;
			public ushort PC;
			public bool C;
			public bool Z;
			public bool I;
			public bool D;
			public bool B;
			public bool V;
			public bool N;
			public bool irqInterrupt;
			public bool nmiInterrupt;
			public int Cycles;
			public int _idle;

			public object CpuMemory;
		}

		public object SaveState()
		{
			return new CpuState
			{
				A = A,
				X = X,
				Y = Y,
				S = S,
				PC = PC,
				C = C,
				Z = Z,
				I = I,
				D = D,
				B = B,
				V = V,
				N = N,
				irqInterrupt = irqInterrupt,
				nmiInterrupt = nmiInterrupt,
				Cycles = Cycles,
				_idle = _idle,
				CpuMemory = _memory.SaveState()
			};
		}

		public void LoadState(object stateObj)
		{
			var state = stateObj as CpuState;

			A = state.A;
			X = state.X;
			Y = state.Y;
			S = state.S;
			PC = state.PC;
			C = state.C;
			Z = state.Z;
			I = state.I;
			D = state.D;
			B = state.B;
			V = state.V;
			N = state.N;
			irqInterrupt = state.irqInterrupt;
			nmiInterrupt = state.nmiInterrupt;
			Cycles = state.Cycles;
			_idle = state._idle;

			_memory.LoadState(state.CpuMemory);
		}

		#endregion
	}
}