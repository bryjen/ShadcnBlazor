using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog;

namespace ShadcnBlazor.Docs.Pages.Components.Dialog.Examples.SeparateFilesAbstraction;

public abstract class AbstractDialog<T> : ComponentBase where T : class, new()
{
    protected DialogRoot Dialog { get; set; } = default!;
    protected T Model = new();
    protected TaskCompletionSource<T?> Tcs = new();

    public Task<T?> Show()
    {
        // don't forget to clear the state.
        // this can be done before the dialog is shown (here), or directly after it's closed
        Model = new();

        Dialog.OpenDialog();
        Tcs = new TaskCompletionSource<T?>();
        return Tcs.Task;
    }

    protected async Task Cancel()
    {
        Tcs.SetResult(null);
        await Dialog.CloseAsync();
    }

    protected async Task Submit()
    {
        Tcs.SetResult(Model);
        await Dialog.CloseAsync();
    }

    protected Task OnAutoClose()
    {
        Tcs.SetResult(null);
        return Task.CompletedTask;
    }
}