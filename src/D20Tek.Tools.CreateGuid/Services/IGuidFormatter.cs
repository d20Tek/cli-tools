//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------

namespace D20Tek.Tools.CreateGuid.Services
{
    public interface IGuidFormatter
    {
        public string Format(Guid guid, GuidFormat format, bool toUpper);
    }
}
