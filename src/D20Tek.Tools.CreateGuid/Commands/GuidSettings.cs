using System.ComponentModel;
using D20Tek.Spectre.Console.Extensions.Settings;
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid.Commands;

internal sealed class GuidSettings : VerbositySettings
{
	[CommandOption("-c|--count <COUNT>")]
	[Description("The number of GUIDs to generate (defaults to 1).")]
	[DefaultValue(1)]
	public int Count { get; set; } = 1;

	[CommandOption("-f|--format <GUID-FORMAT>")]
	[Description("Defines how the GUIDs are formatted in string form (Default, Number, Braces, Parens, Hex).")]
	[DefaultValue(GuidFormat.Default)]
	public GuidFormat Format { get; set; }

    [CommandOption("-s|--uuid-v7")]
    [Description("Generates a UUID v7 compliant GUID for sortable unique identifiers.")]
    [DefaultValue(false)]
    public bool UsesUuidV7 { get; set; }
    
	[CommandOption("-e|--empty")]
	[Description("Defines if the GUIDs should be empty (using zero-values).")]
	[DefaultValue(false)]
	public bool UsesEmptyGuid { get; set; }

	[CommandOption("-u|--upper")]
	[Description("Defines if the generated GUIDs should be upper-cased (defaults to lower-cased).")]
	[DefaultValue(false)]
	public bool UsesUpperCase { get; set; }

	[CommandOption("-p|--clipboard-copy")]
	[Description("Defines whether the output of this command should be copied to the system clipboard.")]
	[DefaultValue(false)]
	public bool CopyToClipboard { get; set; }

	[CommandOption("-o|--output")]
	[Description("Filename for output file used to save generated guids.")]
	[DefaultValue("")]
	public string OutputFile { get; set; } = string.Empty;
}
