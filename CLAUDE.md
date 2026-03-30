# Razor Component Binding Rules

## Always use @ for variable binding in attributes

**Rule:** When binding to variables or expressions in Razor component attributes, ALWAYS use the `@` prefix.

**Pattern:**
```razor
<!-- ✗ WRONG - treated as string literal -->
<Component Property="variableName" />

<!-- ✓ CORRECT - binds to the variable -->
<Component Property="@variableName" />
```

**Why:** Without `@`, Blazor treats the value as a string literal, not a variable reference. This breaks two-way binding and parameter updates.

**Examples:**
```razor
<!-- ✗ Wrong -->
<DropdownMenuRadioGroup SelectedValue="fontSize" />

<!-- ✓ Correct -->
<DropdownMenuRadioGroup SelectedValue="@fontSize" />

<!-- ✓ Also correct (explicit string) -->
<DropdownMenuRadioGroup SelectedValue="@nameof(fontSize)" />
```

This applies to ALL variable bindings in components:
- `Property="@variable"`
- `Property="@GetValue()"`
- `Property="@(expression)"`
- Event handlers with expressions: `@onclick="@(e => HandleClick(variable))"`
