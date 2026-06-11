# clean-binobj design document

## Overview

`clean-binobj` is a cross‑platform .NET global tool that recursively finds and deletes `bin` and `obj` directories (and optionally other build artifacts) within a repository or specified path. It is designed as a fast, safe, and predictable alternative to `dotnet clean`, ad‑hoc scripts, and destructive commands like `git clean -xdf`.

The tool focuses on:

- **High‑confidence cleanup** of build artifacts (`bin`, `obj`, and optional extras).
- **Safety** via dry‑run, ignore patterns, depth limits, and confirmation prompts.
- **Performance** via efficient directory scanning and parallel deletion.
- **Zero‑config defaults** that “just work” for most .NET repos.
- **Discoverability** as `dotnet tool install -g clean-binobj`.

---

## Goals and non‑goals

### Goals

- **Fast repo‑wide cleanup:**
  - Recursively delete `bin` and `obj` directories from a root path.
- **Safe by default:**
  - Dry‑run mode, confirmation prompts for large deletions, ignore patterns, and depth limits.
- **Cross‑platform:**
  - Runs on Windows, macOS, and Linux with consistent behavior.
- **Scales to large repos:**
  - Efficient scanning and parallel deletion with bounded concurrency.
- **Scriptable and CI‑friendly:**
  - Stable exit codes, machine‑readable output mode, and non‑interactive options.
- **Minimal dependencies:**
  - Implemented as a single .NET tool with no heavy external libraries.

### Non‑goals

- **Not a build orchestrator:**
  - Does not run MSBuild, restore, or build.
- **Not a generic “delete anything” tool:**
  - Focused on build artifacts; extensibility is controlled and opinionated.
- **Not a replacement for `git clean`:**
  - Intentionally narrower and safer.

---

## UX and command structure

### Command model

Single primary command with sub‑commands for future extensibility:

- `clean-binobj` → default `clean` command.
- `clean-binobj clean` → explicit clean.
- `clean-binobj list` → list candidate directories without deleting.
- `clean-binobj stats` → show summary of what would be or was cleaned.
- `clean-binobj config` → show or validate config (future‑friendly).

### Top‑level syntax

```bash
clean-binobj [command] [options]
```

If `command` is omitted, `clean` is assumed.

### Commands and arguments

| Command          | Description                                      |
|------------------|--------------------------------------------------|
| `clean` (default)| Scan and delete `bin`/`obj` (and configured) dirs|
| `list`           | Scan and list candidate directories only         |
| `stats`          | Show summary stats for a scan                    |
| `config`         | Show effective configuration and exit            |
| `help`           | Show help for tool or specific command           |

#### Common options (all commands)

- `--root <path>`  
  **Description:** Root directory to start scanning from.  
  **Default:** Current working directory.

- `--include <pattern>` (repeatable)  
  **Description:** Additional directory names or glob patterns to treat as deletable build artifacts (e.g., `artifacts`, `out`).  
  **Default:** `bin`, `obj`.

- `--exclude <pattern>` (repeatable)  
  **Description:** Directory names or glob patterns to ignore entirely (e.g., `.git`, `.svn`, `node_modules`, `packages`).  
  **Default:** `.git`, `.svn`, `.hg`, `.vs`, `.idea`, `.vscode`, `node_modules`, `.gitlab`, `.azure-pipelines`.

- `--max-depth <n>`  
  **Description:** Maximum directory depth (relative to root) to scan.  
  **Default:** Unlimited (or a high default like `64` to avoid pathological trees).

- `--follow-symlinks` / `--no-follow-symlinks`  
  **Description:** Whether to follow symbolic links/junctions.  
  **Default:** Do not follow symlinks.

- `--concurrency <n>`  
  **Description:** Maximum number of concurrent deletions.  
  **Default:** Auto (e.g., logical processors or a capped value like 4–8).

- `--quiet`  
  **Description:** Minimal output; only errors and final summary.

- `--verbose`  
  **Description:** Detailed logging of scanning and deletion operations.

- `--json`  
  **Description:** Emit machine‑readable JSON output (for `list`, `stats`, and `clean` summaries).

- `--version`  
  **Description:** Print version and exit.

- `-h`, `--help`  
  **Description:** Show help.

#### `clean` command

```bash
clean-binobj clean [options]
# or simply:
clean-binobj [options]
```

