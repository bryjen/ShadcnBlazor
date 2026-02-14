namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public static class AiChatServiceExtensions
{
    public static IServiceCollection AddAiChat(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IStreamResponseParserService, StreamResponseParserService>();
        services.AddScoped<ChatOrchestrator>();
        return services;
    }
}