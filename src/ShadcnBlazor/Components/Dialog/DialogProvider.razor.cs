using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog.Models;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Provider component that hosts dialogs shown via <see cref="IDialogService"/>. Required for imperative dialogs to work.
/// </summary>
[ComponentMetadata(Name = "Dialog", Description = "Imperative dialog service; show dialogs via IDialogService.Show. Requires DialogProvider in layout.", Dependencies = [])]
public partial class DialogProvider : ComponentBase, IDisposable
{
}
