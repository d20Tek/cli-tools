using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal class FakeGuidGenerator : IGuidGenerator
{
    private readonly Guid[] _guids;

    public FakeGuidGenerator(Guid guid)
        : this([guid]) { }

    public FakeGuidGenerator(Guid[] guids) => _guids = guids;

    public IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid, bool isUuidV7)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(guidCount, _guids.Length);
        return _guids.Take(guidCount);
    }
}
