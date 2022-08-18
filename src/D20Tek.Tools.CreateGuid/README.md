# create-guid cli tool
This tool creates a single or multiple random GUIDs, which can be formatted in different ways.

## Installation
First, you must have the .NET 6.0 or later runtime on your machine. If you're using Visual Studio or other developer tools, it's probably already on your machine.

Then, install the create-guid tool, using the .NET command-line tool:
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

Sample output:
    create-guid: running to generate your GUIDs:
    {4c29d028-e8ff-4e69-9cd2-f8ea142d5656}
    Command completed successfully!
```

To get 5 GUIDs:
```
create-guid --count 5
create-guid -c 5

Sample output:
    create-guid: running to generate your GUIDs:
    34f8aeaa-c20a-4649-9325-a9f156ba8b50
    75a8fd64-8302-4d12-8a46-c9ad04a665fb
    ac313e57-bb50-442d-8424-dfd8a0a4607c
    43943fc9-bda4-493c-8578-b6aa92a17a92
    dcfd7689-378f-4ccb-841f-604fd9f1782a
    Command completed successfully!
```
