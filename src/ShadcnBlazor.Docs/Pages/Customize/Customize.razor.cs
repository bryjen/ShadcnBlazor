using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages.Customize;

public partial class Customize : ComponentBase
{
    private Tab _selectedTab = Tab.Code;

    private enum Tab
    {
        Code,
        Values,
        Presets,
    }

    private string CurrentPrimaryValue => ThemeService.CurrentTheme.Primary;
}
