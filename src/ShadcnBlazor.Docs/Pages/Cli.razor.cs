using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Docs.Components.Docs.CodeBlock;

namespace ShadcnBlazor.Docs.Pages;

public partial class Cli : ComponentBase
{
    private static readonly IReadOnlyDictionary<string, string> _cliRootCommands = new Dictionary<string, string>
    {
        ["shell"] = "shadcnblazor [command]"
    };

    private static readonly IReadOnlyDictionary<string, string> _cliNewCommands = new Dictionary<string, string>
    {
        ["shell"] = "shadcnblazor new --wasm --proj MyApp"
    };

    private static readonly IReadOnlyDictionary<string, string> _cliRepairCommands = new Dictionary<string, string>
    {
        ["shell"] = "shadcnblazor repair"
    };

    private static readonly IReadOnlyDictionary<string, string> _cliComponentCommands = new Dictionary<string, string>
    {
        ["single"] = "shadcnblazor component add [component]",
        ["all"] = "shadcnblazor component add --all"
    };

    private static readonly IReadOnlyDictionary<string, string> _cliAddCommands = new Dictionary<string, string>
    {
        ["single"] = "shadcnblazor component add [component]",
        ["all"] = "shadcnblazor component add --all [--overwrite] [--silent]"
    };

    private CodeFile _sampleJson = new()
    {
        Language = "json",
        Contents =
            """
            [
                {
                    "name": "Shared",
                    "description": "Base classes, enums, services, and utilities required by all components",
                    "dependencies": [],
                    "required_actions": [
                        {
                            "$type": "copyCss",
                            "css_file_name": "shadcn_blazor_in.css",
                            "link": false
                        },
                        {
                            "$type": "copyCss",
                            "css_file_name": "shadcn_blazor_out.css",
                            "link": false
                        },
                        {
                            "$type": "addCssLinksToRoot"
                        },
                        {
                            "$type": "addNugetDependency",
                            "package_name": "TailwindMerge.NET",
                            "version": "1.2.0"
                        },
                        {
                            "$type": "addProgramService",
                            "using_namespace": "TailwindMerge.Extensions",
                            "service_call": "AddTailwindMerge()"
                        },
                        {
                            "$type": "mergeToImports",
                            "namespaces": ["ShadcnBlazor.Components.Shared"]
                        }
                    ]
                }
            ]
            """
    };
    
    private CodeFile _sampleDependencyTree = new()
    {
        Language = "text",
        Contents =
            """
            Component Dependency Tree:      (2 dependencies)
            Select
            ├── Popover
            │   └── Shared
            └── Shared
            """
    };
    
    private CodeFile _cardFileStructure = new()
    {
        Language = "text",
        Contents =
            """
            Card/
            ├── Card.razor
            └── Card.razor.cs
            """
    };
    
    private CodeFile _dialogFileStructure = new()
    {
        Language = "text",
        Contents =
            """
            Dialog/
            ├── Declarative/
            │   ├── DialogRoot.razor
            │   ├── DialogContent.razor
            │   ├── DialogTrigger.razor
            │   └── ... (5 more files)
            ├── Models/
            │   ├── DialogOptions.cs
            │   ├── DialogResult.cs
            │   └── ... (4 more files)
            ├── Services/
            │   ├── DialogService.cs
            │   └── IDialogService.cs
            ├── Dialog.razor
            ├── DialogProvider.razor
            ├── DialogReference.cs
            └── ... (3 more files)
            """
    };
    
    private CodeFile _sharedFileStructure = new()
    {
        Language = "text",
        Contents =
            """
            Shared/
            ├── Models/
            │   ├── Enums/
            │   │   ├── Size.cs
            │   │   ├── Variant.cs
            │   │   └── ... (2 more files)
            │   └── Options/
            │       └── ... (2 more files)
            ├── Services/
            │   └── ... (service files)
            ├── Interop/
            │   └── ... (interop files)
            └── ShadcnComponentBase.cs
            """
    };
    
    private CodeFile _cliRoot = new()
    {
        Language = "text",
        Contents =
            """
            USAGE:
                ShadcnBlazor.Cli.dll [OPTIONS] <COMMAND>

            OPTIONS:
                -h, --help    Display help

            COMMANDS:
                new          Creates a new Blazor project from a template
                repair       Re-add Shared component with overwrite. Use if setup was broken or incomplete
                component    Component management. Shared and its dependencies are added automatically
            """
    };

    private CodeFile _cliNew = new()
    {
        Language = "text",
        Contents =
            """
            DESCRIPTION:
            Creates a new Blazor project from a template

            USAGE:
                ShadcnBlazor.Cli.dll new [OPTIONS]

            OPTIONS:
                                    DEFAULT
                -h, --help                        Display help
                --wasm                            Use the WebAssembly standalone template
                --server                          Use the Blazor Server (global interactivity) template
                -p, --proj <NAME>                 Project name, root namespace, and default output folder
                -o, --out <DIR>                   Output directory. Default: ./{proj}
                --net <VERSION>         9         Target .NET version (9 only for now)
            """
    };

    private CodeFile _cliRepair = new()
    {
        Language = "text",
        Contents =
            """
            DESCRIPTION:
            Re-add Shared component with overwrite. Use if setup was broken or incomplete

            USAGE:
                ShadcnBlazor.Cli.dll repair [OPTIONS]

            OPTIONS:
                            DEFAULT
                -h, --help                Display help
                --silent    false         Mutes output
            """
    };

    private CodeFile _cliComponent = new()
    {
        Language = "text",
        Contents =
            """
            DESCRIPTION:
            Component management. Shared and its dependencies are added automatically

            USAGE:
                ShadcnBlazor.Cli.dll component [OPTIONS] <COMMAND>

            OPTIONS:
                -h, --help    Display help

            COMMANDS:
                add [name]     Adds a component to a project. Use --all to add all. Runs first-time setup automatically if needed
                list           Lists all available components
                info <name>    Displays details related to a component
            """
    };

    private CodeFile _cliAdd = new()
    {
        Language = "text",
        Contents =
            """
            DESCRIPTION:
            Adds a component to a project. Runs first-time setup automatically if needed

            USAGE:
                ShadcnBlazor.Cli.dll component add [OPTIONS] [name]

            ARGUMENTS:
                name    The name of the component to add (input, textarea, checkbox, etc.). Omit when using --all

            OPTIONS:
                            DEFAULT
                -h, --help                Display help
                -a, --all    false        Adds all available components (name not required)
                --silent    false         Mutes output
                --overwrite  false        Overwrite if component already exists (no prompt)
            """
    };
}