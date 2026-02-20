namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Named preset theme with display swatches and full token values.
/// </summary>
/// <param name="Name">Preset display name.</param>
/// <param name="Swatches">Preview swatches shown in the UI.</param>
/// <param name="Theme">Theme token values for the preset.</param>
public sealed record ThemePreset(string Name, string[] Swatches, ThemeState Theme);
