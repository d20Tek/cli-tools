//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Settings;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace D20Tek.Tools.CreateGuid.Commands
{
    internal class GuidSettings : VerbositySettings
    {
        [CommandOption("-c|--count <COUNT>")]
        [Description("The number of GUIDs to generate (defaults to 1).")]
        [DefaultValue(1)]
        public int Count { get; set; } = 1;

        [CommandOption("-f|--format <GUID-FORMAT>")]
        [Description("Defines how the GUIDs are formatted in string form (Default, Number, Braces, Parens, Hex).")]
        [DefaultValue(GuidFormat.Default)]
        public GuidFormat Format { get; set; } = GuidFormat.Default;

        [CommandOption("-e|--empty")]
        [Description("Defines if the GUIDs should be empty (using zero-values).")]
        [DefaultValue(false)]
        public bool UsesEmptyGuid { get; set; } = false;

        [CommandOption("-u|--upper")]
        [Description("Defines if the generated GUIDs should be upper-cased (defaults to lower-cased).")]
        [DefaultValue(false)]
        public bool UsesUpperCase { get; set; } = false;
    }
}
