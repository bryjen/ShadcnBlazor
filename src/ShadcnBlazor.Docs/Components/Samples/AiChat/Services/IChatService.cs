namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public interface IChatService
{
    IAsyncEnumerable<string> RunStreamingAsync(string prompt, CancellationToken cancellationToken = default);
}
