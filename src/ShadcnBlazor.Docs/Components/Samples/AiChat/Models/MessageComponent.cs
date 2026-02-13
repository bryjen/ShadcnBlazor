namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

public abstract class MessageComponent;

public class ThinkingMessageComponent : MessageComponent
{
    public required string Content { get; set; }
    public int? ThinkingTime { get; set; }
}

public class TextMessageComponent : MessageComponent
{
    public required string Content { get; set; }
}

public class WebSearchMessageComponent : MessageComponent
{
    public required string Content { get; set; }
}
