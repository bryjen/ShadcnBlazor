# ShadcnBlazor Styling & Component Architecture Guide

This guide defines the "Gold Standard" for generating components for the `ShadcnBlazor` library. All AI-generated components MUST follow these patterns to ensure consistency, accessibility, and high aesthetic quality.

## 1. Core Principles
- **Tokens Only**: Never use hardcoded hex codes or default Tailwind colors (e.g., `bg-blue-500`). Use Shadcn tokens: `bg-background`, `text-foreground`, `border-input`, `bg-primary`, etc.
- **Separation of Concerns**: Keep styling logic in a static `*Styles` class and behavior/parameters in the `*.razor.cs` file.
- **Accessibility First**: Use ARIA attributes (`aria-label`, `aria-busy`, `aria-expanded`) and ensure they are merged correctly with `AdditionalAttributes`.
- **Premium Feel**: Use subtle transitions (`duration-200`), micros-interactions (`active:scale-95`), and OKLCH-based colors for depth and contrast.

---

## 2. Component Structure (The "Gold Standard")

### A. The Razor File (`Component.razor`)
Focus on semantic HTML, `data-*` attributes for styling hooks, and class/attribute binding.

```razor
@inherits ShadcnComponentBase

<button
    data-slot="button"
    data-variant="@(Variant.ToString().ToLower())"
    data-size="@(Size.ToString().ToLower())"
    class="@(GetClass())"
    disabled="@IsDisabled"
    @attributes="@(GetAttributes())">
    @ChildContent
</button>
```

### B. The Code-Behind (`Component.razor.cs`)
Handle parameters, state management, and attribute merging.

```csharp
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.MyComponent;

public partial class MyComponent : ShadcnComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Variant Variant { get; set; } = Variant.Default;
    [Parameter] public Size Size { get; set; } = Size.Md;
    [Parameter] public bool IsLoading { get; set; }

    private bool IsDisabled => IsLoading;

    private string GetClass()
    {
        // Use the Styles helper to build the class string
        return MyComponentStyles.Build(base.MergeCss, Variant, Size, IsLoading, Class);
    }

    private IReadOnlyDictionary<string, object>? GetAttributes()
    {
        var merged = new Dictionary<string, object>(StringComparer.Ordinal);
        
        if (IsLoading) merged["aria-busy"] = true;

        if (AdditionalAttributes is not null)
            foreach (var kv in AdditionalAttributes)
                merged[kv.Key] = kv.Value;

        return merged;
    }
}
```

### C. The Styling Helper (`ComponentShared.cs`)
Centralize all Tailwind logic. Use `MergeCss` (passed as a callback) to handle final merging.

```csharp
namespace ShadcnBlazor.Components.MyComponent;

internal static class MyComponentStyles
{
    public static string Build(Func<string[], string> mergeCallback, Variant variant, Size size, bool isLoading, string @class)
    {
        var baseClasses = "inline-flex items-center justify-center gap-2 rounded-md transition-all duration-200";
        
        var variantClasses = variant switch
        {
            Variant.Default => "bg-primary text-primary-foreground hover:bg-primary/90",
            Variant.Outline => "border border-input bg-background hover:bg-accent",
            _ => ""
        };

        var sizeClasses = size switch
        {
            Size.Sm => "h-8 px-3 text-xs",
            Size.Md => "h-9 px-4 text-sm",
            _ => ""
        };

        return mergeCallback([baseClasses, variantClasses, sizeClasses, isLoading ? "opacity-50" : "", @class]);
    }
}
```

---

## 3. Styling Tokens & Variables

### Common Tokens
| Token | Usage |
|-------|-------|
| `bg-background` | Main page/component background |
| `text-foreground` | Main text color |
| `border-input` | Standard border for inputs/buttons |
| `bg-primary` | Primary action background |
| `text-primary-foreground` | Text on primary background |
| `bg-destructive` | Danger/Warning actions |
| `ring-ring` | Focus ring color |
| `rounded-md` | Standard corner radius |

### OKLCH Colors
The theme uses OKLCH for better perceptual uniformity. Avoid hardcoded values; these are mapped to Tailwind classes via `@theme inline`.

---

## 4. Interaction Patterns
- **Hover/Active**: Always provide `hover:` and `active:scale-95` (for buttons) to provide tactile feedback.
- **Focus**: Use `focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]`.
- **Disabled**: Use `disabled:pointer-events-none disabled:opacity-50`.
- **Loading**: Use `cursor-wait` and `aria-busy`.

## 5. Directory Convention
- `Components/[ComponentName]/[ComponentName].razor`
- `Components/[ComponentName]/[ComponentName].razor.cs`
- `Components/[ComponentName]/[ComponentName]Shared.cs` (for styles/enums if needed)
