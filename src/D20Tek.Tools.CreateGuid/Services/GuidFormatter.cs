using D20Tek.Tools.CreateGuid.Contracts;

namespace D20Tek.Tools.CreateGuid.Services;

internal sealed class GuidFormatter : IGuidFormatter
{
	public string Format(Guid guid, GuidFormat format, bool toUpper)
	{
		string text = guid.ToString(format.ToFormatString());
		return toUpper ? text.ToUpper() : text;
	}
}
