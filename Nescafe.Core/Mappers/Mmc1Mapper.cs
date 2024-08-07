﻿using Nescafe.Services;

namespace Nescafe.Core.Mappers;

/// <summary>
/// Represents Nintendo's MMC1 mapper.
/// </summary>
[Mapper(1)]
public class Mmc1Mapper : Mapper
{
	// Common shift register
	private byte _shiftReg;

	// Internal registers
	private byte _controlReg;
	private byte _chr0Reg;
	private byte _chr1Reg;
	private byte _prgReg;

	// Control register
	private byte _prgMode;
	private byte _chrMode;

	// PRG register
	private byte _prgRamEnable;

	// CHR offsets
	private int _chrBank0Offset;
	private int _chrBank1Offset;

	// PRG offsets
	private int _prgBank0Offset;
	private int _prgBank1Offset;

	// Current number of writes to internal shift register
	private int _shiftCount;

	/// <summary>
	/// Construct a new MMC1 mapper.
	/// </summary>
	/// <param name="console">the console that this mapper is a part of</param>
	public Mmc1Mapper(Console console)
	{
		_console = console;

		_shiftReg = 0x0F;
		_controlReg = 0x00;
		_chr0Reg = 0x00;
		_chr1Reg = 0x00;
		_prgReg = 0x00;

		_shiftCount = 0;

		_prgBank1Offset = (_console.Cartridge.PrgRomBanks - 1) * 0x4000;

		_vramMirroringType = VramMirroring.Horizontal;

		WriteControlReg(0x0F);
	}

	/// <summary>
	/// Read a byte from the specified address.
	/// </summary>
	/// <returns>the byte read from the specified address</returns>
	/// <param name="address">the address to read a byte from</param>
	public override byte Read(ushort address)
	{
		byte data;
		if (address <= 0x1FFF) // CHR Banks 0 and 1 $0000-0x0FFF and $1000-$1FFF
		{
			var offset = (address / BankSize) == 0 ? _chrBank0Offset : _chrBank1Offset;
			offset += address % BankSize;
			data = _console.Cartridge.ReadChr(offset);
		}
		else if (address >= 0x4018 && address <= 0x5FFF)
		{
			data = _console.Cartridge.ReadExpRam(address - 0x4018);
		}
		else if (address >= 0x6000 && address <= 0x7FFF) // 8 KB PRG RAM bank (CPU) $6000-$7FFF
		{
			data = _console.Cartridge.ReadPrgRam(address - 0x6000);
		}
		else if (address >= 0x8000 && address <= 0xFFFF) // 2 PRG ROM banks
		{
			//var newAddress = address - 0x8000;
			//var offset = (newAddress / 0x4000) == 0 ? _prgBank0Offset : _prgBank1Offset;
			//offset += newAddress % 0x4000;
			//data = _console.Cartridge.ReadPrgRom(offset);

			address -= 0x8000;
			var offset = (address / 0x4000) == 0 ? _prgBank0Offset : _prgBank1Offset;
			offset += address % 0x4000;
			data = _console.Cartridge.ReadPrgRom(offset);
		}
		else
		{
			LoggingService.LogEvent(NESEvents.Mapper, $"Invalid Mapper read at address {address.ToString("X4")}");
			//throw new Exception("Invalid Mapper read at address " + address.ToString("X4"));
			data = 0;
		}
		return data;
	}

	/// <summary>
	/// Read a byte from the specified address.
	/// </summary>
	/// <returns>the byte read from the specified address</returns>
	/// <param name="address">the address to read a byte from</param>
	/// <param name="data">The data to write to the address</param>
	public override void Write(ushort address, byte data)
	{
		if (address < 0x2000)
		{
			if (!_console.Cartridge.UsesChrRam)
			{
				throw new Exception("Attempt to write to CHR ROM at " + address.ToString("X4"));
			}
			var offset = (address / BankSize) == 0 ? _chrBank0Offset : _chrBank1Offset;
			offset += address % BankSize;
			_console.Cartridge.WriteChr(offset, data);
		}
		else if(address >= 0x4018 && address<= 0x5FFF)
		{
			_console.Cartridge.WriteExpRam(address - 0x4018, data);
		}
		else if (address >= 0x6000 && address <= 0x7FFF)
		{
			_console.Cartridge.WritePrgRam(address - 0x6000, data);
		}
		else if (address >= 0x8000) // Connected to common shift register
		{
			LoadRegister(address, data);
		}
		else
		{
			throw new Exception("Invalid mapper write at address " + address.ToString("X4"));
		}
	}

	private void LoadRegister(ushort address, byte data)
	{
		if ((data & 0x80) != 0)
		{
			// If bit 7 set, clear internal shift register
			_shiftReg = 0;
			_shiftCount = 0;
			WriteControlReg((byte)(_controlReg | 0x0C));
		}
		else
		{
			_shiftReg |= (byte)((data & 1) << _shiftCount);
			_shiftCount++;

			if (_shiftCount == 5)
			{
				_shiftCount = 0;
				WriteRegister(address, _shiftReg);
				_shiftReg = 0;
			}
		}
	}

