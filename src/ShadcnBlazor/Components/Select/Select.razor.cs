using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Select;

/// <summary>
/// Dropdown select component for choosing a single value from a list of options.
/// </summary>
[ComponentMetadata(Name = nameof(Select), Description = "Dropdown select for choosing a single value from a list of options; requires PopoverProvider in layout.", Dependencies = ["Popover"])]
public partial class Select<T>
{
}
