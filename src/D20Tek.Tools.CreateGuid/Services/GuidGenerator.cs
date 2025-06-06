using D20Tek.Tools.CreateGuid.Contracts;

namespace D20Tek.Tools.CreateGuid.Services;

internal sealed class GuidGenerator : IGuidGenerator
{
	public IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid, bool isUuidV7)
	{
		for (int i = 0; i < guidCount; i++)
		{
			yield return GenerateGuid(useEmptyGuid, isUuidV7);
		}
	}

	private static Guid GenerateGuid(bool useEmptyGuid, bool isUuidV7) =>
		(useEmptyGuid, isUuidV7) switch
		{
			(true, _) => Guid.Empty,
			(false, true) => Guid.CreateVersion7(),
			_ => Guid.NewGuid()
		};
}
