namespace D20Tek.Tools.CreateGuid.Contracts;

public interface IGuidGenerator
{
	IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid, bool isUuidV7);
}
