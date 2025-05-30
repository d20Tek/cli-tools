namespace D20Tek.Tools.CreateGuid.Services;

public interface IGuidFormatter
{
	string Format(Guid guid, GuidFormat format, bool toUpper);
}
