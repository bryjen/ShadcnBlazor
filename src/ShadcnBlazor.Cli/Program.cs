using Microsoft.Extensions.DependencyInjection;
using ShadcnBlazor.Cli.Args;
using Spectre.Console;
using Spectre.Console.Cli;


var services = new ServiceCollection();
services.AddSingleton<GreetingService>();
  
var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(conf =>
{
    conf.AddCommand<GreetCommand>("greet");
    conf.AddCommand<InitCommand>("init");
});
return app.Run(args);

  
public class GreetingService
{
    public string GetGreeting(string name, bool formal)
    {
        return formal
            ? $"Good day, {name}."
            : $"Hello, {name}!";
    }
}

