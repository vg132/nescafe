﻿using System;

namespace Nescafe.Core.Mappers;

/// <summary>
/// Represents Nintendo's MMC3 mapper.
/// </summary>
[Mapper(4)]
public class Mmc3Mapper : Mapper
{
	// Bank select register
	private byte _bank;
	private byte _prgRomMode;
	private byte _chrRomMode;

	// PRG RAM protect register
	private byte _prgRamEnable;
	private byte _prgRamProtect;

	// Bank offsets
	private int[] _chrOffsets;
	private int[] _prgOffsets;

	// Bank registers
	private byte[] _bankRegisters;

	// IRQ enable/disable registers
	private bool _irqEnabled;

	// IRQ counter and reload value
	private int _irqCounter;
	private byte _irqCounterReload;

	/// <summary>
	/// Constructs a new MMC3 mapper.
	/// </summary>
	/// <param name="console">Console.</param>
	public Mmc3Mapper(Console console)
	{
		_console = console;

		_bankRegisters = new byte[8];

		// 6 switchable CHR banks
		_chrOffsets = new int[8];

		// 2 switchable PRG ROM banks, 2 fixed to last and second to last banks
		_prgOffsets = new int[4];

		_prgOffsets[0] = 0;
		_prgOffsets[1] = 0x2000;
		_prgOffsets[2] = ((_console.Cartridge.PrgRomBanks * 2) - 2) * 0x2000;
		_prgOffsets[3] = _prgOffsets[2] + 0x2000;
	}

	/// <summary>
	/// Read a byte from the specified address.
	/// </summary>
	/// <returns>the byte read from the specified address</returns>
	/// <param name="address">the address to read a byte from</param>
	public override byte Read(ushort address)
	{
		byte data;

		if (address < 0x2000)
		{
			var bank = address / 0x400;
			var offset = _chrOffsets[bank] + (address % 0x400);
			data = _console.Cartridge.ReadChr(offset);
		}
		else if (address >= 0x6000 && address < 0x8000) // 8 KB PRG RAM
		{
			data = (byte)(_prgRamEnable == 1 ? _console.Cartridge.ReadPrgRam(address - 0x6000) : 0);
		}
		else if (address <= 0xFFFF) // 8 KB PRG ROM banks
		{
			var bank = (address - 0x8000) / 0x2000;
			var offset = _prgOffsets[bank] + (address % 0x2000);
			data = _console.Cartridge.ReadPrgRom(offset);
		}
		else
		{
			throw new Exception("Invalid mapper read at address " + address.ToString("X4"));
		}

		return data;
	}

	/// <summary>
	/// Writes a byte to the specified address.
	/// </summary>
	/// <param name="address">the address to write a byte to</param>
	/// <param name="data">the byte to write to the address</param>
	public override void Write(ushort address, byte data)
	{
		var even = address % 2 == 0;
		if (address < 0x2000) // CHR
		{
			var bank = address / 0x400;
			var offset = _chrOffsets[bank] + (address % 0x400);
			_console.Cartridge.WriteChr(offset, data);
		}
		else if (address >= 0x6000 && address < 0x8000) // PRG RAM
		{
			if (_prgRamProtect == 0)
			{
				_console.Cartridge.WritePrgRam(address - 0x6000, data);
			}
		}
		else if (address < 0xA000) // $8000-$9FFFF
		{
			if (even)
			{
				WriteBankSelectReg(data);
			}
			else
			{
				WriteBankDataReg(data);
			}
		}
		else if (address < 0xC000) // $A000-$BFFF
		{
			if (even)
			{
				WriteMirroringReg(data);
			}
			else
			{
				WritePrgRamProtectReg(data);
			}
		}
		else if (address < 0xE000) // $C000-$DFFF
		{
			if (even)
			{
				WriteIrqLatchReg(data);
			}
			else
			{
				WriteIrqReloadReg(data);
			}
		}
		else if (address <= 0xFFFF) // $E000-$FFFF
		{
			if (even)
			{
				WriteIrqDisableReg(data);
			}
			else
			{
				WriteIrqEnableReg(data);
			}
		}
	}

	private void WriteBankSelectReg(byte data)
	{
		_bank = (byte)(data & 0x07);
		_prgRomMode = (byte)((data >> 6) & 0x01);
		_chrRomMode = (byte)((data >> 7) & 0x01);
		UpdateBankOffsets();
	}

	private void WriteBankDataReg(byte data)
	{
		_bankRegisters[_bank] = data;
		UpdateBankOffsets();
	}

	/// <summary>
	/// Inform the mapper that a PPU step has occurred.
	/// </summary>
	public override void Step()
	{
		var scanline = _console.Ppu.State.Scanline;
		var cycle = _console.Ppu.State.Cycle;
		var renderingEnabled = _console.Ppu.RenderingEnabled;

		// The choice of 315 as the PPU Cycle to clock A12 on is slightly arbitrary
		// but seems to work alright for most games
		// Read "IRQ Specifics" at https://wiki.nesdev.com/w/index.php/MMC3
		if (renderingEnabled && cycle == 315 && scanline >= 0 && scanline < 240)
		{
			ClockA12();
		}
	}