**Options (in addition to common):**

- `--dry-run`  
  **Description:** Do not delete anything; show what *would* be deleted.

- `--yes`, `-y`  
  **Description:** Assume “yes” to prompts; non‑interactive mode.

- `--force`  
  **Description:** Skip safety checks like large‑deletion confirmation; intended for CI.

- `--older-than <duration>`  
  **Description:** Only delete directories whose last write time is older than the given duration (e.g., `1d`, `4h`, `2w`).  
  **Examples:** `--older-than 1d`, `--older-than 12h`.

- `--only <type>` (repeatable)  
  **Description:** Restrict deletion to specific artifact types.  
  **Values:** `bin`, `obj`, `extra` (for `--include` patterns).  
  **Default:** All.

- `--exclude-path <path-pattern>` (repeatable)  
  **Description:** Path‑based exclusion (e.g., `tests/*`, `samples/*`).

- `--preview-limit <n>`  
  **Description:** Limit number of directories shown in dry‑run or confirmation preview.  
  **Default:** 50.

#### `list` command

```bash
clean-binobj list [options]
```

**Behavior:**

- Scans using the same rules as `clean`, but never deletes.
- Outputs list of candidate directories.

**Options:**

- All common options.
- `--older-than`, `--only`, `--exclude-path` (same semantics as `clean`).

#### `stats` command

```bash
clean-binobj stats [options]
```

**Behavior:**

- Scans and computes:
  - Number of candidate directories.
  - Total size on disk (approximate).
  - Breakdown by type (`bin`, `obj`, `extra`).
- No deletion.

**Options:**

- All common options.
- `--older-than`, `--only`, `--exclude-path`.

#### `config` command

```bash
clean-binobj config [options]
```

**Behavior:**

- Shows effective configuration:
  - Default include/exclude patterns.
  - Overrides from environment variables.
  - Overrides from config file (if implemented).
- Future‑friendly for per‑repo config.

**Options:**

- `--root` (to locate repo config).
- `--json` (machine‑readable config).

---

## Safety model

### Safety principles

- **No surprises:** Default behavior is conservative and clearly communicated.
- **Reversible in practice:** Only deletes build artifacts that can be regenerated.
- **Guardrails for large deletions:** Confirmation prompts and thresholds.
- **Opt‑in for aggressive behavior:** `--force` and `--yes` are explicit.

### Dry‑run

- `--dry-run`:
  - Performs full scan.
  - Prints:
    - List of directories to delete (respecting `--preview-limit`).
    - Summary: count, total size, breakdown.
  - Exit code:
    - `0` if scan succeeded.
    - Non‑zero on errors.

### Ignore patterns

- **Default excludes:**
  - `.git`, `.svn`, `.hg`, `.vs`, `.idea`, `.vscode`, `node_modules`, `.gitlab`, `.azure-pipelines`.
- **User excludes:**
  - `--exclude` for directory names/globs.
  - `--exclude-path` for path‑based patterns.
- **Precedence:**
  - Excludes always win over includes.

### Depth limits

- `--max-depth`:
  - Prevents runaway scanning in deeply nested or misconfigured trees.
  - Depth is measured from `--root` (root depth = 0).
  - Directories beyond `max-depth` are not traversed.

### Large deletion confirmation

- When not in `--dry-run` and not in `--yes`/`--force`:
  - If number of directories or total size exceeds a threshold (e.g., >100 dirs or >5 GB):
    - Show a summary and a short preview list.
    - Prompt: “Proceed with deletion? [y/N]”.
- `--yes`:
  - Skips prompt but keeps other safety checks.
- `--force`:
  - Skips prompt and large‑deletion threshold checks.

### Symlink handling

- Default: do not follow symlinks.
- If `--follow-symlinks` is specified:
  - Detect cycles and avoid infinite loops.
  - Still respect excludes and depth limits.

---

## Parallel deletion strategy

### Goals

- **Speed:** Use multiple threads to delete directories in parallel.
- **Stability:** Avoid overwhelming the filesystem.
- **Determinism:** Order of deletion is not guaranteed, but behavior is.

### Approach

