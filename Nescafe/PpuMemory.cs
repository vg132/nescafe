using System;

namespace Nescafe
{
	/// <summary>
	/// Represents the PPU's memory and memory mapped IO.
	/// </summary>
	public class PpuMemory : Memory
	{
		readonly Console _console;
		readonly byte[] _vRam;
		readonly byte[] _paletteRam;

		/// <summary>
		/// Construct a new PPU memory device.
		/// </summary>
		/// <param name="console">the console that this PPU memory is a part of</param>
		public PpuMemory(Console console)
		{
			_console = console;
			_vRam = new byte[2048];
			_paletteRam = new byte[32];
		}

		/// <summary>
		/// Reset this PPU memory to its startup state.
		/// </summary>
		public void Reset()
		{
			Array.Clear(_vRam, 0, _vRam.Length);
			Array.Clear(_paletteRam, 0, _paletteRam.Length);
		}

		/// <summary>
		/// Given a palette RAM address ($3F00-$3FFF), return the index in
		/// palette RAM that it corresponds to.
		/// </summary>
		/// <returns>The palette ram index.</returns>
		/// <param name="address">Address.</param>
		public ushort GetPaletteRamIndex(ushort address)
		{
			var index = (ushort)((address - 0x3F00) % 32);

			// Mirror $3F10, $3F14, $3F18, $3F1C to $3F00, $3F14, $3F08 $3F0C
			return index >= 16 && ((index - 16) % 4 == 0) ? (ushort)(index - 16) : index;
		}

		/// <summary>
		/// Read a byte of memory from the specified address.
		/// </summary>
		/// <returns>the byte read</returns>
		/// <param name="address">the address to read from</param>
		public override byte Read(ushort address)
		{
			byte data;
			if (address < 0x2000)
			{
				data = _console.Mapper.Read(address);
			}
			else
			{
				if (address <= 0x3EFF)
				{
					data = _vRam[_console.Mapper.VramAddressToIndex(address)];
				}
				else
				{
					if (address >= 0x3F00 && address <= 0x3FFF)
					{
						data = _paletteRam[GetPaletteRamIndex(address)];
					}
					else
					{
						throw new Exception("Invalid PPU Memory Read at address: " + address.ToString("x4"));
					}
				}
			}
			return data;
		}

		/// <summary>
		/// Write a byte of memory to the specified address.
		/// </summary>
		/// <param name="address">the address to write to</param>
		/// <param name="data">the byte to write to the specified address</param>
		public override void Write(ushort address, byte data)
		{
			if (address < 0x2000)
			{
				_console.Mapper.Write(address, data);
			}
			else if (address >= 0x2000 && address <= 0x3EFF) // Internal VRAM
			{
				_vRam[_console.Mapper.VramAddressToIndex(address)] = data;
			}
			else if (address >= 0x3F00 && address <= 0x3FFF) // Palette RAM addresses
			{
				var addr = GetPaletteRamIndex(address);
				_paletteRam[addr] = data;
			}
			else // Invalid Write
			{
				throw new Exception("Invalid PPU Memory Write at address: " + address.ToString("x4"));
			}
		}
	}
}
