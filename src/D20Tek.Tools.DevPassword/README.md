# dev-password
A command-line utility to help developers create fully randomized, strong passwords for their projects and services. 
Makes it easy to copy and save these passwords for your user.

Run dev-password in your favorite terminal window.

## Features
- Generate one or more random, strong passwords from the command line
- Supports:
  - **Multiple character sets:** lower and upper case letters, numbers, and symbols.
  - **Variable command line options:** number of passwords and password lenth
  - **Configuration:** saving your configuration for creating passwords
- Output in plain text with entropy value and strength designation.

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global dev-password
```

To update the tool:

```bash
dotnet tool update --global dev-password
```

## Usage
```bash
USAGE:
    dev-password [OPTIONS] [COMMAND]

OPTIONS:
                                         DEFAULT
    -h, --help                                      Prints help information
    -v, --verbosity <VERBOSITY-LEVEL>    Normal     The verbosity level for this operation: q(uiet), m(inimal),
                                                    n(ormal), d(etailed), and diag(nostic)
    -c, --count <COUNT>                  1          The number of passwords to generate (defaults to 1)
    -l, --length <PASSWORD-LENGTH>       25         The number of characters to use in the generated password (defaults
                                                    to 25)

COMMANDS:
    generate     Default command that a generates password based on defined settings
    configure    Configure various properties of the dev passwords
```

### Examples
Generate a single password (with default length - 25):
```bash
dev-password generate
```

Generate 5 passwords with length of 15 characters:
```bash
dev-password generate --count 5 --length 15
```

Configure dev-password to include only the character sets you desire:
```bash
dev-password config
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
