using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages;

public partial class Home : ComponentBase
{
    private Tab _selectedTab = Tab.MarkdownEditor;
    
    private enum Tab
    {
        Examples,
        MarkdownEditor,
        AiChat,
    }
    
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    private static string TabToString(Tab tab) => tab switch
    {
        Tab.Examples => "Examples",
        Tab.MarkdownEditor => "Markdown Editor",
        Tab.AiChat => "AI Chat",
    };
    
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    private static Tab StringToTab(string tab) => tab switch
    {
        "Examples" => Tab.Examples,
        "Markdown Editor" => Tab.MarkdownEditor,
        "AI Chat" => Tab.AiChat,
    };
}