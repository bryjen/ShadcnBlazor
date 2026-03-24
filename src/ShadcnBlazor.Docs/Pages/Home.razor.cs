using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages;

public partial class Home : ComponentBase
{
    private Tab _selectedTab = Tab.V01;
    
    private enum Tab
    {
        V01,
        AiChat,
    }
    
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    private static string TabToString(Tab tab) => tab switch
    {
        Tab.V01 => "v0.1",
        Tab.AiChat => "AI Chat",
    };
    
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    private static Tab StringToTab(string tab) => tab switch
    {
        "v0.1" => Tab.V01,
        "AI Chat" => Tab.AiChat,
    };
}