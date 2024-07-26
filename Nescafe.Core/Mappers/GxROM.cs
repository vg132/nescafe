namespace Nescafe.Core.Mappers;

[Mapper(66)]
public class GxROM : Mapper
{
	private int _chrBankOffset;
	private int _prgBankOffset;

	public GxROM(Console console)
	{
		_console = console;
	}

	public override byte Read(ushort address)
	{
		byte data;
		if (address < 0x2000)
		{
			data = _console.Cartridge.ReadChr(_chrBankOffset + address);
		}
		else if (address >= 0x6000 && address < 0x8000)
		{
			// Open Bus
			data = 0x00;
		}
		else if (address >= 0x8000 && address <= 0xFFFF)
		{
			data = _console.Cartridge.ReadPrgRom(_prgBankOffset + (address - 0x8000));
		}
		else
		{
			throw new Exception("Invalid mapper read at address: " + address.ToString("X4"));
		}
		return data;
	}

	public override void Write(ushort address, byte data)
	{
		if (address >= 0x8000 && address <= 0xFFFF)
		{
			_chrBankOffset = (byte)(data & 0x03) * 0x2000;
			_prgBankOffset = (byte)((data & 0x30) >> 4) * 0x8000;
		}
	}

	public override object SaveState()
	{
		return null;
	}

	public override void LoadState(object state)
	{
	}
}
