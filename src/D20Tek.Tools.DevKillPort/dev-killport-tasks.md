# dev-killport — Development Tasks

**Tool:** D20Tek.Tools.DevKillPort
**Template:** D20Tek.Tools.DevLog (CLI patterns, DI, Spectre.Console)

---

## Design Overview

### Architecture Layers

```
CLI Layer (Spectre.Console.Cli commands + InteractiveCommand)
    ↓
Application Layer (Command handlers: KillCommand, ListCommand)
    ↓
Domain Layer (PortProcessInfo, PortState, PortQueryOptions)
    ↓
Infrastructure Layer (IPortResolver, IProcessTerminator — OS-specific implementations)
```

### Key Components

| Component | Purpose |
|-----------|---------|
| `PortProcessInfo` | Domain record: Port, PID, ProcessName, Protocol, Address, State |
| `PortState` | Enum: Listen, Established, TimeWait, etc. |
| `PortQueryOptions` | Options record: Protocol filter, etc. |
| `IPortResolver` | Interface to find processes bound to a port |
| `IProcessTerminator` | Interface to kill a process by PID |
| `PortResolverFactory` | Selects OS-specific `IPortResolver` |
| `WindowsPortResolver` | Windows implementation (PowerShell `Get-NetTCPConnection`) |
| `LinuxPortResolver` | Linux implementation (`ss` with `lsof`/`netstat` fallback) |
| `MacPortResolver` | macOS implementation (`lsof`) |
| `ProcessTerminator` | Cross-platform terminator using `Process.Kill()` |
| `KillPortCommand` | Main command: find + prompt + kill |
| `ListPortCommand` | List processes on a port without killing |
| `InteractiveCommand` | REPL-style interactive mode (default) |
| `Settings` | CLI settings: Port, --force, --all, --protocol, --json, --dry-run, --watch, --timeout |
| `Constants` | App-wide string constants |
| `Startup` | DI registration + command configuration |
| `Program` | Entry point |

### Project Dependencies

- D20Tek.Tools.Common (shared CLI infrastructure)
- D20Tek.Functional (Result type)
- D20Tek.Spectre.Console.Extensions (CommandAppBuilder, DI, InteractiveCommandBase)
- Spectre.Console (rendering, prompts)

---

## Development Order (optimized for least dependencies)

Phases are ordered so each phase depends only on previously completed phases.

### Phase 1: Domain Model (no dependencies)
### Phase 2: Infrastructure Interfaces (depends on Phase 1)
### Phase 3: Infrastructure Implementations (depends on Phase 2)
### Phase 4: Application/Command Layer (depends on Phase 3)
### Phase 5: CLI Wiring & Interactive Mode (depends on Phase 4)
### Phase 6: Output Formatting & JSON (depends on Phase 4)
### Phase 7: Advanced Features (depends on Phase 5-6)
### Phase 8: Testing (parallel with all phases)

---

## Tasks

### Phase 1: Domain Model

- [x] 1.1 Create `PortState` enum (Listen, Established, TimeWait, CloseWait, Other)
- [x] 1.2 Create `PortProcessInfo` record (Port, ProcessId, ProcessName, Protocol, Address, State)
- [x] 1.3 Create `PortQueryOptions` record (Protocol filter: Tcp/Udp/Both, Force, All, DryRun, Watch, Timeout)
- [x] 1.4 Create `Constants` class with app-wide strings (AppName, prompts, messages)
- [x] 1.5 Write unit tests for domain records

### Phase 2: Infrastructure Interfaces

- [x] 2.1 Create `IPortResolver` interface with `FindAsync(int port, PortQueryOptions, CancellationToken)`
- [x] 2.2 Create `IProcessTerminator` interface with `KillAsync(int pid, bool force, CancellationToken)`
- [x] 2.3 Create `ICommandRunner` interface for abstracting external process execution (testability)

### Phase 3: Infrastructure Implementations

- [x] 3.1 Create `CommandRunner` — runs external CLI commands and returns output
- [x] 3.2 Create `WindowsPortResolver` — uses `Get-NetTCPConnection` PowerShell command, parses output
- [x] 3.3 Create `LinuxPortResolver` — uses `ss` with fallback to `lsof` and `netstat`
- [x] 3.4 Create `MacPortResolver` — uses `lsof -i :<port>`
- [x] 3.5 Create `PortResolverFactory` — returns OS-specific resolver based on `OperatingSystem.Is*`
- [x] 3.6 Create `ProcessTerminator` — kills process by PID using `System.Diagnostics.Process`
- [x] 3.7 Write unit tests for parsing logic (Windows, Linux, macOS output parsing)
- [x] 3.8 Write unit tests for `PortResolverFactory`
- [ ] 3.9 Write unit tests for `ProcessTerminator` (mocked)

### Phase 4: Application / Command Layer

- [x] 4.1 Create `Settings` classes (PortSettings with port argument + all CLI options)
- [x] 4.2 Create `KillPortCommand` — find processes, display, prompt, kill, verify
- [x] 4.3 Create `ListPortCommand` — find and display processes on a port (no kill)
- [x] 4.4 Implement confirmation prompt logic (safe mode vs --force)
- [x] 4.5 Implement --dry-run behavior (show what would happen, skip kill)
- [x] 4.6 Implement --all flag (kill all matching vs prompt to select)
- [x] 4.7 Write unit tests for `KillPortCommand` (mocked resolver/terminator)
- [x] 4.8 Write unit tests for `ListPortCommand`

### Phase 5: CLI Wiring & Interactive Mode

- [x] 5.1 Create `Startup` class — register commands and DI services
- [x] 5.2 Create `InteractiveCommand` (default command, REPL mode)
- [x] 5.3 Wire up `Program.cs` with `CommandAppBuilder`, DI, and Startup
- [x] 5.4 Update `.csproj` — set AssemblyName to `dev-killport`, PackAsTool, metadata
- [x] 5.5 Write unit tests for `InteractiveCommand`
- [ ] 5.6 Manual smoke test on current OS

### Phase 6: Output Formatting & JSON

- [x] 6.1 Implement human-readable table output (PID, Name, Protocol, State)
- [x] 6.2 Implement --json flag for machine-readable JSON output
- [x] 6.3 Implement status messages (port free, TIME_WAIT, permission denied, etc.)
- [ ] 6.4 Write unit tests for output formatting

### Phase 7: Advanced Features

- [x] 7.1 Implement --watch mode (poll until port is free, with --timeout)
- [x] 7.2 Implement --protocol filter (tcp/udp/both)
- [x] 7.3 Implement IPv4/IPv6 deduplication by PID
- [ ] 7.4 Implement race condition mitigation (re-check PID before kill)
- [x] 7.5 Implement privilege elevation messaging (suggest sudo/admin)
- [ ] 7.6 Write unit tests for watch mode logic
- [ ] 7.7 Write unit tests for protocol filtering and deduplication

### Phase 8: Packaging & Polish

- [ ] 8.1 Add README.md for the project
- [ ] 8.2 Verify .NET global tool packaging (`dotnet pack` / `dotnet tool install`)
- [ ] 8.3 Cross-platform integration testing (Windows, Linux, macOS)
- [ ] 8.4 Final review and cleanup