	private void ClockA12()
	{
		if (_irqCounter == 0)
		{
			_irqCounter = _irqCounterReload;
		}
		else
		{
			_irqCounter--;
			if (_irqCounter == 0 && _irqEnabled)
			{
				_console.Cpu.TriggerIrq();
			}
		}
	}

	private void WriteIrqEnableReg(byte data)
	{
		_irqEnabled = true;
	}

	private void WriteIrqDisableReg(byte data)
	{
		_irqEnabled = false;
	}

	private void WriteIrqReloadReg(byte data)
	{
		_irqCounter = 0;
	}

	private void WriteIrqLatchReg(byte data)
	{
		_irqCounterReload = data;
	}

	private void WritePrgRamProtectReg(byte data)
	{
		_prgRamEnable = (byte)((data >> 7) & 0x01);
		_prgRamProtect = (byte)((data >> 6) & 0x01);
	}

	private void WriteMirroringReg(byte data)
	{
		switch (data & 0x01)
		{
			case 0:
				_vramMirroringType = VramMirroring.Vertical;
				break;
			case 1:
				_vramMirroringType = VramMirroring.Horizontal;
				break;
		}
	}

	private void UpdateBankOffsets()
	{
		switch (_chrRomMode)
		{
			case 0:
				_chrOffsets[0] = (_bankRegisters[0] & 0xFE) * 0x400;
				_chrOffsets[1] = (_bankRegisters[0] | 0x01) * 0x400;
				_chrOffsets[2] = (_bankRegisters[1] & 0xFE) * 0x400;
				_chrOffsets[3] = (_bankRegisters[1] | 0x01) * 0x400;
				_chrOffsets[4] = _bankRegisters[2] * 0x400;
				_chrOffsets[5] = _bankRegisters[3] * 0x400;
				_chrOffsets[6] = _bankRegisters[4] * 0x400;
				_chrOffsets[7] = _bankRegisters[5] * 0x400;
				break;
			case 1:
				_chrOffsets[0] = _bankRegisters[2] * 0x400;
				_chrOffsets[1] = _bankRegisters[3] * 0x400;
				_chrOffsets[2] = _bankRegisters[4] * 0x400;
				_chrOffsets[3] = _bankRegisters[5] * 0x400;
				_chrOffsets[4] = (_bankRegisters[0] & 0xFE) * 0x400;
				_chrOffsets[5] = (_bankRegisters[0] | 0x01) * 0x400;
				_chrOffsets[6] = (_bankRegisters[1] & 0xFE) * 0x400;
				_chrOffsets[7] = (_bankRegisters[1] | 0x01) * 0x400;
				break;
		}

		var secondLastBankOffset = ((_console.Cartridge.PrgRomBanks * 2) - 2) * 0x2000;
		var lastBankOffset = secondLastBankOffset + 0x2000;
		switch (_prgRomMode)
		{
			case 0:
				_prgOffsets[0] = _bankRegisters[6] * 0x2000;
				_prgOffsets[1] = _bankRegisters[7] * 0x2000;
				_prgOffsets[2] = secondLastBankOffset;
				_prgOffsets[3] = lastBankOffset;
				break;
			case 1:
				_prgOffsets[0] = secondLastBankOffset;
				_prgOffsets[1] = _bankRegisters[7] * 0x2000;
				_prgOffsets[2] = _bankRegisters[6] * 0x2000;
				_prgOffsets[3] = lastBankOffset;
				break;
		}
	}

	#region Save/Load state

	[Serializable]
	public class Mmc6MapperState
	{
		public byte Bank;
		public byte PrgRomMode;
		public byte ChrRomMode;
		public byte PrgRamEnable;
		public byte PrgRamProtect;
		public int[] ChrOffsets;
		public int[] PrgOffsets;
		public byte[] BankRegisters;
		public bool IrqEnabled;
		public int IrqCounter;
		public byte IrqCounterReload;
	}

	public override object SaveState()
	{
		lock (_console.FrameLock)
		{
			return new Mmc6MapperState
			{
				Bank = _bank,
				PrgRomMode = _prgRomMode,
				ChrRomMode = _chrRomMode,
				PrgRamEnable = _prgRamEnable,
				PrgRamProtect = _prgRamProtect,
				BankRegisters = _bankRegisters,
				ChrOffsets = _chrOffsets,
				PrgOffsets = _prgOffsets,
				IrqCounter = _irqCounter,
				IrqCounterReload = _irqCounterReload,
				IrqEnabled = _irqEnabled
			};
		}
	}

	public override void LoadState(object stateItem)
	{
		lock (_console.FrameLock)
		{
			var state = stateItem as Mmc6MapperState;
			_bank = state.Bank;
			_prgRomMode = state.PrgRomMode;
			_chrRomMode = state.ChrRomMode;
			_prgRamEnable = state.PrgRamEnable;
			_prgRamProtect = state.PrgRamProtect;
			_bankRegisters = state.BankRegisters;
			_chrOffsets = state.ChrOffsets;
			_prgOffsets = state.PrgOffsets;
			_irqCounter = state.IrqCounter;
			_irqCounterReload = state.IrqCounterReload;
			_irqEnabled = state.IrqEnabled;
		}
	}

	#endregion
}
