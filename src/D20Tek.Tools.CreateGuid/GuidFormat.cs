//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------

namespace D20Tek.Tools.CreateGuid
{
    public enum GuidFormat
    {
        Default = 0,
        Number,
        Braces,
        Parens,
        Hex
    }

    internal static class GuidFormatExtensions
    {
        internal static string ToFormatString(this GuidFormat format)
        {
            return format switch
            {
                GuidFormat.Number => "N",
                GuidFormat.Braces => "B",
                GuidFormat.Parens => "P",
                GuidFormat.Hex => "X",
                _ => "D",
            };
        }
    }
}
