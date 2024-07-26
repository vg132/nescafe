namespace Nescafe.Core.Mappers;

public class MapperAttribute : Attribute
{
	private readonly int _mapperId;

	public MapperAttribute(int mapperId)
	{
		_mapperId = mapperId;
	}

	public int Id => _mapperId;
}
