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
Run the timer for default (4) cycles:
```bash
dev-pomo run-timer 3
```

Run the timer for custom cycles:
```bash
dev-pomo run-timer 3
```

Configure the pomodoro timer options:
```bash
dev-pomo configure
      _
   __| |   ___  __   __          _ __     ___    _ __ ___     ___
  / _` |  / _ \ \ \ / /  _____  | '_ \   / _ \  | '_ ` _ \   / _ \
 | (_| | |  __/  \ V /  |_____| | |_) | | (_) | | | | | | | | (_) |
  \__,_|  \___|   \_/           | .__/   \___/  |_| |_| |_|  \___/
                                |_|

Update the configuration for the pomodoro timers.
Enter the pomodoro duration (in minutes) (25): 25
Enter the break duration (in minutes) (5): 5
Show the terminal application's title? [y/n] (y): y
Play notification sound on timer competion? [y/n] (y): y
Auto-start new cycle when current one completes? [y/n] (n): n
Show compact/minimal timer output? [y/n] (n): n
```

## Feedback
If you use this tool and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository.
