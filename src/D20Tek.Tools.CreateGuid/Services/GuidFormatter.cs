//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------

namespace D20Tek.Tools.CreateGuid.Services
{
    internal class GuidFormatter : IGuidFormatter
    {
        public string Format(Guid guid, GuidFormat format, bool toUpper)
        {
            var result = guid.ToString(format.ToFormatString());
            if (toUpper) result = result.ToUpper();

            return result;
        }
    }
}
