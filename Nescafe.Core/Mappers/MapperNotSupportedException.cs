namespace Nescafe.Core.Mappers;

public class MapperNotSupportedException : Exception
{
	private readonly int _mapper;

	public MapperNotSupportedException(int mapper)
		: base($"Mapper {mapper} not supported.")
	{
		_mapper = mapper;
	}

	public int Mapper => _mapper;
}
