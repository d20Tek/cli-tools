# dev-killoprt — Cross-Platform Port Process Killer

**Design Document (v1.0)**

---

## 1. Overview

**dev-killport** is a cross-platform command-line tool that allows developers to:

* Identify processes bound to a given port
* Safely terminate those processes
* Handle edge cases across Windows, Linux, and macOS
* Provide both human-friendly and machine-readable output

### Core Value Proposition

> “Instantly free a port, safely and reliably, regardless of OS.”

---

## 2. Goals & Non-Goals

### Goals

* Cross-platform support (Windows, Linux, macOS)
* Reliable port → process resolution
* Safe process termination (default)
* Clean CLI UX for technical users
* Scriptable output (`--json`)
* Extensible architecture

### Non-Goals

* Full network diagnostics tool (not replacing `netstat`, `ss`, etc.)
* Managing remote systems
* Deep container introspection (v1)

---

## 3. High-Level Architecture

```
CLI Layer (D20Tek.Spectre.Console.Extensions / Spectre.Console)
        ↓
Application Layer (Command Handlers)
        ↓
Domain Layer (PortProcessInfo, Policies)
        ↓
Infrastructure Layer (OS-specific resolvers)
```

---

## 4. Core Components

### 4.1 Domain Model

```csharp
public class PortProcessInfo
{
    public int Port { get; init; }
    public int ProcessId { get; init; }
    public string ProcessName { get; init; }
    public string Protocol { get; init; } // TCP/UDP
    public string Address { get; init; }  // 0.0.0.0, [::], etc.
    public PortState State { get; init; } // LISTEN, ESTABLISHED, etc.
}
```

---

### 4.2 Core Interfaces

```csharp
public interface IPortResolver
{
    Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct);
}

public interface IProcessTerminator
{
    Task KillAsync(int pid, bool force, CancellationToken ct);
}
```

---

## 5. OS-Specific Strategies

This is the **most critical part of the design**.

---

### Windows Strategy

#### Preferred Approach: Native API (P/Invoke)

Use:

* `GetExtendedTcpTable`
* `GetExtendedUdpTable`

#### Why:

* No parsing fragile CLI output
* Fast and reliable
* Works without external dependencies

#### Fallback:

* PowerShell:

  ```powershell
  Get-NetTCPConnection -LocalPort <port>
  ```

#### Edge Cases:

* Requires elevation for full visibility
* IPv4/IPv6 duplication
* Some system processes cannot be killed

---

### Linux Strategy

#### Preferred Tool: `ss`

```bash
ss -ltnp 'sport = :5000'
```

#### Why:

* Modern replacement for `netstat`
* Faster than `lsof`
* Widely available (but not guaranteed)

#### Fallback Order:

1. `ss`
2. `lsof -i :<port>`
3. `netstat -tulpn`

#### Edge Cases:

* `ss` output varies slightly by distro
* PID visibility may require `sudo`
* Container namespaces may hide processes

---

### macOS Strategy

#### Primary Tool: `lsof`

```bash
lsof -i :5000
```

#### Why:

* Standard on macOS
* Reliable enough despite slower performance

#### Edge Cases:

* Parsing required
* Permissions limit visibility
* Slower on large systems

---

## 6. Strategy Selection

```csharp
public class PortResolverFactory
{
    public static IPortResolver Create()
    {
        if (OperatingSystem.IsWindows())
            return new WindowsPortResolver();

        if (OperatingSystem.IsLinux())
            return new LinuxPortResolver();

        if (OperatingSystem.IsMacOS())
            return new MacPortResolver();

        throw new PlatformNotSupportedException();
    }
}
```

---

## 7. Command-Line Interface

### Command: `dev-killport`

#### Basic Usage

```bash
dev-killport 5000
```

---

### Options

| Option       | Description                 |
| ------------ | --------------------------- |
| `--force`    | Kill without confirmation   |
| `--all`      | Kill all matching processes |
| `--protocol` | tcp / udp / both            |
| `--json`     | Output JSON                 |
| `--watch`    | Wait until port is free     |
| `--dry-run`  | Show what would happen      |
| `--timeout`  | Max wait time (for watch)   |

---

### Examples

