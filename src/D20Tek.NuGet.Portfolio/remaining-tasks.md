# NuGet Portfolio - Remaining Tasks

## Overview
The NuGet Portfolio (`nu-port`) CLI tool helps NuGet package developers track historic download
numbers over time. It organizes packages into collections, fetches current download counts from
nuget.org, and persists daily snapshots in a local SQLite database for historical comparison.

## Current State
The following features are **implemented and tested**:
- **Domain entities**: CollectionEntity, TrackedPackageEntity, PackageSnapshotEntity, DateRange
- **Persistence**: EF Core + SQLite with migrations, CRUD operations, snapshot upsert/delete/query
- **Collections**: Add, Edit, Delete, List commands (all tested)
- **Tracked Packages**: Add, Edit, Delete, List commands (all tested)
- **Package Downloads (live)**: GetByPackageId, GetByCollectionId, GetAllPackages (all tested)
- **Snapshots**: Add (save today's downloads), Delete (by date), ListToday (all tested)
- **Services**: NuGetSearchClient (API-based) and NuGetScrapingClient (HTML scraping) - both tested
- **Interactive mode**: Working interactive REPL command loop

## Where Implementation Left Off
The snapshot feature area is partially complete. `ListWeekByCollectionCommand` exists but is marked
`[ExcludeFromCodeCoverage]` and has no unit tests. The `DateRange` record defines `ForMonthEnding`
and `ForYearEnding` helpers, but no corresponding list commands use them yet. The weekly/monthly/yearly
snapshot views would form the core historical tracking experience but are not wired up or tested.

---

## Remaining Tasks

### 0. Upgrade the project to .NET 10
- Update `src/D20Tek.NuGet.Portfolio/D20Tek.NuGet.Portfolio.csproj` to target `net10.0`
- Update other project dependencies to their latest versions compatible with .NET 10

### 1. Complete ListWeekByCollectionCommand
- **File**: `src/D20Tek.NuGet.Portfolio/Features/Snapshots/ListWeekByCollectionCommand.cs`
- Remove `[ExcludeFromCodeCoverage]` attribute once implementation is finalized
- Verify the weekly snapshot table rendering shows date-grouped rows properly
- **Test**: Create `tests/.../NuGetPortfolio/Features/Snapshots/ListWeekByCollectionCommandTests.cs`

### 2. Add ListMonthByCollectionCommand
- **File**: `src/D20Tek.NuGet.Portfolio/Features/Snapshots/ListMonthByCollectionCommand.cs` (new)
- Follow the same pattern as `ListWeekByCollectionCommand` but use `DateRange.ForMonthEnding()`
- Register in `SnapshotsCommandConfiguration` with alias `m` / `month`
- **Test**: Create `tests/.../NuGetPortfolio/Features/Snapshots/ListMonthByCollectionCommandTests.cs`

### 3. Add ListYearByCollectionCommand
- **File**: `src/D20Tek.NuGet.Portfolio/Features/Snapshots/ListYearByCollectionCommand.cs` (new)
- Follow the same pattern using `DateRange.ForYearEnding()`
- Register in `SnapshotsCommandConfiguration` with alias `y` / `year`
- **Test**: Create `tests/.../NuGetPortfolio/Features/Snapshots/ListYearByCollectionCommandTests.cs`

### 4. Add DownloadsHelper unit tests
- **File**: `tests/.../NuGetPortfolio/Features/Helpers/DownloadsHelperTests.cs` (new)
- Test `RetrieveDownloadSnapshots` with success/failure scenarios using `FakeNuGetSearchClient`
- Test `RenderDownloadSnapshots` renders table output

### 5. Add DownloadsTableBuilder unit tests
- **File**: `tests/.../NuGetPortfolio/Features/Helpers/DownloadsTableBuilderTests.cs` (new)
- Test `WithHeader`, `WithRows` (empty and non-empty), `WithTotals`, `Build`

### 6. Enhance snapshot table rendering for date ranges
- The current `DownloadsTableBuilder` shows a single-day view (Id, PackageId, Downloads)
- For week/month/year views, consider adding a Date column or grouping by date
- Alternatively, create a `SnapshotRangeTableBuilder` that shows columns per date or
  summary rows with delta/change calculations

### 7. Add download trend/delta display
- When listing snapshots for a range, calculate and display the change in downloads
  (e.g., downloads gained per day/week)
- Show totals, averages, or percentage growth over the selected time period

### 8. Add CSV/JSON export command
- **File**: `src/D20Tek.NuGet.Portfolio/Features/Snapshots/ExportSnapshotsCommand.cs` (new)
- Allow exporting snapshot data to CSV or JSON for external analysis
- Register in `SnapshotsCommandConfiguration` with alias `export` / `e`
- **Test**: Create corresponding test file

### 9. Add AppDbSnapshotOperations unit tests for date range queries
- The `GetSnapshotsForCollection(context, collectionId, DateRange)` overload is marked
  `[ExcludeFromCodeCoverage]` - add tests and remove the attribute
- Test with various date ranges (week, month, year boundaries)

### 10. Add summary/dashboard command
- Create a command that shows an overview of all collections with their latest snapshot
  totals and recent trends
- Provide a quick at-a-glance view of portfolio health
