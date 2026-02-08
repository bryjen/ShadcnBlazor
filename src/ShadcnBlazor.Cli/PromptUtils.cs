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