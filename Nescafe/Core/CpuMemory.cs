using System;

namespace Nescafe.Core
{
	/// <summary>
	/// Represents the CPU's memory and memory mapped IO.
	/// </summary>
	public class CpuMemory : Memory
	{
		// First 2KB of internal ram
		byte[] _internalRam = new byte[2048];
		readonly Console _console;

		/// <summary>
		/// Construct a new CPU memory device.
		/// </summary>
		/// <param name="console">the console that this CPU Memory is a part of</param>
		public CpuMemory(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Resets this CPU Memory to its startup state
		/// </summary>
		public void Reset()
		{
			Array.Clear(_internalRam, 0, _internalRam.Length);
		}

		// Return the index in internalRam of the address (handle mirroring)
		private ushort HandleInternalRamMirror(ushort address)
		{
			return (ushort)(address % 0x800);
		}

		// Handles mirroring of PPU register addresses
		private ushort GetPpuRegisterFromAddress(ushort address)
		{
			// Special case for OAMDMA ($4014) which is not alongside the other registers
			return address == 0x4014 ? address : (ushort)(0x2000 + ((address - 0x2000) % 8));
		}

		private void WritePpuRegister(ushort address, byte data)
		{
			_console.Ppu.WriteToRegister(GetPpuRegisterFromAddress(address), data);
		}

		private byte ReadPpuRegister(ushort address)
		{
			return _console.Ppu.ReadFromRegister(GetPpuRegisterFromAddress(address));
		}

		private byte ReadApuIoRegister(ushort address)
		{
			byte data;
			switch (address)
			{
				case 0x4016: // Controller 1
					data = _console.Controller.ReadControllerOutput();
					break;
				default: // Unimplemented register
					data = 0;
					break;
			}
			return data;
		}

		void WriteApuIoRegister(ushort address, byte data)
		{
			switch (address)
			{
				case 0x4016: // Controller 1
					_console.Controller.WriteControllerInput(data);
					break;
				default: // Unimplemented register
					data = 0;
					break;
			}
		}

		/// <summary>
		/// Read a byte of memory from the specified address.
		/// </summary>
		/// <returns>the byte read</returns>
		/// <param name="address">the address to read from</param>
		public override byte Read(ushort address)
		{
			byte data;
			if (address < 0x2000) // Internal CPU RAM 
			{
				var addressIndex = HandleInternalRamMirror(address);
				data = _internalRam[addressIndex];
			}
			else
			{
				if (address <= 0x3FFF)
				{
					data = ReadPpuRegister(address);
				}
				else
				{
					if (address <= 0x4017)
					{
						data = ReadApuIoRegister(address);
					}
					else
					{
						if (address <= 0x401F)
						{
							data = (byte)0;
						}
						else
						{
							if (address >= 0x4020)
							{
								data = _console.Mapper.Read(address);
							}
							else
							{
								throw new Exception("Invalid CPU read at address " + address.ToString("X4"));
							}
						}
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
			if (address < 0x2000) // Internal CPU RAM
			{
				var addressIndex = HandleInternalRamMirror(address);
				_internalRam[addressIndex] = data;
			}
			else if (address <= 0x3FFF || address == 0x4014) // PPU Registers
			{
				WritePpuRegister(address, data);
			}
			else if (address <= 0x4017) // APU / IO 
			{
				WriteApuIoRegister(address, data);
			}
			else if (address <= 0x401F) // Disabled on a retail NES
			{

			}
			else if (address >= 0x4020)
			{
				_console.Mapper.Write(address, data);
			}
			else // Invalid Write
			{
				throw new Exception("Invalid CPU write to address " + address.ToString("X4"));
			}
		}

		#region Save/Load state

		[Serializable]
		public class CpuMemoryState
		{
			public byte[] InternalRam;
		}

		public override object SaveState()
		{
			return new CpuMemoryState
			{
				InternalRam = _internalRam
			};
		}

		public override void LoadState(object state)
		{
			_internalRam = ((CpuMemoryState)state).InternalRam;
		}

		#endregion
	}
}
