using Microsoft.Extensions.DependencyInjection;
using ShadcnBlazor.Cli.Args;
using ShadcnBlazor.Cli.Services;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddSingleton<CsprojService>();
services.AddSingleton<FileSystemService>();
services.AddSingleton<ConfigService>();
services.AddSingleton<ProjectValidator>();
services.AddSingleton<ComponentService>();
services.AddSingleton<ProjectNamespaceService>();
services.AddSingleton<NamespaceService>();
services.AddSingleton<UsingService>();
  
var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(conf =>
{
    conf.AddCommand<AddCommand>("add");
    conf.AddCommand<InitCommand>("init");
});
return app.Run(args);