	private void WriteRegister(ushort address, byte data)
	{
		if (address >= 0x8000 && address <= 0x9FFF)
		{
			WriteControlReg(data);
		}
		else if (address <= 0xBFFF)
		{
			WriteChr0Reg(data);
		}
		else if (address <= 0xDFFF)
		{
			WriteChr1Reg(data);
		}
		else if (address <= 0xFFFF)
		{
			WritePrgReg(data);
		}
		else
		{
			throw new Exception("Invalid MMC1 Register write at address " + address.ToString("X4"));
		}
	}

	private void WriteControlReg(byte data)
	{
		_controlReg = data;
		_prgMode = (byte)((data >> 2) & 0x3);
		_chrMode = (byte)((data >> 4) & 0x01);
		switch (_controlReg & 0x03)
		{
			case 0:
				_vramMirroringType = VramMirroring.SingleLower;
				break;
			case 1:
				_vramMirroringType = VramMirroring.SingleUpper;
				break;
			case 2:
				_vramMirroringType = VramMirroring.Vertical;
				break;
			case 3:
				_vramMirroringType = VramMirroring.Horizontal;
				break;
		}
		UpdateBankOffsets();
	}

	private void WriteChr0Reg(byte data)
	{
		_chr0Reg = data;
		UpdateBankOffsets();
	}

	private void WriteChr1Reg(byte data)
	{
		_chr1Reg = data;
		UpdateBankOffsets();
	}

	private void WritePrgReg(byte data)
	{
		_prgReg = data;
		_prgRamEnable = (byte)((data >> 4) & 0x01);
		UpdateBankOffsets();
	}

	private void UpdateBankOffsets()
	{
		switch (_chrMode)
		{
			case 0: // Switch 8 KB at a time
							// Lowest bit of bank number ignored in 8 Kb mode
				_chrBank0Offset = ((_chr0Reg & 0x1E) >> 1) * BankSize;
				_chrBank1Offset = _chrBank0Offset + BankSize;
				break;
			case 1: // Switch 4 KB at a time
				_chrBank0Offset = _chr0Reg * BankSize;
				_chrBank1Offset = _chr1Reg * BankSize;
				break;
		}
		switch (_prgMode)
		{
			case 0:
			case 1:
				// Lowest bit of bank number is ignored with mode 0 or 1
				_prgBank0Offset = ((_prgReg & 0xE) >> 1) * 0x4000;
				_prgBank1Offset = _prgBank0Offset + 0x4000;
				break;
			case 2:
				// Fix first bank at $8000
				_prgBank0Offset = 0;
				// Switch second bank at $C000
				_prgBank1Offset = (_prgReg & 0xF) * 0x4000;
				break;
			case 3:
				// Switch 16 KB bank at $8000
				_prgBank0Offset = (_prgReg & 0xF) * 0x4000;
				// Fix last bank at $C000
				_prgBank1Offset = (_console.Cartridge.PrgRomBanks - 1) * 0x4000;
				break;
		}
	}

	private int BankSize => _chrMode == 0 ? 0x2000 : 0x1000;

	#region Save/Load state

	[Serializable]
	public class Mmc1MapperState
	{
		public byte _shiftReg;
		public byte _controlReg;
		public byte _chr0Reg;
		public byte _chr1Reg;
		public byte _prgReg;
		public byte _prgMode;
		public byte _chrMode;
		public byte _prgRamEnable;
		public int _chrBank0Offset;
		public int _chrBank1Offset;
		public int _prgBank0Offset;
		public int _prgBank1Offset;
		public int _shiftCount;
	}

	public override object SaveState()
	{
		lock (_console.FrameLock)
		{
			return new Mmc1MapperState
			{
				_chr0Reg = _chr0Reg,
				_chr1Reg = _chr1Reg,
				_prgReg = _prgReg,
				_prgMode = _prgMode,
				_chrMode = _chrMode,
				_prgRamEnable = _prgRamEnable,
				_chrBank0Offset = _chrBank0Offset,
				_chrBank1Offset = _chrBank1Offset,
				_controlReg = _controlReg,
				_prgBank0Offset = _prgBank0Offset,
				_prgBank1Offset = _prgBank1Offset,
				_shiftCount = _shiftCount,
				_shiftReg = _shiftReg
			};
		}
	}

	public override void LoadState(object stateItem)
	{
		lock (_console.FrameLock)
		{
			var state = stateItem as Mmc1MapperState;
			_chr0Reg = state._chr0Reg;
			_chr1Reg = state._chr1Reg;
			_prgReg = state._prgReg;
			_prgMode = state._prgMode;
			_chrMode = state._chrMode;
			_prgRamEnable = state._prgRamEnable;
			_chrBank0Offset = state._chrBank0Offset;
			_chrBank1Offset = state._chrBank1Offset;
			_controlReg = state._controlReg;
			_prgBank0Offset = state._prgBank0Offset;
			_prgBank1Offset = state._prgBank1Offset;
			_shiftCount = state._shiftCount;
			_shiftReg = state._shiftReg;
		}
	}

	#endregion
}
