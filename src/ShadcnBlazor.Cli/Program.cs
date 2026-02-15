using Microsoft.Extensions.DependencyInjection;
using ShadcnBlazor.Cli.Commands;
using ShadcnBlazor.Cli.Commands.Components;
using ShadcnBlazor.Cli.Services;
using ShadcnBlazor.Cli.Utils;
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
    conf.AddCommand<InitCommand>("init")
        .WithDescription("Initializes [yellow]ShadcnBlazor[/] in an existing blazor project");

    conf.AddCommand<NewCommand>("new")
        .WithDescription("Creates a new Blazor project from a template.");

    conf.AddBranch("component", branch =>
    {
        branch.SetDescription("Component management, includes commands such as [yellow]add[/] and [yellow]list[/].");
        branch.AddCommand<AddCommand>("add")
            .WithDescription("Adds a component to a project.");
        branch.AddCommand<ListCommand>("list")
            .WithDescription("Lists all available components.");
        branch.AddCommand<InfoCommand>("info")
            .WithDescription("Displays details related to a component.");
    });
});

return app.Run(args);

/*
Todo:
- change to darkmode by adding dark to html
- remove all backing css files and stuff like that idk (maybe)
 */
