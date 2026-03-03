# dev-log
A command-line utility to help developers create and edit weekly developer work logs, persisting them as structured markdown files. Tracks project accomplishments and delivery status week by week.

Run dev-log in your favorite console window.

## Features
- Create weekly dev-logs organized by project name
- Add accomplishments interactively from the command line
- View and edit existing log entries
- Browse all available weekly log files
- Logs are saved as readable markdown files following a consistent weekly template
- Supports an interactive REPL mode for multi-command sessions

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global dev-log
```

To update the tool:

```bash
dotnet tool update --global dev-log
```

## Usage
```
USAGE:
    dev-log [COMMAND] [OPTIONS]

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    add <PROJECT>    Add a project entry to this week's dev-log     (alias: a)
    edit <PROJECT>   Edit an existing project entry in the dev-log  (alias: e)
    view             View the full dev-log for a given week         (alias: v)
    list             List all available dev-log files               (alias: ls)
```

Running `dev-log` with no arguments starts **interactive mode**, where you can run multiple commands in a single session. Type `exit` to quit or `--help` to list available commands:

```
log> add MyProject -f ./devlogs
log> view -f ./devlogs
log> list -f ./devlogs
log> exit
```

### add Command Options
```
USAGE:
    dev-log add <PROJECT> [OPTIONS]

ARGUMENTS:
    <PROJECT>    The name of the project to add the dev-log entry for

OPTIONS:
    -h, --help              Prints help information
    -d, --date <DATE>       The date for the dev-log entry (defaults to today). Format: MM-dd-yyyy
    -f, --folder <FOLDER>   The folder path where dev-log files are stored (defaults to current directory)
```

After running `add`, the tool prompts for accomplishments interactively — enter one per line and press Enter on an empty line to finish:

```
Enter accomplishments (one per line, empty line to finish):
  > Implemented the OAuth integration with full test coverage
  > Fixed the cache invalidation bug blocking the release
  > 
```

### edit Command Options
```
USAGE:
    dev-log edit <PROJECT> [OPTIONS]

ARGUMENTS:
    <PROJECT>    The name of the project entry to edit

OPTIONS:
    -h, --help              Prints help information
    -d, --date <DATE>       The date for the dev-log entry to edit (defaults to today). Format: MM-dd-yyyy
    -f, --folder <FOLDER>   The folder path where dev-log files are stored (defaults to current directory)
```

The `edit` command displays the current numbered accomplishments and prompts for a line number to edit. Press Enter on an empty line to finish editing:

```
Current accomplishments:
 [1] Implemented the OAuth integration with full test coverage
 [2] Fixed the cache invalidation bug blocking the release

Enter line number to edit (empty to finish):
  > 2
  New text: Resolved the cache invalidation bug, unblocking the v2.1 release
Enter line number to edit (empty to finish):
  > 
```

### view Command Options
```
USAGE:
    dev-log view [OPTIONS]

OPTIONS:
    -h, --help              Prints help information
    -d, --date <DATE>       The date within the week to view (defaults to current week). Format: MM-dd-yyyy
    -f, --folder <FOLDER>   The folder path where dev-log files are stored (defaults to current directory)
```

### list Command Options
```
USAGE:
    dev-log list [OPTIONS]

OPTIONS:
    -h, --help              Prints help information
    -f, --folder <FOLDER>   The folder path where dev-log files are stored (defaults to current directory)
```

### Examples
Add a project entry to this week's dev-log (prompts for accomplishments interactively):
```bash
dev-log add MyProject
```

Add an entry to a specific folder for a prior week:
```bash
dev-log add MyProject -f ./devlogs -d 01-08-2025
```

View this week's dev-log:
```bash
dev-log view
```

View a specific week's log stored in a custom folder:
```bash
dev-log view -d 01-08-2025 -f ./devlogs
```

Edit an existing project entry in this week's log:
```bash
dev-log edit MyProject
```

Edit a prior week's entry:
```bash
dev-log edit MyProject -d 01-08-2025 -f ./devlogs
```

List all available dev-log files in the current directory:
```bash
dev-log list
```

List logs stored in a specific folder:
```bash
dev-log list -f ./devlogs
```

## Log Format
Each weekly log is saved as a markdown file named `dev-log-YYYYMMDD.md`, where the date is the Sunday that starts the week. Files are stored in the specified folder (defaults to the current directory) and follow this template:

```markdown
## Week of January 5, 2025

### MyProject
- Implemented the OAuth integration with full test coverage
- Resolved the cache invalidation bug, unblocking the v2.1 release
- Refactored the service layer to improve readability and reduce duplication

### AnotherProject
- Reviewed and merged six pull requests
- Upgraded dependencies to the latest stable versions
- Wrote design doc for the upcoming data migration feature
```

The `list` command displays all available logs with their week dates:

```
 dev-log-20250112.md  (Week of January 12, 2025)
 dev-log-20250105.md  (Week of January 5, 2025)
 dev-log-20241229.md  (Week of December 29, 2024)
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
