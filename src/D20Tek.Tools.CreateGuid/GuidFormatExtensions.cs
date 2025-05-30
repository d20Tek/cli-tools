namespace D20Tek.Tools.CreateGuid;

internal static class GuidFormatExtensions
{
    internal static string ToFormatString(this GuidFormat format) =>
        format switch
        {
            GuidFormat.Number or GuidFormat.N => "N",
            GuidFormat.Braces or GuidFormat.B => "B",
            GuidFormat.Parens or GuidFormat.P => "P",
            GuidFormat.Hex or GuidFormat.X => "X",
            _ => "D",
        };
}
