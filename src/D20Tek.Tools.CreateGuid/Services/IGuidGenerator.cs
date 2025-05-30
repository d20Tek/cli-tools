namespace D20Tek.Tools.CreateGuid.Services;

public interface IGuidGenerator
{
	IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid);
}
