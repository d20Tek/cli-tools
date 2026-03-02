# DevLog CLI Tool - Implementation Plan

## Overview
DevLog is an interactive CLI tool that allows developers to create and edit dev-log files
persisted as markdown. Each file covers one week (Sunday as week start) and follows a
structured template with project-level accomplishments.

### Markdown Template
```markdown
## Week of [Date]

### [Project Name]
- Specific accomplishment with technical detail
- Challenge overcome with impact/result
- Feature implemented with context
- Learning or improvement made

### [Another Project if applicable]
- Similar format for multiple projects
```

### Design Patterns (from solution analysis)
- **Startup pattern**: `StartupBase` subclass wires commands (`ConfigureCommands`) and DI (`ConfigureServices`)
- **Command pattern**: Spectre.Console `Command<TSettings>` with inner `Settings` class for options/arguments
- **Functional style**: `Result<T>` monadic chaining with `.Bind()`, `.Map()`, `.Iter()`, `.Pipe()`, `.Render()`
- **Service abstraction**: Interfaces for file I/O (`IFileSystemAdapter`) and domain logic (`IDevLogService`)
- **Validation**: Dedicated validator classes using `Result<T>` chains (e.g., `FilePathValidator`)
- **Constants**: Static class with nested classes for organized string/error constants
- **Interactive mode**: `InteractiveCommandBase` as the default command with prompt loop
- **GlobalUsings**: Centralized global using statements per project

---

## Steps

### 1. ✅ Define Constants for DevLog
- **File**: `src/D20Tek.Tools.DevLog/Constants.cs` (modify)
- Add file extension constants (`.md`)
- Add data folder name constant (e.g., `devlogs`)
- Add file name format constant (e.g., `dev-log-{yyyyMMdd}.md`)
- Add date format constants for week-start calculation
- Add markdown template fragments (week header, project header)
- Add nested `Errors` class with validation error definitions
  - FolderPath required/not found
  - File not found error
  - ProjectName required
  - No entries found
- Add success/display message strings for each command

### 2. ✅ Create IFileSystemAdapter interface
- **File**: `src/D20Tek.Tools.DevLog/Services/IFileSystemAdapter.cs` (new)
- `bool Exists(string path)`
- `bool FolderExists(string path)`
- `bool EnsureFolderExists(string path)`
- `string ReadAllText(string path)`
- `void WriteAllText(string path, string contents)`
- `IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern)`

### 3. ✅ Create FileSystemAdapter implementation
- **File**: `src/D20Tek.Tools.DevLog/Services/FileSystemAdapter.cs` (new)
- Implement each method from IFileSystemAdapter using `System.IO`
- Follow the same implementation style as JsonMinify's FileSystemAdapter

### 4. ✅ Create IDevLogService interface
- **File**: `src/D20Tek.Tools.DevLog/Services/IDevLogService.cs` (new)
- `Result<bool> AddEntry(string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null)`
- `Result<string> ViewLog(string logFolder, DateOnly? date = null)`
- `Result<bool> EditEntry(string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null)`
- `Result<IEnumerable<string>> ListLogs(string logFolder)`

### 5. ✅ Create DevLogEntry model
- **File**: `src/D20Tek.Tools.DevLog/Contracts/DevLogEntry.cs` (new)
- `DateOnly WeekStart` - the Sunday date for this week
- `string ProjectName` - name of the project
- `List<string> Accomplishments` - list of bullet items
- Factory method: `Create(string projectName, List<string> accomplishments, DateOnly? date)`
- Helper: `GetWeekStart(DateOnly date)` - calculates the previous Sunday

