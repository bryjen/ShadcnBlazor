namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

public abstract class ChatMessage(string rawContent)
{
    public string GetRawContent() => RawContent;
    protected string RawContent = rawContent;
}

public class UserChatMessage(string rawContent) : ChatMessage(rawContent);

public class AiChatMessage(string rawContent) : ChatMessage(rawContent)
{
    public List<MessageComponent> Components { get; set; } = new();
    
    public void SetContent(string value)
    {
        RawContent = value ?? "";
    }

    public void AppendContent(string chunk)
    {
        if (!string.IsNullOrEmpty(chunk))
            RawContent += chunk;
    }

    public void Clear()
    {
        RawContent = "";
    }
}