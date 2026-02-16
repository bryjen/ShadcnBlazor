using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog.Models;
using ShadcnBlazor.Components.Dialog.Services;

namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Provider component that hosts dialogs shown via <see cref="IDialogService"/>. Required for imperative dialogs to work.
/// </summary>
public partial class DialogProvider : ComponentBase, IDisposable
{
}
