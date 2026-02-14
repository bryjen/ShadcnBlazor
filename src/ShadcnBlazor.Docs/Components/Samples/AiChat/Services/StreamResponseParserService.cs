using ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public class StreamResponseParserService : IStreamResponseParserService
{
    public IStreamResponseParser CreateStream(IList<MessageComponent> components) =>
        new StreamResponseParser(components);
}
