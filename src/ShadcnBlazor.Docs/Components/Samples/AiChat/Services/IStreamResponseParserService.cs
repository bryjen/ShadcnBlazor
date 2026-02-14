using ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public interface IStreamResponseParserService
{
    IStreamResponseParser CreateStream(IList<MessageComponent> components);
}