- **Two‑phase process:**
  1. **Scan phase:**
     - Single threaded or lightly parallelized directory enumeration.
     - Build an in‑memory list of candidate directories with metadata:
       - Full path.
       - Type (`bin`, `obj`, `extra`).
       - Depth.
       - Size estimate (optional, may be lazy).
  2. **Delete phase:**
     - Use a bounded concurrency scheduler (e.g., `Parallel.ForEachAsync` or a custom `Task` pool).
     - Concurrency level:
       - Default: `min(Environment.ProcessorCount, 8)` or similar.
       - Overridable via `--concurrency`.

- **Deletion semantics:**
  - Delete directories from deepest to shallowest to avoid parent/child conflicts.
  - For each directory:
    - Attempt recursive deletion.
    - On failure:
      - Log error.
      - Continue with others.
  - At the end:
    - Report number of successful and failed deletions.

- **Error handling:**
  - Permission issues, locked files, or transient IO errors:
    - Retries (optional, small count).
    - If still failing, record and continue.
  - Exit code:
    - `0` if all deletions succeeded.
    - `1` if some deletions failed.
    - `>1` for other error categories (e.g., invalid arguments).

---

## Repo scanning heuristics

### Default scanning behavior

- Start at `--root` (default: current directory).
- Depth‑first or breadth‑first traversal (implementation choice; DFS is simple).
- For each directory:
  - Check depth against `--max-depth`.
  - Check against exclude patterns and path excludes.
  - If directory name matches include patterns (`bin`, `obj`, extras):
    - Mark as candidate.
    - Do **not** traverse inside (no need to scan children).
  - Otherwise:
    - Continue traversal into children.

### Include patterns

- **Built‑in includes:**
  - `bin`, `obj`.
- **User includes:**
  - `--include artifacts`, `--include out`, etc.
- **Pattern matching:**
  - Simple name match by default.
  - Optional glob support (e.g., `*bin*`, `obj-*`) if not too heavy.

### Path‑based heuristics

- **Common .NET repo layouts:**
  - `src/*/bin`, `src/*/obj`.
  - `tests/*/bin`, `tests/*/obj`.
  - `samples/*/bin`, `samples/*/obj`.
- The tool does not hard‑code these paths but naturally finds them via name‑based includes.

### Age filtering

- `--older-than <duration>`:
  - For each candidate directory:
    - Compute last write time (max of directory and its contents).
    - If newer than threshold, skip.
- This allows:
  - Keeping recent builds.
  - Cleaning only stale artifacts.

---

## Performance considerations

### Scanning

- **Use efficient APIs:**
  - `Directory.EnumerateDirectories` instead of `GetDirectories` to stream results.
- **Avoid unnecessary work:**
  - Do not compute directory sizes unless needed (e.g., for `stats` or large‑deletion thresholds).
  - When size is needed:
    - Use a separate pass or lazy computation.
- **Short‑circuit traversal:**
  - Once a directory is identified as a candidate (e.g., `bin`), do not traverse inside it.

### Deletion

- **Bounded concurrency:**
  - Avoid saturating disk with too many parallel deletions.
- **Platform quirks:**
  - Windows:
    - Handle read‑only attributes by clearing them before deletion.
  - Unix:
    - Standard recursive delete is usually straightforward.

### Memory usage

- **Candidate list:**
  - Store only necessary metadata.
  - For extremely large repos, consider streaming deletion (scan and delete in chunks), but initial version can hold candidates in memory.

### Logging

- **Quiet mode:**
  - Minimal output for CI.
- **Verbose mode:**
  - Detailed logs for troubleshooting.
- **JSON mode:**
  - Structured output for tooling integration.

---

## Implementation plan

### Technology stack

- **Language:** C#.
- **Target framework:** `net8.0` (or latest LTS).
- **Packaging:** .NET global tool (`dotnet tool`).

### Project structure

- **Project:** `D20Tek.Tools.DevCleanBinObj.Tool`
  - `Program.cs`:
    - Entry point, command routing.
  - `Commands/`:
    - `CleanCommand`
    - `ListCommand`
    - `StatsCommand`
    - `ConfigCommand`
  - `Core/`:
    - `Scanner` (directory traversal and candidate discovery).
    - `Deleter` (parallel deletion).
    - `Patterns` (include/exclude matching).
    - `Config` (defaults, env, future config file).
    - `Output` (console + JSON formatting).
  - `Models/`:
    - `CandidateDirectory` (path, type, depth, size, timestamps).
    - `ScanResult`, `DeleteResult`, `StatsResult`.

