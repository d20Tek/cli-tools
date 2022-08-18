# create-guid cli tool
This tool creates a single or multiple random GUIDs, which can be formatted in different ways.

## Installation
First, you must have the .NET 6.0 or later runtime on your machine. If you're using Visual Studio or other developer tools, it's probably already on your machine.

The install the create-guid tool, using the command-line:
```
	dotnet tool install D20Tek.Tools.CreateGuidguid
	dotnet tool install -g D20Tek.Tools.CreateGuidguid
```

The first command installs the tool into your current project directory, and can only be used in that project.

The second command installs it as a global tool that can be accessed from any command-line or project.

Depending on the tool and your needs, you will need to decide which to use. For GUID creation, this seems like a good candidate for a global tool that we would use in many projects.

## Usage
Here is an example of the help documentation for the tool:
```
USAGE:
    create-guid [OPTIONS]

OPTIONS:
    -h, --help                    Prints help information
    -c, --count <COUNT>           The number of GUIDs to generate (defaults to 1)
    -f, --format <GUID-FORMAT>    Defines how the GUIDs are formatted in string form
                                  (Allowed values: Default, Number, Braces, Parens, Hex)
    -e, --empty                   Defines if the GUIDs should be empty (using zero-values)
    -u, --upper                   Defines if the generated GUIDs should be printed in upper-case
                                  (defaults to lower-case)
```

### Examples
To get a single GUID:
```
    create-guid --format Braces
    create-guid -f B
```

To get 5 GUIDs:
```
    create-guid --count 5
    create-guid -c 5
```
