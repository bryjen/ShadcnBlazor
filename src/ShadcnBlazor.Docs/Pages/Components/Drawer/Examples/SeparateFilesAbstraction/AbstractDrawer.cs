using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Drawer;

namespace ShadcnBlazor.Docs.Pages.Components.Drawer.Examples.SeparateFilesAbstraction;

public abstract class AbstractDrawer<T> : ComponentBase where T : class, new()
{
    protected DrawerRoot Drawer { get; set; } = default!;
    protected T Model = new();
    protected TaskCompletionSource<T?> Tcs = new();

    public Task<T?> Show()
    {
        // Reset model state
        Model = new();

        Drawer.OpenAsync();
        Tcs = new TaskCompletionSource<T?>();
        return Tcs.Task;
    }

    protected async Task Cancel()
    {
        Tcs.SetResult(null);
        await Drawer.CloseAsync();
    }

    protected async Task Submit()
    {
        Tcs.SetResult(Model);
        await Drawer.CloseAsync();
    }

    protected Task OnAutoClose()
    {
        Tcs.SetResult(null);
        return Task.CompletedTask;
    }
}
