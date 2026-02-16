using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Cli;

public enum ConfirmationResponse
{
    Yes,
    No
};

[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
public static class PromptUtils
{
    public static ConfirmationResponse PromptUser(string prompt)
    {
        var promptObject = new SelectionPrompt<ConfirmationResponse>()
            .Title(prompt)
            .HighlightStyle(new Style(Color.SkyBlue1))
            .AddChoices(ConfirmationResponse.Yes, ConfirmationResponse.No);
        return AnsiConsole.Prompt(promptObject);
    }

    public static ConfirmationResponse PromptFirstTimeSetup()
    {
        AnsiConsole.MarkupLine("[yellow]First-time setup required.[/]");
        AnsiConsole.MarkupLine("The following will be added or modified:");
        AnsiConsole.MarkupLine("  • [yellow]Shared/[/] - enums, services, and shared code");
        AnsiConsole.MarkupLine("  • [yellow]wwwroot/css/[/] and [yellow]wwwroot/js/[/] - component assets");
        AnsiConsole.MarkupLine("  • [yellow]_Imports.razor[/] - component usings");
        AnsiConsole.MarkupLine("  • [yellow]Program.cs[/] - AddTailwindMerge registration");
        AnsiConsole.MarkupLine("  • [yellow].csproj[/] - TailwindMerge.NET package");
        AnsiConsole.MarkupLine("  • [yellow]index.html[/] or [yellow]App.razor[/] - CSS/JS references");
        return PromptUser("Continue? [Y/n]");
    }

    public static ConfirmationResponse PromptRepairSetup()
    {
        AnsiConsole.MarkupLine("[yellow]Setup appears incomplete.[/]");
        AnsiConsole.MarkupLine("(e.g., Shared exists but Program.cs is missing AddTailwindMerge)");
        return PromptUser("Run repair to fix? [Y/n]");
    }

    public static ConfirmationResponse PromptOverwriteComponent(string componentName)
    {
        return PromptUser($"Component `[yellow]{componentName}[/]` already exists. Overwrite? [Y/n]");
    }
    
    [Obsolete]
    public static void PromptUser(string prompt, Action onYes, Action onNo)
    {
        var promptObject = new SelectionPrompt<ConfirmationResponse>()
            .Title("The file `_Imports.razor already exists, do you want to override it?`?")
            .HighlightStyle(new Style(Color.SkyBlue1))
            .AddChoices(ConfirmationResponse.Yes, ConfirmationResponse.No);
        switch (AnsiConsole.Prompt(promptObject))
        {
            case ConfirmationResponse.Yes:
                onYes();
                break;
            case ConfirmationResponse.No:
                onNo();
                break;
        }
    }
    
    [Obsolete]
    public static T PromptUser<T>(string prompt, Func<T> onYes, Func<T> onNo)
    {
        var promptObject = new SelectionPrompt<ConfirmationResponse>()
            .Title("The file `_Imports.razor already exists, do you want to override it?`?")
            .HighlightStyle(new Style(Color.SkyBlue1))
            .AddChoices(ConfirmationResponse.Yes, ConfirmationResponse.No);
        var response = AnsiConsole.Prompt(promptObject);
        return response switch
        {
            ConfirmationResponse.Yes => onYes(),
            ConfirmationResponse.No => onNo(),
        };
    }
}