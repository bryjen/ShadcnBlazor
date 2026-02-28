# ShadcnBlazor Conventions

## Base Class

All components inherit from `ShadcnComponentBase`, which provides:
- `Class` parameter — additional CSS classes merged via TailwindMerge.NET `MergeCss()`
- `AdditionalAttributes` — splatted onto the root element (`@attributes="AdditionalAttributes"`)

```csharp
// Example base class usage pattern
protected string RootClass => MergeCss("base-classes", Class);
```

## Parameter Categories

Parameters are grouped into three categories:

| Category    | Description                                          | Examples                              |
|-------------|------------------------------------------------------|---------------------------------------|
| Content     | Child content or data to display                     | `ChildContent`, `Value`, `Placeholder` |
| Appearance  | Visual styling (variant, size, color)                | `Variant`, `Size`, `Class`            |
| Behavior    | Functional configuration and event callbacks         | `Disabled`, `OnClick`, `ValueChanged` |

## Enums

```csharp
// Variant — visual style
enum Variant { Default, Destructive, Outline, Secondary, Ghost, Link }

// Size
enum Size { Default, Sm, Lg, Icon }

// VerticalAlignment
enum VerticalAlignment { Top, Center, Bottom }
```

## Two-Way Binding

Components follow the standard Blazor `Value` + `ValueChanged` pattern for two-way binding:

```razor
<Checkbox @bind-Value="isChecked" />
<Switch @bind-Value="isOn" />
<Input @bind-Value="text" />
<Select @bind-Value="selectedItem" Items="options" />
<Slider @bind-Value="sliderValue" Min="0" Max="100" />
```

## Cascading Parameters

Some components receive context via cascading parameters:

- `PopoverTriggerContext` — passed from `Popover` to trigger elements
- `FormValidationContext` — passed from `FormValidationProvider` to form inputs (enables `aria-invalid`, `aria-errormessage`)

## Required Providers in Layout

Some components require a provider registered in `MainLayout.razor` or `App.razor`:

| Component(s)                           | Required Provider      |
|----------------------------------------|------------------------|
| Popover, DropdownMenu, Select, Combobox, MultiSelect, Tooltip | `<PopoverProvider />` |
| Dialog                                 | `<DialogProvider />`   |

Example layout:
```razor
@Body
<PopoverProvider />
<DialogProvider />
```

## CSS Utilities

- Uses **TailwindCSS** for styling
- Class merging via `TailwindMerge.NET`: conflicting utility classes are resolved correctly
- `MergeCss(string base, string? additional)` is the primary merge method

## Dialog Usage

Dialog is **imperative** — inject `IDialogService` and call `Show<TComponent>()`:

```csharp
@inject IDialogService DialogService

var result = await DialogService.Show<MyDialog>("Title", parameters).Result;
```

For **declarative** usage, use the `DialogRoot` / `DialogTrigger` / `DialogContent` components.

## Component Installation

Components define their own dependencies and `RequiredActions` (NuGet packages, JS files, CSS files, DI registrations). The CLI (`shadcn-blazor add <component>`) handles installation automatically.
