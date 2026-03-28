using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog;
using ShadcnBlazor.Docs.Pages.Customize.Tabs;

namespace ShadcnBlazor.Docs.Pages.Customize;

public partial class Customize : ComponentBase
{
    private DialogRoot? dialog;
    private Tab _selectedTab = Tab.Values;

    private CodeTab _codeTab = null!;

    private enum Tab
    {
        Code,
        Values,
    }

    private string CurrentPrimaryValue => ThemeService.CurrentTheme.Dark.Primary;
}

