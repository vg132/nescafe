namespace Nescafe.Core.Mappers;

[Mapper(7)]
public class AxROM : Mapper
{
	private int _bank0Offset;

	public AxROM(Console console)
	{
		_console = console;

		_bank0Offset = 0;

		_vramMirroringType = VramMirroring.SingleLower;
	}

	public override byte Read(ushort address)
	{
		byte data;
		if (address < 0x2000) // CHR ROM or RAM
		{
			data = _console.Cartridge.ReadChr(address);
		}
		else if (address >= 0x6000 && address < 0x8000)
		{
			// Open Bus
			data = 0x00;
		}
		else if (address <= 0xFFFF)
		{
			data = _console.Cartridge.ReadPrgRom(_bank0Offset + (address - 0x8000));
		}
		else
		{
			throw new Exception("Invalid mapper read at address: " + address.ToString("X4"));
		}
		return data;
	}

	public override void Write(ushort address, byte data)
	{
		if (address < 0x2000) // CHR ROM or RAM
		{
			_console.Cartridge.WriteChr(address, data);
		}
		else if (address >= 0x6000 && address < 0x8000)
		{
			// Open Bus
		}
		else if (address >= 0x8000)
		{
			_bank0Offset = (data & 0x7) * 0x8000;
			_vramMirroringType = ((data >> 4) & 0x1) == 0 ? VramMirroring.SingleLower : VramMirroring.SingleUpper;
		}
	}

	public override void LoadState(object state)
	{
	}

	public override object SaveState()
	{
		return null;
	}
}
