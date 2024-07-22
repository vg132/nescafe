namespace Nescafe.Core.Mappers;

public class Mapper172 : Mapper
{
	private readonly Console _console;

	public Mapper172(Console console)
	{
		_console = console;
	}

	public override byte Read(ushort address)
	{
		return 0;
	}

	public override void Write(ushort address, byte data)
	{

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