using Microsoft.JSInterop;

namespace SampleWasmProject.Components.Core.Dialog.Declarative;

public sealed class DeclarativeDialogContext
{
    public bool Open { get; set; }
    public bool IsClosing { get; set; }
    public string AnimationState { get; set; } = "closed";
    public string DialogId { get; set; } = "";
    public required Func<Task> CloseAsync { get; init; }
    public required Action OpenDialog { get; init; }
    public required Action<string> SetAnimationState { get; init; }
    public DotNetObjectReference<DialogRoot>? DotNetRef { get; set; }
}
