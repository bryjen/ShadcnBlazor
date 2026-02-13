namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public static class AiChatServiceExtensions
{
    public static IServiceCollection AddAiChat(this IServiceCollection services)
    {
        services.AddScoped<ChatService>();
        services.AddScoped<ChatOrchestrator>();
        return services;
    }
}