using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Dialog.Models;
using SampleWasmProject.Components.Core.Dialog.Services;

namespace SampleWasmProject.Components.Core.Dialog;

/// <summary>
/// Provider component that hosts dialogs shown via <see cref="IDialogService"/>. Required for imperative dialogs to work.
/// </summary>
public partial class DialogProvider : ComponentBase, IDisposable
{
}
