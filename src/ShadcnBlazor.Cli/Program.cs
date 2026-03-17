using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ShadcnBlazor.Cli.Commands;
using ShadcnBlazor.Cli.Commands.Components;
using ShadcnBlazor.Cli.Services;
using ShadcnBlazor.Cli.Services.Actions;
using ShadcnBlazor.Cli.Services.Components;
using ShadcnBlazor.Cli.Utils;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddSingleton<CsprojService>();
services.AddSingleton<FileSystemService>();
services.AddSingleton<ProjectValidator>();
services.AddSingleton<ComponentService>();
services.AddSingleton<ProjectNamespaceService>();
services.AddSingleton<NamespaceService>();
services.AddSingleton<UsingService>();
services.AddSingleton<CopyCssActionService>();
services.AddSingleton<CopyJsActionService>();
services.AddSingleton<AddCssLinksToRootActionService>();
services.AddSingleton<AddNugetDependencyActionService>();
services.AddSingleton<AddProgramServiceActionService>();
services.AddSingleton<MergeToImportsActionService>();
services.AddSingleton<AddToServicesActionService>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(conf =>
{
    conf.AddCommand<NewCommand>("new")
        .WithDescription("Creates a new Blazor project from a template.");

    conf.AddCommand<RepairCommand>("repair")
        .WithDescription("Re-add Shared component with overwrite. Use if setup was broken or incomplete.");

    conf.AddBranch("component", branch =>
    {
        branch.SetDescription("Component management. Shared and its dependencies are added automatically.");
        branch.AddCommand<AddCommand>("add")
            .WithDescription("Adds a component to a project. Runs first-time setup automatically if needed.");
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
