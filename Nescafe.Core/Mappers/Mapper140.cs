namespace Nescafe.Core.Mappers;

[Mapper(140)]
public class Mapper140 : Mapper
{
	private int _chrBankOffset;
	private int _prgBankOffset;

	public Mapper140(Console console)
	{
		_console = console;
		//_vramMirroringType = VramMirroring.SingleLower;
	}

	public override byte Read(ushort address)
	{
		byte data;
		if (address < 0x2000)
		{
			data = _console.Cartridge.ReadChr(_chrBankOffset + address);
		}
		else if (address >= 0x6000 && address <= 0x7FFF)
		{
			data = _console.Cartridge.ReadPrgRom(_prgBankOffset + (address - 0x6000));
		}
		else
		{
			data = 0x00;
			//throw new Exception("Invalid mapper read at address: " + address.ToString("X4"));
		}
		return data;
	}

	public override void Write(ushort address, byte data)
	{
		if (address >= 0x6000 && address <= 0x7FFF)
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
