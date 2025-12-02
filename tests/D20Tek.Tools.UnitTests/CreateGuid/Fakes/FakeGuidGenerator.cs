using D20Tek.Tools.CreateGuid.Contracts;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal sealed class FakeGuidGenerator(Guid[] guids) : IGuidGenerator
{
    private readonly Guid[] _guids = guids;

    public IEnumerable<Guid> GenerateGuids(int guidCount, bool useEmptyGuid, bool isUuidV7)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(guidCount, _guids.Length);
        return _guids.Take(guidCount);
    }
}
