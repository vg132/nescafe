namespace Nescafe.Core.Mappers;

[Mapper(185)]
public class Mapper185 : Mapper
{
	private bool _copyProtection;
	private byte _copyProtectionData = 0x0f;

	public Mapper185(Console console)
	{
		_console = console;
		_vramMirroringType = _console.Cartridge.VerticalVramMirroring ? VramMirroring.Vertical : VramMirroring.Horizontal;
		_copyProtection = true;
	}

	public override byte Read(ushort address)
	{
		byte data;
		if (address < 0x2000)
		{
			if (_copyProtection)
			{
				data = _copyProtectionData;
			}
			else
			{
				data = _console.Cartridge.ReadChr(address);
			}
		}
		else if (address >= 0x6000 && address < 0x8000)
		{
			// Open Bus
			data = 0x00;
		}
		else if (address >= 0x8000 && address <= 0xFFFF)
		{
			data = _console.Cartridge.ReadPrgRom(address - 0x8000);
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
			if (_copyProtection)
			{
				if (data == 0x11 || _copyProtectionData == 0x3c)
				{
					_copyProtection = false;
				}
				else if (data == 0x33)
				{
					_copyProtectionData = 0x3c;
				}
				else
				{
					_copyProtectionData = 0x0f;
				}
			}
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