### Argument parsing

- Use a lightweight, dependency‑minimal library or .NET’s built‑in `Spectre.Console.Cli`.
- Requirements:
  - Sub‑commands.
  - Options with aliases.
  - Help generation.

Here is a **fully rewritten, Spectre.Console.Cli–targeted design document** for `clean-binobj`.  
This version assumes Spectre.Console.Cli is the CLI framework and updates the command model, settings classes, validation strategy, and UX patterns accordingly.

It keeps the founder‑grade clarity and the architectural rigor you expect.

---

## Command Model (Spectre.Console.Cli)

Spectre.Console.Cli uses:

- A **CommandApp**
- **Command classes** implementing `ICommand` or `AsyncCommand`
- **Settings classes** with `[CommandOption]` attributes
- Optional `Validate()` overrides

This maps perfectly to `clean-binobj`.

---

# Commands

## 1. Default command: `clean`

Runs when no command is specified:

```
clean-binobj
clean-binobj clean
```

### Purpose
Scan for build artifact directories and delete them.

### Settings class (conceptual)

```csharp
public sealed class CleanSettings : CommandSettings
{
    [CommandOption("--root <PATH>")]
    public string? Root { get; set; }

    [CommandOption("--include <PATTERN>")]
    public List<string> Includes { get; set; } = new();

    [CommandOption("--exclude <PATTERN>")]
    public List<string> Excludes { get; set; } = new();

    [CommandOption("--exclude-path <PATH>")]
    public List<string> ExcludePaths { get; set; } = new();

    [CommandOption("--only <TYPE>")]
    public List<string> Only { get; set; } = new();

    [CommandOption("--older-than <DURATION>")]
    public string? OlderThan { get; set; }

    [CommandOption("--max-depth <N>")]
    public int? MaxDepth { get; set; }

    [CommandOption("--concurrency <N>")]
    public int? Concurrency { get; set; }

    [CommandOption("--dry-run")]
    public bool DryRun { get; set; }

    [CommandOption("--yes|-y")]
    public bool Yes { get; set; }

    [CommandOption("--force")]
    public bool Force { get; set; }

    [CommandOption("--quiet")]
    public bool Quiet { get; set; }

    [CommandOption("--verbose")]
    public bool Verbose { get; set; }

    [CommandOption("--json")]
    public bool Json { get; set; }

    public override ValidationResult Validate()
    {
        if (MaxDepth is < 0)
            return ValidationResult.Error("Max depth must be non-negative.");

        if (Concurrency is < 1)
            return ValidationResult.Error("Concurrency must be >= 1.");

        return ValidationResult.Success();
    }
}
```

### Behavior
- Scan repo for `bin`, `obj`, and user‑included directories
- Apply excludes and depth limits
- Apply age filtering
- If `--dry-run`: show preview only
- If not dry-run:
  - Confirm large deletions unless `--yes` or `--force`
  - Delete directories in parallel
- Output summary (human or JSON)

---

## 2. `list` command

```
clean-binobj list
```

### Purpose
List candidate directories without deleting anything.

### Settings
Same as `clean`, minus deletion‑specific flags:

- `--root`
- `--include`
- `--exclude`
- `--exclude-path`
- `--only`
- `--older-than`
- `--max-depth`
- `--quiet`
- `--verbose`
- `--json`

### Behavior
- Perform full scan
- Output list of directories
- No deletion

---

## 3. `stats` command

```
clean-binobj stats
```

### Purpose
Compute and display:

- Number of candidate directories
- Total size on disk
- Breakdown by type (`bin`, `obj`, extras)

### Settings
Same as `list`.

### Behavior
- Scan
- Compute size (lazy or second pass)
- Output summary (table or JSON)

---

## 4. `config` command

```
clean-binobj config
```

### Purpose
Show effective configuration:

- Default includes/excludes
- CLI overrides
- Environment overrides
- Future config file support

### Settings
- `--root`
- `--json`

---

## Performance Model

### Scanning
- DFS or BFS traversal
- Use `Directory.EnumerateDirectories`
- Skip children of matched directories
- Lazy size computation

### Deletion
- Parallel deletion using `Parallel.ForEachAsync`
- Concurrency capped by:
  - CPU count
  - User override

