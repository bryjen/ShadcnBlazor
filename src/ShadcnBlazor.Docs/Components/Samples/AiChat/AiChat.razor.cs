using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Docs.Components.Samples.AiChat.Services;
using ShadcnBlazor.Shared;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat;

public partial class AiChat : ShadcnComponentBase
{
    [Inject]
    public required ChatOrchestrator ChatOrchestrator { get; set; }
    
    protected override void OnInitialized()
    {
        ChatOrchestrator.OnStateChange += OnChatStateChange;
    }

    public void Dispose()
    {
        ChatOrchestrator.OnStateChange -= OnChatStateChange;
    }

    private void SendPrompt()
    {
        _ = ChatOrchestrator.SendPromptAsync("hello ai chat how are yo udoing");
    }

    private async Task OnChatStateChange()
    {
        await InvokeAsync(StateHasChanged);
    }
}