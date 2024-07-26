namespace Nescafe.Core.Mappers;

/// <summary>
/// Not working, template implementation
/// </summary>
[Mapper(172)]
public class Mapper172 : Mapper
{
	private int _bank0Offset;

	private byte _accumulator = 0;
	private byte _staging;
	private bool _invert = false;
	private bool _increase = false;
	private bool _yFlag = false;

	private readonly byte _mask = 0x0F;

	public Mapper172(Console console)
	{
		throw new NotImplementedException();
		_console = console;

		_bank0Offset = 0;
		_vramMirroringType = _console.Cartridge.VerticalVramMirroring ? VramMirroring.Vertical : VramMirroring.Horizontal;

		_invert = true;
		_increase = false;	
		_yFlag = false;
	}

	public override byte Read(ushort address)
	{
		//uint8_t value = (_accumulator & _mask) | ((_inverter ^ (_invert ? 0xFF : 0)) & ~_mask);
		//_yFlag = !_invert || ((value & 0x10) != 0);
		byte data;
		if (address < 0x2000) // CHR ROM or RAM
		{
			data = _console.Cartridge.ReadChr(_bank0Offset + address);
		}
		else if (address == 0x4100)
		{
			data = _accumulator;
		}
		else if (address == 0x4101)
		{
			data = (byte)(_invert ? 8 : 0);
		}
		else if (address == 0x4102)
		{
			data = 0;
		}
		else if (address == 0x4103)
		{
			data = (byte)(_increase ? 8 : 0);
		}
		else if (address >= 0x8000) // PRG ROM stored at $8000 and above
		{
			data = _console.Cartridge.ReadPrgRom(address - 0x8000);
		}
		else
		{
			data = _accumulator;
		}
		return data;
	}

	public override void Write(ushort address, byte data)
	{
		if (address < 0x2000) // CHR ROM or RAM
		{
			_console.Cartridge.WriteChr(_bank0Offset + address, data);
		}
		else if (address == 0x4100)
		{
			if (_increase)
			{
				_accumulator++;
			}
			else
			{
				_accumulator = (byte)(((_accumulator & ~_mask) | (_staging & _mask)) ^ (_invert ? 0xFF : 0));
			}
		}
		else if (address == 0x4101)
		{
			_invert = (data & 0x01) != 0;
		}
		else if (address == 0x4102)
		{
			_staging = (byte)(data & _mask);
			_invert = (data & ~_mask) != 0;
		}
		else if (address == 0x4103)
		{
			_increase = (data & 0x01) != 0;
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
			throw new Exception("Invalid mapper write at address: 0x" + address.ToString("X4"));
		}
		_yFlag = !_invert || ((data & 0x10) != 0);
	}

	private void WriteBankSelect(byte data)
	{
		_bank0Offset = (data & 0x3) * 0x2000;
	}

	#region Load/Save state

	public override object SaveState()
	{
		return null;
	}

	public override void LoadState(object state)
	{
	}

	#endregion
}