### Error handling
- Retry on transient IO errors
- Log failures
- Exit code:
  - `0` success
  - `1` partial failure
  - `2` invalid arguments
  - `3` aborted by user

---

## Repo Scanning Heuristics

### Includes
- Built‑in: `bin`, `obj`
- User: `--include artifacts`, etc.

### Excludes
- Name‑based: `--exclude`
- Path‑based: `--exclude-path`

### Age filtering
`--older-than 1d`, `--older-than 4h`, etc.

### Only filtering
`--only bin`, `--only obj`, `--only extra`

---

## Spectre.Console Integration

### Rich output
- Tables for stats
- Trees for previews
- Panels for summaries
- Color‑coded warnings
- Progress indicators for deletion

### Prompts
- `AnsiConsole.Confirm()` for large deletions
- `AnsiConsole.Markup()` for warnings

### JSON output
- Use `System.Text.Json` for serialization
- Spectre handles formatting around it

---

# Implementation Architecture

```
CleanBinObj/
 ├── Program.cs
 ├── Commands/
 │    ├── CleanCommand.cs
 │    ├── ListCommand.cs
 │    ├── StatsCommand.cs
 │    └── ConfigCommand.cs
 ├── Settings/
 │    ├── CleanSettings.cs
 │    ├── ListSettings.cs
 │    ├── StatsSettings.cs
 │    └── ConfigSettings.cs
 ├── Core/
 │    ├── Scanner.cs
 │    ├── Deleter.cs
 │    ├── Patterns.cs
 │    ├── SizeCalculator.cs
 │    ├── OutputRenderer.cs
 │    └── ConfigLoader.cs
 └── Models/
      ├── CandidateDirectory.cs
      ├── ScanResult.cs
      ├── DeleteResult.cs
      └── StatsResult.cs
```

### CommandApp setup

```csharp
var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<CleanCommand>("clean")
        .WithDescription("Clean bin/obj directories.");

    config.AddCommand<ListCommand>("list")
        .WithDescription("List candidate directories.");

    config.AddCommand<StatsCommand>("stats")
        .WithDescription("Show statistics.");

    config.AddCommand<ConfigCommand>("config")
        .WithDescription("Show effective configuration.");

    config.SetDefaultCommand<CleanCommand>();
});

return app.Run(args);
```

---

### Testing

- **Unit tests:**
  - Pattern matching.
  - Depth and exclude logic.
  - Age filtering.
  - JSON output shape.
- **Integration tests:**
  - Temporary directory trees with nested `bin`/`obj`.
  - Cross‑platform path handling.
  - Dry‑run vs real deletion.
  - Large repo simulation (many directories).

---

## Packaging and distribution strategy

### Distribution

- **As a .NET global tool:**

  ```bash
  dotnet tool install -g clean-binobj
  ```

- **Update:**

  ```bash
  dotnet tool update -g clean-binobj
  ```

- **Uninstall:**

  ```bash
  dotnet tool uninstall -g clean-binobj
  ```

### Versioning

- **Semantic versioning:**
  - `1.x` for stable CLI surface.
  - Breaking changes only in major versions.

### Discoverability

- **README highlights:**
  - Problem statement: haunted builds, branch switches, stale artifacts.
  - Quick start: `clean-binobj --dry-run`, then `clean-binobj`.
  - Safety features and examples.
- **Examples:**
  - `clean-binobj` (default clean).
  - `clean-binobj --dry-run --root .`
  - `clean-binobj --older-than 1d --yes`
  - `clean-binobj list --json`
  - `clean-binobj clean --include artifacts --exclude-path samples/*`

### CI usage

- Recommended pattern:

  ```bash
  dotnet tool install -g clean-binobj
  clean-binobj --force --quiet
  ```

- Or with age filtering:

  ```bash
  clean-binobj --older-than 1d --force --quiet
  ```

---

## Example UX flows

### Developer with haunted build

```bash
# See what would be deleted
clean-binobj --dry-run

# Looks good, actually clean
clean-binobj
```

### Monorepo with extra artifacts

```bash
clean-binobj clean --include artifacts --include out
```

### CI pipeline

```bash
clean-binobj clean --force --quiet --older-than 1d
```

### Large repo, cautious cleanup

```bash
clean-binobj clean --dry-run --max-depth 8 --exclude-path samples/*
clean-binobj clean --max-depth 8 --exclude-path samples/*
```

---
