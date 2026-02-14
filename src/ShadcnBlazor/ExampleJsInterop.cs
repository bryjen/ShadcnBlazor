using Microsoft.JSInterop;

namespace ShadcnBlazor;

/// <summary>
/// Example wrapper for JavaScript functionality. The associated JavaScript module is
/// loaded on demand when first needed. Can be registered as a scoped DI service.
/// </summary>
public class ExampleJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    /// <summary>
    /// Creates a new ExampleJsInterop instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public ExampleJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/ShadcnBlazor/exampleJsInterop.js").AsTask());
    }

    /// <summary>
    /// Shows a browser prompt and returns the user's input.
    /// </summary>
    /// <param name="message">The prompt message to display.</param>
    /// <returns>The user's input string.</returns>
    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}