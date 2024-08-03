namespace Nescafe.Core.Mappers;

[Mapper(11)]
public class ColorDreamMapper : Mapper
{
	private int _chrBankOffset;
	private int _prgBankOffset;

	public ColorDreamMapper(Console console)
	{
		_console = console;
		_vramMirroringType = _console.Cartridge.VerticalVramMirroring ? VramMirroring.Vertical : VramMirroring.Horizontal;
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
			//data = 0x00;
			throw new Exception("Invalid mapper read at address: " + address.ToString("X4"));
		}
		return data;
	}

	public override void Write(ushort address, byte data)
	{
		if (address >= 0x8000 && address <= 0xFFFF)
		{
			var currentValue = Read(address);

			var oldData = data;
			data = (byte)((data & currentValue) | (currentValue & 1));

			var chrBank = ((data & 0xF0) >> 4);
			if (chrBank > _console.Cartridge.ChrBanks)
			{
				chrBank = (chrBank % 2) == 0 ? 0 : 1;
			}
			_chrBankOffset = chrBank * 0x2000;
			_prgBankOffset = (data & 0x3) * 0x8000;
		}
	}

	#region Load/Save state

	public override void LoadState(object state)
	{

	}

	public override object SaveState()
	{
		return null;
	}

	#endregion
}
