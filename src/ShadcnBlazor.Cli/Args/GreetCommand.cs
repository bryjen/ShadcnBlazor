using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Args;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal class GreetCommand(
    GreetingService greetingService, 
    IAnsiConsole console) : Command<GreetCommand.GreetSettings>
{
    public class GreetSettings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("The name to greet")]
        public string Name { get; init; } = string.Empty;
  
        [CommandOption("-f|--formal")]
        [Description("Use formal greeting")]
        [DefaultValue(false)]
        public bool Formal { get; init; }
    }

    public override int Execute(CommandContext context, GreetSettings settings, CancellationToken cancellation)
    {
        var greeting = greetingService.GetGreeting(settings.Name, settings.Formal);
        console.WriteLine(greeting);
        return 0;
    }
}
