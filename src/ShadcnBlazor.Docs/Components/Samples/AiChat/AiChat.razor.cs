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

    private async Task OnChatStateChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void SubmitPrompt(string prompt)
    {
        _ = ChatOrchestrator.SendPromptAsync(prompt);
    }
}