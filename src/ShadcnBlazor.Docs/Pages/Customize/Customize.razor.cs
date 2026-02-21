using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog.Models;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Docs.Pages.Customize.Tabs;

namespace ShadcnBlazor.Docs.Pages.Customize;

public partial class Customize : ComponentBase
{
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    private Tab _selectedTab = Tab.Values;

    private CodeTab _codeTab = null!;

    private enum Tab
    {
        Code,
        Values,
    }

    private string CurrentPrimaryValue => ThemeService.CurrentTheme.Dark.Primary;
    

    private void OpenDialog()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions
        {
            NoHeader = true
        };
        DialogService.Show<CodeTabInfoDialog>(string.Empty, parameters, options);
    }
}

