using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal sealed class FakeGuidGenerator : IGuidGenerator
{
    private readonly Guid[] _guids;

    public FakeGuidGenerator(Guid[] guids) => _guids = guids;

    public IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid, bool isUuidV7)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(guidCount, _guids.Length);
        return _guids.Take(guidCount);
    }
}