### 6. ✅ Create DevLogService implementation
- **File**: `src/D20Tek.Tools.DevLog/Services/DevLogService.cs` (new)
- Inject `IFileSystemAdapter`
- Calculate weekly file path: `{logFolder}/devlog-{yyyy-MM-dd}.md` (where date = week's Sunday)
- **AddEntry**: Read existing file (or create new), append project section with accomplishments
- **ViewLog**: Read and return the markdown content for the week
- **EditEntry**: Parse existing file, find project section, replace accomplishments
- **ListLogs**: Enumerate `*.md` files in the log folder
- Markdown generation follows the template format exactly

### 7. ✅ Create AddEntryCommand
- **File**: `src/D20Tek.Tools.DevLog/Commands/AddEntryCommand.cs` (new)
- `Command<AddEntryCommand.Settings>`
- **Settings**:
  - `[CommandArgument(0, "<PROJECT>")]` - project name
  - `[CommandOption("-d|--date")]` - optional date override (defaults to today)
  - `[CommandOption("-f|--folder")]` - log folder path (defaults to `./devlogs`)
- **Execute**: Prompt for accomplishments interactively (multi-line input, empty line to finish)
- Chain: validate → display title → call service.AddEntry → render result

### 8. Create ViewLogCommand
- **File**: `src/D20Tek.Tools.DevLog/Commands/ViewLogCommand.cs` (new)
- `Command<ViewLogCommand.Settings>`
- **Settings**:
  - `[CommandOption("-d|--date")]` - optional date (defaults to current week)
  - `[CommandOption("-f|--folder")]` - log folder path (defaults to `./devlogs`)
- **Execute**: Read and render the markdown content for the specified week
- Display the formatted log content to the console

### 9. Create EditEntryCommand
- **File**: `src/D20Tek.Tools.DevLog/Commands/EditEntryCommand.cs` (new)
- `Command<EditEntryCommand.Settings>`
- **Settings**:
  - `[CommandArgument(0, "<PROJECT>")]` - project name to edit
  - `[CommandOption("-d|--date")]` - optional date override
  - `[CommandOption("-f|--folder")]` - log folder path (defaults to `./devlogs`)
- **Execute**: Load existing entry, show current accomplishments, prompt for replacements
- Chain: validate → load existing → prompt for edits → save → render result

### 10. Create ListLogsCommand
- **File**: `src/D20Tek.Tools.DevLog/Commands/ListLogsCommand.cs` (new)
- `Command<ListLogsCommand.Settings>`
- **Settings**:
  - `[CommandOption("-f|--folder")]` - log folder path (defaults to `./devlogs`)
- **Execute**: Enumerate and display all weekly log files with their week-of dates
- Format output as a table or list showing available weeks

### 11. Wire Startup with commands and DI registrations
- **File**: `src/D20Tek.Tools.DevLog/Startup.cs` (modify)
- Register commands:
  - `add` → `AddEntryCommand` (add a project entry to this week's log)
  - `view` → `ViewLogCommand` (view a week's log)
  - `edit` → `EditEntryCommand` (edit a project entry)
  - `list` → `ListLogsCommand` (list available weekly logs)
- Register services:
  - `IFileSystemAdapter` → `FileSystemAdapter` (singleton)
  - `IDevLogService` → `DevLogService` (singleton)

### 12. Update Program.cs GlobalUsings
- **File**: `src/D20Tek.Tools.DevLog/Program.cs` (verify)
- Ensure global usings include all required namespaces
- Verify `InteractiveCommand` remains the default command

### 13. Implement tests for dev-log
- Add DevLog folder to D20Tek.Tools.UnitTests project.
- Follow the patterns in the other test classes in D20Tek.Tools.UnitTests.
- Add unit tests for all commands and services. (no unit tests specifically for FileSystemAdapter).
- Add end-to-end tests to cover configuration, registration, and command execution flow.

---

## File Summary
| File | Action | Purpose |
|------|--------|---------|
| `Constants.cs` | modify | Add all string constants, errors, and messages |
| `Services/IFileSystemAdapter.cs` | new | File system abstraction interface |
| `Services/FileSystemAdapter.cs` | new | File system implementation |
| `Services/IDevLogService.cs` | new | Dev-log domain service interface |
| `Services/DevLogService.cs` | new | Markdown parsing, generation, weekly file management |
| `Contracts/DevLogEntry.cs` | new | Dev-log entry model with week calculation |
| `Commands/AddEntryCommand.cs` | new | Add project accomplishments to weekly log |
| `Commands/ViewLogCommand.cs` | new | View a week's dev-log |
| `Commands/EditEntryCommand.cs` | new | Edit existing project entries |
| `Commands/ListLogsCommand.cs` | new | List available weekly log files |
| `Startup.cs` | modify | Wire commands and DI registrations |
