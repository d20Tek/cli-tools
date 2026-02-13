# json-minify
A command-line utility to help developers minify JSON files by removing whitespace and formatting, reducing file size (typically 30%) for production use.

Run json-minify in your favorite console window.

## Features
- Minify a single JSON file or all JSON files in a folder
- Outputs minified files with `.min.json` extension
- Specify a custom target folder for output files
- Validates JSON syntax during minification

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global json-minify
```

To update the tool:

```bash
dotnet tool update --global json-minify 
```

## Usage
```bash
USAGE:
    json-minify <FILEPATH> [OPTIONS] [COMMAND]

ARGUMENTS:
    <FILEPATH>    The fully qualified file path for the target json file to minify

OPTIONS:
    -h, --help             Prints help information
    -t, --target-folder    The target folder where the minified file will be written. Defaults to the current folder

COMMANDS:
    file <FILEPATH>        Default command that minifies the specified json file
    folder <FOLDERPATH>    Command that minifies all of the json files in the specified folder
```

### File Command Options
```bash
USAGE:
    json-minify file <FILEPATH> [OPTIONS]

ARGUMENTS:
    <FILEPATH>    The fully qualified file path for the target json file to minify

OPTIONS:
    -h, --help                       Prints help information
    -t, --target-folder <FOLDER>     The target folder where the minified file will be written.
                                     Defaults to the current folder.
```

### Folder Command Options
```bash
USAGE:
    json-minify folder <FOLDERPATH> [OPTIONS]

ARGUMENTS:
    <FOLDERPATH>    The fully qualified target directory/folder path to search for json files to minify

OPTIONS:
    -h, --help                       Prints help information
    -t, --target-folder <FOLDER>     The target folder where the minified files will be written.
                                     Defaults to the current folder.
```

### Examples
Minify a single JSON file (file is the default command used when no command is specified):
```bash
json-minify .\config.json
```

Minify a JSON file and output to a specific folder:
```bash
json-minify file .\config.json --target-folder .\dist
```

Minify all JSON files in a folder:
```bash
json-minify folder .\data
```

Minify all JSON files in a folder and output to a build directory:
```bash
json-minify folder .\data -t .\build\output
```

## Output
- Input: `config.json` → Output: `config.min.json`
- Input: `data.json` → Output: `data.min.json`

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
