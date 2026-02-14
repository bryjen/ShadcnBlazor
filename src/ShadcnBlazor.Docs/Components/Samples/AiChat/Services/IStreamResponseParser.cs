namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public interface IStreamResponseParser
{
    void AppendChunk(string chunk);
}
