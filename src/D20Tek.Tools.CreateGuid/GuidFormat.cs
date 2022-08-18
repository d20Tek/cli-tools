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
        Hex,
        D,
        N,
        B,
        P,
        X
    }

    internal static class GuidFormatExtensions
    {
        internal static string ToFormatString(this GuidFormat format)
        {
            return format switch
            {
                GuidFormat.Number or GuidFormat.N => "N",
                GuidFormat.Braces or GuidFormat.B => "B",
                GuidFormat.Parens or GuidFormat.P => "P",
                GuidFormat.Hex or GuidFormat.X => "X",
                _ => "D",
            };
        }
    }
}
