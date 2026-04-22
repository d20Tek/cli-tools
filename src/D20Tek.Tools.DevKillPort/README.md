# dev-killport
A cross-platform command-line utility to identify and kill processes bound to a specific port. Supports Windows, macOS, and Linux with OS-native process discovery.

Run dev-killport in your favorite console window.

## Features
- Find processes bound to any TCP or UDP port
- Kill processes by port number with optional confirmation prompt
- View port usage without killing anything
- List all active port bindings across the system in one command
- Filter by protocol (TCP, UDP, or both)
- Dry-run mode to preview what would be killed without taking action
- Watch mode to poll until a port is freed
- JSON output for scripting and automation
- Interactive REPL mode for multi-command sessions
- Works on Windows (PowerShell), macOS, and Linux (`ss` / `lsof` / `/proc`)

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global dev-killport
```

To update the tool:

```bash
dotnet tool update --global dev-killport
```

## Usage
```
USAGE:
    dev-killport [COMMAND] [OPTIONS]

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    kill <PORT>    Find and kill processes bound to a port    (alias: k)
    view <PORT>    View processes bound to a port             (alias: v)
    list           List all ports with mapped process details (alias: ls)
```

Running `dev-killport` with no arguments starts **interactive mode**, where you can run multiple commands in a single session. Type `exit` to quit or `--help` to list available commands:

```
killport> kill 5000 --dry-run
killport> view 8080 --json
killport> list --protocol tcp
killport> exit
```

### kill Command Options
```
USAGE:
    dev-killport kill <PORT> [OPTIONS]

ARGUMENTS:
    <PORT>    The port number to find and kill processes for

OPTIONS:
    -h, --help                  Prints help information
    -f, --force                 Kill without confirmation prompt
        --all                   Kill all matching processes (default: kill only first)
    -p, --protocol <PROTOCOL>   Protocol filter: tcp, udp, or both (default: both)
    -j, --json                  Output results as JSON before killing
    -d, --dry-run               Show what would happen without killing
    -w, --watch                 Wait and poll until the port is free after kill
    -t, --timeout <SECONDS>     Max wait time in seconds for watch mode (default: 30)
```

Without `--force`, the tool prompts for confirmation before killing:

```
Port 5000 is used by:
 PID   Name    Protocol  State
 1234  dotnet  TCP       Listen

Kill this process? [y/n] (n): y
Successfully killed process dotnet (PID: 1234).
```

### view Command Options
```
USAGE:
    dev-killport view <PORT> [OPTIONS]

ARGUMENTS:
    <PORT>    The port number to find processes for

OPTIONS:
    -h, --help                  Prints help information
    -p, --protocol <PROTOCOL>   Protocol filter: tcp, udp, or both (default: both)
    -j, --json                  Output results as JSON
```

### list Command Options
```
USAGE:
    dev-killport list [OPTIONS]

OPTIONS:
    -h, --help                  Prints help information
    -p, --protocol <PROTOCOL>   Protocol filter: tcp, udp, or both (default: both)
    -j, --json                  Output results as JSON
```

### Examples

Kill the process on port 5000 (prompts for confirmation):
```bash
dev-killport kill 5000
```

Kill without confirmation:
```bash
dev-killport kill 5000 --force
```

Kill all matching processes on a port:
```bash
dev-killport kill 5000 --all --force
```

Preview what would be killed without actually killing it:
```bash
dev-killport kill 5000 --dry-run
```

Kill and then wait until the port is free (up to 60 seconds):
```bash
dev-killport kill 5000 --watch --timeout 60
```

View processes on a port without killing:
```bash
dev-killport view 5000
```

View only TCP processes on a port:
```bash
dev-killport view 5000 --protocol tcp
```

View processes as JSON:
```bash
dev-killport view 5000 --json
```

List all active port bindings on the system:
```bash
dev-killport list
```

List only UDP port bindings:
```bash
dev-killport list --protocol udp
```

List all port bindings as JSON:
```bash
dev-killport list --json
```

## Output Format

The table output shows the port, PID, process name, protocol, and connection state:

```
Port 5000 is used by:

 PID   Name    Protocol  State
 1234  dotnet  TCP       Listen
```

The `list` command includes a Port column:

```
All ports with active processes:

 Port   PID    Name     Protocol  State
 5000   1234   dotnet   TCP       Listen
 8080   5678   node     TCP       Established
 27017  9012   mongod   TCP       Listen
```

With `--json`, the output is machine-readable:

```json
{
  "port": 5000,
  "processes": [
    {
      "pid": 1234,
      "name": "dotnet",
      "protocol": "TCP",
      "state": "Listen"
    }
  ]
}
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