```bash
dev-killport 5000 --force
dev-killport 3000 --protocol tcp
dev-killport 8080 --json
dev-killport 5000 --watch
```

---

## 8. Application Flow

```
Parse CLI args
   ↓
Resolve OS strategy
   ↓
Find processes bound to port
   ↓
Display results
   ↓
Apply policy (safe vs force)
   ↓
Kill selected processes
   ↓
Verify port is freed
```

---

## 9. Safety Model (Important)

### Default Behavior (Safe Mode)

* Show processes
* Require confirmation
* Show:

  * PID
  * Process name
  * Protocol
  * State

---

### Force Mode

```bash
dev-killport 5000 --force
```

* No prompt
* Kill immediately

---

### Race Condition Mitigation

Before killing:

1. Re-check PID still owns port
2. Confirm process name matches

---

## 10. Output Design

### Human Output

```text
Port 5000 is used by:

  PID   Name      Protocol   State
  1234  dotnet    TCP        LISTEN

Kill this process? (y/N)
```

---

### JSON Output

```json
{
  "port": 5000,
  "processes": [
    {
      "pid": 1234,
      "name": "dotnet",
      "protocol": "TCP",
      "state": "LISTEN"
    }
  ]
}
```

---

## 11. Error Handling

### Common Cases

| Scenario           | Behavior                  |
| ------------------ | ------------------------- |
| No process found   | Exit 0 with message       |
| Permission denied  | Warn + suggest sudo/admin |
| Tool not available | Try fallback              |
| Multiple processes | Prompt user               |
| Kill fails         | Report + continue         |

---

## 12. Cross-Platform Edge Cases

### 12.1 Multiple Bindings

* `SO_REUSEPORT`
* Multiple PIDs per port

Default: prompt user

---

### 12.2 TIME_WAIT

* No process found

Output:

> “Port is in TIME_WAIT state; no active process to kill.”

---

### 12.3 IPv4 vs IPv6

* Deduplicate by PID

---

### 12.4 Containers

* May not see host/container processes

Future feature: `--namespace` support

---

## 13. Extensibility

Future enhancements:

* Docker integration (`docker ps` mapping)
* Remote SSH execution
* Port range support (`5000-5010`)
* Interactive TUI mode
* Plugin system for resolvers

---

## 14. Packaging & Distribution

### As a .NET Global Tool

```bash
dotnet pack
dotnet tool install -g dev-killport
```

### Requirements

* .NET 10 SDK

---

## 15. Testing Strategy

### Unit Tests

* Parsing logic per OS
* Resolver behavior (mocked)

### Integration Tests

* Spin up test servers
* Bind to ports
* Verify detection + kill

### OS Matrix

* Windows (latest + LTS)
* Ubuntu
* Omarchy
* macOS

---

## 16. Performance Considerations

* Prefer native APIs over shelling out
* Cache tool availability (`ss`, `lsof`)
* Avoid repeated process spawning
* Timeout external commands

---

## 17. Security Considerations

* Never auto-escalate privileges
* Require explicit user intent (`--force`)
* Validate inputs (port ranges)
* Avoid killing system-critical processes unless forced

---

## 18. Example Internal Flow
This just a simple example of what we expect, but not the full flow. Make any changes you think are necessary for the full implementation.

```csharp
var resolver = PortResolverFactory.Create();
var processes = await resolver.FindAsync(port, options, ct);

if (!processes.Any())
{
    Console.WriteLine("Port is free.");
    return;
}

if (!options.Force)
{
    PromptUser(processes);
}

foreach (var proc in processes)
{
    await terminator.KillAsync(proc.ProcessId, options.Force, ct);
}
```

---

## 19. Final Assessment

### Complexity

* **Medium → High** (if done correctly)

### Differentiator

Most tools:

* Hacky
* OS-specific
* Unsafe

**dev-killport becomes:**

* Cross-platform
* Safe by default
* Architecturally clean
* Extensible

---

## 20. Recommendation

Start with:

1. Linux (`ss`)
2. macOS (`lsof`)
3. Windows (CLI fallback first, then upgrade to P/Invoke)

Then iterate toward:

* Native Windows API
* Better parsing abstraction
* Advanced UX features
