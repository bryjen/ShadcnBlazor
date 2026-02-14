using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public class ChatOrchestrator(IChatService chatService, IStreamResponseParserService streamParserService)
{
    public bool LockUserInput { get; set; } = false;
    public ObservableCollection<ChatMessage> Messages { get; set; } = new();
    
    public event Func<Task> OnStateChange = null!;
    
    public async Task SendPromptAsync(string promptRaw, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(promptRaw);
        
        var userMsg = new UserChatMessage(promptRaw);
        LockUserInput = true;
        Messages.Add(userMsg);
        await OnStateChange.Invoke();

        var responseMsg = new AiChatMessage(string.Empty);
        var streamParser = streamParserService.CreateStream(responseMsg.Components);
        Messages.Add(responseMsg);
        
        await foreach (var update in chatService.RunStreamingAsync(promptRaw, cancellationToken))
        {
            responseMsg.AppendContent(update);
            streamParser.AppendChunk(update);
            await OnStateChange.Invoke();
        }
        
        LockUserInput = false;
        await OnStateChange.Invoke();
    }
}