using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShadcnBlazor.Mcp.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSingleton<ComponentRegistryService>()
    .AddSingleton<ApiDocumentationService>()
    .AddSingleton<SnippetExampleService>()
    .AddSingleton<ThemeTokenReaderService>()
    .AddSingleton<DocumentationReaderService>()
    .AddSingleton<AccessibilitySpecReaderService>()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
