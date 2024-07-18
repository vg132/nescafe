using System;

namespace Nescafe.Core.Mappers
{
	/// <summary>
	/// Represents Nintendo's CNROM Mapper.
	/// </summary>
	public class CnRomMapper : Mapper
	{
		int _bank0Offset;

		/// <summary>
		/// Construct a new CNROM mapper.
		/// </summary>
		/// <param name="console">the console that this mapper is a part of</param>
		public CnRomMapper(Console console)
		{
			_console = console;

			// PRG Bank 0 is switchable
			_bank0Offset = 0;

			_vramMirroringType = _console.Cartridge.VerticalVramMirroring ? VramMirroring.Vertical : VramMirroring.Horizontal;
		}

		/// <summary>
		/// Read a byte from the specified address.
		/// </summary>
		/// <returns>the byte read from the specified address</returns>
		/// <param name="address">the address to read a byte from</param>
		public override byte Read(ushort address)
		{
			byte data;
			if (address < 0x2000) // CHR ROM or RAM
			{
				data = _console.Cartridge.ReadChr(_bank0Offset + address);
			}
			else if (address >= 0x8000) // PRG ROM stored at $8000 and above
			{
				data = _console.Cartridge.ReadPrgRom(address - 0x8000);
			}
			else
			{
				// Open Bus
				data = 0x00;
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
			if (address < 0x2000) // CHR ROM or RAM
			{
				_console.Cartridge.WriteChr(_bank0Offset + address, data);
			}
			else if (address >= 0x6000 && address < 0x8000)
			{
				// Open Bus
			}
			else if (address >= 0x8000)
			{
				WriteBankSelect(data);
			}
			else
			{
				throw new Exception("Invalid mapper write at address: " + address.ToString("X4"));
			}
		}

		private void WriteBankSelect(byte data)
		{
			_bank0Offset = (data & 0x3) * 0x2000;
		}

		#region Save/Load state

		[Serializable]
		private class UxRomMapperSaveState
		{
			public int Bank0Offset;
		}

		public override object SaveState()
		{
			return new UxRomMapperSaveState
			{
				Bank0Offset = _bank0Offset
			};
		}

		public override void LoadState(object state)
		{
			_bank0Offset = ((UxRomMapperSaveState)state).Bank0Offset;
		}

		#endregion
	}
}
