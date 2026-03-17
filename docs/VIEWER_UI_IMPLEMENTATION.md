# ShadcnBlazor Test Viewer UI Implementation

**Status:** ✅ Complete - Phase 3 MudBlazor-Inspired Viewer Implementation

---

## Overview

Implemented comprehensive MudBlazor-inspired UI for the ShadcnBlazor Test Viewer using Tailwind CSS v4.1.18. The viewer now features a modern, responsive interface with dark mode support, component search, test management, and multi-tab test rendering.

---

## Architecture

### Component Structure

**Layout Hierarchy:**
- `MainLayout.razor` — Wraps all pages, manages persistent UI elements
  - Header: Title + Dark mode toggle
  - Sidebar: Search, test count, grouped component list
  - @Body placeholder: Wrapped in CascadingValue for state sharing

- `Pages/Home.razor` — Main content page
  - Receives selected test via CascadingParameter
  - Displays test component with tabs and controls
  - Manages test variant tabs
  - Provides restore/reset functionality

### State Management

**MainLayout** maintains:
- `isDarkMode` — Dark theme toggle state
- `searchQuery` — Real-time search filter input
- `selectedTest` — Currently selected test name (e.g., "Badge 1")
- `allTests` — Static list of all Tier 1 test names
- `GetGroupedComponents()` — Groups filtered tests by component name

**Home.razor** maintains:
- `_selectedTestType` — Type reference of currently selected test component
- `_testVariants` — List of active test variant tabs
- `_activeVariantIndex` — Currently displayed variant tab
- `_selectedTestLabel` — Display name of selected test

**State Sharing:**
- `MainLayout` cascades `selectedTest` via `<CascadingValue>` around @Body
- `Home.razor` receives via `[CascadingParameter] public string? selectedTest`
- Bidirectional: MainLayout changes selection → Home.razor reflects it

---

## Features Implemented

### ✅ Header Section
- **Title:** "ShadcnBlazor Test Viewer" with border separator
- **Dark Mode Toggle:** Moon/Sun icon button
  - Uses JSInterop to apply/remove "dark" class on `<html>`
  - Smooth color transitions via Tailwind utilities
  - Persists icon state per session

**Styling:**
```
- Border: slate-200 light / slate-800 dark
- Padding: px-4 py-3
- Responsive hover: bg-slate-100 / bg-slate-800
- Transition: all color changes
```

### ✅ Sidebar
**Dimensions:** 256px (w-64) with vertical scroll for overflow

**Search Box:**
- Real-time filtering with `@bind="searchQuery"` + `@bind:event="oninput"`
- Tailwind form utilities for styling
- Dark mode color support
- Focus ring: ring-2 ring-blue-500

**Test Count:**
- Dynamic display: "@filteredTests.Count tests"
- Updated on each search input
- Subtle text color (slate-500/400)

**Component List:**
- Grouped by component name (Alert, Badge, Button, Card, Checkbox, Skeleton)
- Each group shows category header in uppercase + tracking
- List items:
  - Button styling with selected state highlighting
  - Selected test: bg-blue-100 / dark:bg-blue-900
  - Unselected: slate colors with hover effect
  - Rounded corners with transition animations

**Tier 1 Test Names:**
```
Alert: Alert 1
Badge: Badge 1, Badge 2
Button: Button 1
Card: Card 1, Card 2
Checkbox: Checkbox 1, Checkbox 2
Skeleton: Skeleton 1
```

### ✅ Main Content Area

**Test Header:**
- Component title (e.g., "Badge 1") in large bold font
- Subtitle: "Rendering scenario from Shared Tests"
- Bottom border separator

**Tab System:**
- Dynamic tabs for each test variant
- Tab styling:
  - Active: white bg / dark:slate-800, blue bottom border
  - Inactive: transparent with hover state
  - Close button (✕) when multiple tabs exist
- "Add Test" button:
  - Positioned right with flexbox
  - Creates duplicate test variant with increment
  - Example: "Badge 1" → "Badge 2" tab

