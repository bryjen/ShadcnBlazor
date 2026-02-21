namespace ShadcnBlazor.Components.Select;

/// <summary>
/// Represents a single option in a Select component.
/// </summary>
/// <param name="Value">The value of the option.</param>
/// <param name="DisplayText">The text displayed to the user.</param>
/// <param name="Disabled">Whether the option is disabled and cannot be selected.</param>
public readonly record struct SelectOption<T>(T Value, string DisplayText, bool Disabled = false);
