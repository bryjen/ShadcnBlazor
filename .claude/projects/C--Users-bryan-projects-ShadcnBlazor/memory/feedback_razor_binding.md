---
name: Razor binding syntax - always use @
description: Must always use @ prefix when binding variables in Razor component attributes
type: feedback
---

**Rule:** In Razor components, ALWAYS use `@` prefix when binding to variables in attributes.

**Why:** Without `@`, Blazer treats the value as a string literal, not a variable reference. This breaks all two-way binding and parameter updates. I made this mistake in the radio group examples by writing `SelectedValue="fontSize"` instead of `SelectedValue="@fontSize"`, which caused the checkmark bug.

**How to apply:** Every time writing or reviewing Razor component attributes with variable bindings:
- `Property="@variable"` ✓
- `Property="variable"` ✗ (unless you actually want the string literal "variable")
- Event callbacks: `@onclick="@(e => Method(variable))"` ✓
- Never forget the @ in two-way binding: `SelectedValue="@myValue"` not `SelectedValue="myValue"`

This is a Razor gotcha - the IDE won't warn you, so it requires discipline.