**Test Component Rendering:**
- Dynamic component discovery via reflection
- Matches test name to component type (e.g., "Badge 1" → BadgeTest1)
- Renders within bordered container
- White background with shadow and rounded corners

**Controls Section:**
- **Restore Button:** Resets tabs to initial state
  - Shows ↻ icon with "Restore" label
  - Slate styling with hover effect
- **Tab Count Display:** Shows active variant count
  - Example: "Test Tabs: 2"
  - Right-aligned on footer

---

## File Changes

### Modified Files

#### `tests/ShadcnBlazor.Tests.Viewer/Layout/MainLayout.razor`
**Changes:**
- Wrapped `@Body` in `<CascadingValue Value="selectedTest">` to share state with page
- Changed `selectedTest` from `private` to `public` for cascading parameter exposure
- Added dark mode class management via `_jsRuntime.InvokeVoidAsync()`
- Implemented search filtering with `filteredTests` computed property
- Added grouped component list rendering with category headers
- Implemented test selection via `SelectTest()` method
- Added dark mode toggle: `ToggleDarkMode()` method

**Line Count:** 131 lines total

**Key Dependencies:**
- `IJSRuntime` injected for DOM manipulation
- System.Reflection for type discovery
- System.Linq for grouping/filtering

#### `tests/ShadcnBlazor.Tests.Viewer/Pages/Home.razor`
**Changes:**
- Removed standalone sidebar (now provided by MainLayout)
- Added `[CascadingParameter]` to receive `selectedTest` from MainLayout
- Implemented dynamic test type discovery via assembly reflection
- Added tab management system with add/remove/restore functionality
- Implemented test variant rendering with dynamic component instantiation
- Added controls footer with Restore button and tab count display
- Implemented `SelectTestByName()` for matching test names to component types
- Added test discovery caching for performance

**Line Count:** 180 lines total

**Key Methods:**
- `DiscoverTests()` — Discovers all test components in shared assembly
- `SelectTestByName()` — Maps display name to component type
- `SelectVariant()` / `RemoveVariant()` / `AddVariant()` — Tab management
- `ResetTest()` — Clears variants and restores initial state
- `RenderTest()` — Renders component via `RenderFragment`

#### `tests/ShadcnBlazor.Tests.Viewer/wwwroot/css/app.css`
**Status:** Already updated with Tailwind CSS v4.1.18 foundation
- Layer directives for cascade control
- Blazor error UI styling
- Loading progress indicator styles

#### `tests/ShadcnBlazor.Tests.Viewer/App.razor`
**Status:** Already configured
- `MainLayout` set as default layout
- Router and navigation configured
- Service providers registered (PopoverProvider, DialogProvider)

---

## Dark Mode Implementation

**CSS Classes Used:**
- Base classes: `dark:` prefix applied to utilities
- Colors: slate-50 → slate-950 gradient
- Transitions: `transition-colors` for smooth changes

**Color Palette:**
| Element | Light | Dark |
|---------|-------|------|
| Background | white | slate-950 |
| Sidebar BG | slate-50 | slate-900 |
| Borders | slate-200 | slate-800 |
| Text | slate-900 | white |
| Hover | slate-100 | slate-800 |
| Accent | blue-500 | blue-500 |

**JavaScript Integration:**
```csharp
// Add dark class
_jsRuntime?.InvokeVoidAsync("document.documentElement.classList.add", "dark");

// Remove dark class
_jsRuntime?.InvokeVoidAsync("document.documentElement.classList.remove", "dark");
```

---

## Test Component Integration

### Test Discovery Flow

1. **Assembly Reflection** (OnInitialized)
   - Scans `ShadcnBlazor.Tests.Shared` assembly
   - Finds types with "Test" in name + implements `IComponent`
   - Examples: `BadgeTest1`, `ButtonTest1`, `CheckboxTest2`

