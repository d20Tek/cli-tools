# create-guid
A command-line utility to help developers create GUIDs and UUIDs for their projects in various output formats, copy to clipboard, and save to output file.

Run create-guid in your favorite console window.

## Features
- Generate one or more GUIDs from the command line
- Supports:
  - **Standard GUIDs** (UUIDv4)
  - **UUIDv7** (time-ordered, future-proof)
  - **Empty GUIDs** for testing or placeholder usage
- Output in plain text (one per line)

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global create-guid
```

To update the tool:

```bash
dotnet tool update --global create-guid
```

## Usage
```bash
USAGE:
    create-guid [OPTIONS] [COMMAND]

OPTIONS:
                                         DEFAULT
    -h, --help                                      Prints help information
    -v, --verbosity <VERBOSITY-LEVEL>    Normal     The verbosity level for this operation: q(uiet), m(inimal),
                                                    n(ormal), d(etailed), and diag(nostic)
    -c, --count <COUNT>                  1          The number of GUIDs to generate (defaults to 1)
    -f, --format <GUID-FORMAT>           Default    Defines how the GUIDs are formatted in string form (Default, Number,
                                                    Braces, Parens, Hex)
    -s, --uuid-v7                                   Generates a UUID v7 compliant GUID for sortable unique identifiers
    -e, --empty                                     Defines if the GUIDs should be empty (using zero-values)
    -u, --upper                                     Defines if the generated GUIDs should be upper-cased (defaults to
                                                    lower-cased)
    -p, --clipboard-copy                            Defines whether the output of this command should be copied to the
                                                    system clipboard
    -o, --output                                    Filename for output file used to save generated guids

COMMANDS:
    generate    Default command that generates GUIDs in the appropriate format
```

### Examples
Generate a single standard GUID:
```bash
create-guid
```

Generate 5 UUIDv7 GUIDs:
```bash
create-guid --count 5 --uuidv7
```

Generate 3 GUIDs and copy them to the clipboard:
```bash
create-guid -c 3 --clipboard-copy
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
