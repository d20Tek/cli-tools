# dev-pomo
A terminal app that runs a pomodoro timer to allow you to focus on your development for 25 minute windows. Pomodoro is a popular system for focusing your work in small chunks of time where you ignore all other distractions. This terminal app runs a timer for you and notifies you when that time is done.

Run dev-pomo in your favorite console window.

## Features
- Run a pomodoro timer for a specified time period.
- Define the number of pomodoro cycles you wish to run in a session.
- Supports configuring the timer:
  - the pomodoro duration time.
  - the break duration time.
  - show/hide app title banner.
  - enable/disable completion notification sounds.
  - enable/disable auto-starting the next cycle.
  - support for minimal output display.

## Installation
Install globally using the .NET CLI:

```bash
dotnet tool install --global dev-pomo
```

To update the tool:

```bash
dotnet tool update --global dev-pomo
```

## Usage
```bash
USAGE:
    dev-pomo [OPTIONS] [COMMAND]

OPTIONS:
                                      DEFAULT
    -h, --help                                   Prints help information
    -c, --cycles <POMODORO-CYCLES>    4          Defines how many iterations of pomodoros to run in a session (defaults
                                                 to 4)

COMMANDS:
    run-timer    Default command that runs the pomodoro timer
    configure    Configure various properties of the pomodoro timer
```

### Examples
Run the timer for custom cycles:
```bash
dev-pomo run-timer 3
```

Configure the pomodoro timer options:
```bash
dev-pomo configure
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