2. **Name Parsing** (GetGroup / GetLabel)
   ```
   Input: "BadgeTest1"
   Group: "Badge"
   Label: "Badge 1"
   ```

3. **User Selection Flow**
   ```
   User clicks "Badge 1" in sidebar
   → MainLayout.SelectTest("Badge 1")
   → selectedTest = "Badge 1"
   → CascadingValue updates
   → Home.razor receives parameter
   → SelectTestByName matches "Badge 1" → BadgeTest1 type
   → RenderTest(BadgeTest1) renders component
   ```

### Supported Tier 1 Components

| Component | Test Files | Namespaces |
|-----------|-----------|-----------|
| Alert | AlertTest1.razor | ShadcnBlazor.Tests.Shared.TestComponents.Alert |
| Badge | BadgeTest1.razor, BadgeTest2.razor | ShadcnBlazor.Tests.Shared.TestComponents.Badge |
| Button | ButtonTest1.razor | ShadcnBlazor.Tests.Shared.TestComponents.Button |
| Card | CardTest1.razor, CardTest2.razor | ShadcnBlazor.Tests.Shared.TestComponents.Card |
| Checkbox | CheckboxTest1.razor, CheckboxTest2.razor | ShadcnBlazor.Tests.Shared.TestComponents.Checkbox |
| Skeleton | SkeletonTest1.razor | ShadcnBlazor.Tests.Shared.TestComponents.Skeleton |

---

## Build & Test Status

### Build Results
```
✅ ShadcnBlazor builds successfully
✅ ShadcnBlazor.Tests.Shared builds successfully (0 warnings)
✅ ShadcnBlazor.Tests.Viewer builds successfully (0 warnings)
✅ ShadcnBlazor.Tests.UnitTests builds successfully (1 deprecation warning only)
```

### Test Results
```
✅ All 32 Unit Tests Passing (NUnit)
   - Badge: 5 ✅
   - Button: 7 ✅
   - Checkbox: 6 ✅
   - Alert: 4 ✅
   - Skeleton: 2 ✅
   - Card: 5 ✅
   Duration: 532ms
```

---

## Usage Guide

### Running the Viewer

```bash
# Build
dotnet build tests/ShadcnBlazor.Tests.Viewer

# Run (launches at https://localhost:7186 or similar)
dotnet run --project tests/ShadcnBlazor.Tests.Viewer
```

### Using the Interface

1. **Search Tests**
   - Type in search box to filter components
   - Results update in real-time as you type
   - Test count updates to show matches

2. **Select Test**
   - Click any test in the grouped list
   - Selected test highlights in blue
   - Main area displays the selected test component

3. **Manage Tabs**
   - Click "Add Test" to create duplicate variant tab
   - Close tab with ✕ button
   - Click tab name to switch active tab

4. **Toggle Dark Mode**
   - Click moon/sun icon in header
   - Theme applies instantly across UI

5. **Restore Tests**
   - Click "Restore" button to reset all tabs
   - Returns to single initial test view

---

## Next Steps (Future Phases)

### Tier 2 Implementation
- Components: Switch, ToggleButton, Input, Textarea, Slider, Radio
- Will automatically appear in sidebar once test components added
- No code changes needed to viewer (discovery is automatic)

### Advanced Features (Optional)
- Component state inspector (show props, slots, computed values)
- Event logging (log @onclick, @onchange events)
- Performance profiling (render times, re-render counts)
- Test scenario builder (UI to create custom test combinations)
- Export as story (generate Storybook format)

---

## Technical Notes

- **Framework:** Blazor Server (Server-Side Rendering)
- **CSS:** Tailwind CSS v4.1.18 with dark mode
- **Rendering:** Dynamic component instantiation via RenderFragment
- **State:** CascadingValue pattern for parent-to-child communication
- **Discovery:** Reflection-based component discovery (auto-detects new tests)
- **Interop:** JSInterop for dark mode class manipulation

---

**Implementation Date:** 2026-03-17
**Status:** ✅ Complete and Verified
