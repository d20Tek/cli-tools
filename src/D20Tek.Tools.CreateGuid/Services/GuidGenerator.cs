namespace D20Tek.Tools.CreateGuid.Services;

internal class GuidGenerator : IGuidGenerator
{
	public IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid)
	{
		for (int i = 0; i < guidCount; i++)
		{
			yield return useEmptyGuid ? Guid.Empty : Guid.NewGuid();
		}
	}
}
