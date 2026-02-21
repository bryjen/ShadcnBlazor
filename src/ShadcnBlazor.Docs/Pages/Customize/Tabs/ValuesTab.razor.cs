using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Select;
using ShadcnBlazor.Docs.Models.Theme;
using ShadcnBlazor.Docs.Services;
using ShadcnBlazor.Docs.Services.Theme;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class ValuesTab : ComponentBase, IDisposable
{
    [Inject]
    public required ThemeService ThemeService { get; set; }
    
    protected override void OnInitialized()
    {
        // ValuesTabColors.cs
        BuildSectionsFromTheme();
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _loadingPresets = true;
            StateHasChanged();
            
            await ThemeService.EnsureExternalThemesLoadedAsync();
            
            _loadingPresets = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        // ValuesTabColors.cs
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    private bool _loadingPresets = false;
    private ThemePreset? _selectedThemePreset = null;

    private IReadOnlyList<SelectOption<ThemePreset>> GetThemeOptions() => 
        ThemeService.Presets.Select(preset => new SelectOption<ThemePreset>(preset, preset.Name)).ToList();
    
    private async Task OnThemePresetChanged(ThemePreset? option)
    {
        _selectedThemePreset = option;
        if (_selectedThemePreset is null)
        {
            return;
        }

        await ThemeService.ApplyPresetAsync(_selectedThemePreset);
    }
}
