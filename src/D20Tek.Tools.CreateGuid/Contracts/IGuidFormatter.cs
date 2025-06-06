namespace D20Tek.Tools.CreateGuid.Contracts;

public interface IGuidFormatter
{
	string Format(Guid guid, GuidFormat format, bool toUpper);
}
