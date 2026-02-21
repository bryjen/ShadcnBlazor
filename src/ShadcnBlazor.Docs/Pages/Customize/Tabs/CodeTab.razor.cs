using System.Text.RegularExpressions;
using GaelJ.BlazorCodeMirror6;
using GaelJ.BlazorCodeMirror6.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Docs.Services;
using ShadcnBlazor.Docs.Services.Theme;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class CodeTab : ComponentBase, IAsyncDisposable
{
    [Inject]
    public required IJSRuntime JsRuntime { get; set; }

    [Inject]
    public required ThemeService ThemeService { get; set; }

    private const double EditorLineHeightPx = 19.8;

    private CodeMirror6Wrapper? _editorRef;
    private ElementReference _editorHost;
    private DotNetObjectReference<CodeTab>? _selfRef;
    private long? _scrollObserverId;

    private double _editorScrollTop;
    private double _editorViewportHeight = 400;
    private List<ColorLineMarker> _lineMarkers = [];

    public async Task Clear()
    {
        _editorText = string.Empty;
        RebuildLineMarkers(string.Empty);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnEditorChanged(string value)
    {
        _editorText = value;
        RebuildLineMarkers(value);

        var parsedTheme = ThemeStateFullConverter.FromStyleSheet(value, ThemeService.CurrentTheme);
        await ThemeService.SaveThemeAsync(parsedTheme);

        await InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        var initialCss = ThemeService.RuntimeStyleSheet;
        if (!string.IsNullOrWhiteSpace(initialCss))
        {
            _editorText = initialCss;
        }

        RebuildLineMarkers(_editorText);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _selfRef = DotNetObjectReference.Create(this);
        _scrollObserverId = await JsRuntime.InvokeAsync<long?>("shadcnCustomizeEditor.observeScroll", _editorHost, _selfRef);
    }

    [JSInvokable]
    public Task OnEditorScroll(double scrollTop, double viewportHeight)
    {
        _editorScrollTop = scrollTop;
        _editorViewportHeight = viewportHeight;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_scrollObserverId is not null)
        {
            await JsRuntime.InvokeVoidAsync("shadcnCustomizeEditor.unobserveScroll", _scrollObserverId.Value);
        }

        _selfRef?.Dispose();
    }

    private IEnumerable<ColorLineMarker> VisibleMarkers
    {
        get
        {
            var minY = -16d;
            var maxY = _editorViewportHeight + 16d;

            return _lineMarkers.Where(marker =>
            {
                var top = GetMarkerTopPx(marker.LineNumber);
                return top >= minY && top <= maxY;
            });
        }
    }

    private double GetMarkerTopPx(int lineNumber)
        => (lineNumber - 1) * EditorLineHeightPx - _editorScrollTop + 2;

    private void RebuildLineMarkers(string text)
    {
        _lineMarkers = text
            .Split('\n')
            .Select((line, index) =>
            {
                var match = ColorTokenRegex.Match(line);
                return match.Success
                    ? new ColorLineMarker(index + 1, match.Value.Trim())
                    : null;
            })
            .Where(marker => marker is not null)
            .Select(marker => marker!)
            .ToList();
    }

    private readonly CodeMirrorSetup _editorSetup = new()
    {
        ScrollToStart = true,
        FocusOnCreation = true,
        FoldGutter = false,
    };

    private readonly List<SelectionRange> _topSelection =
    [
        new SelectionRange { From = 0, To = 0 }
    ];
    private static readonly Regex ColorTokenRegex = new(
        """#(?:[0-9a-fA-F]{3,8})\b|(?:oklch|oklab|lch|lab|hsl|hsla|rgb|rgba|hwb|color)\([^\n\r;{}]+\)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public record ColorLineMarker(int LineNumber, string Token);

    private string _editorText =
"""
:root {
  --background: oklch(0 0 0);
  --foreground: oklch(0.8686 0.2776 144.4661);
  --card: oklch(0.1149 0 0);
  --card-foreground: oklch(0.8686 0.2776 144.4661);
  --popover: oklch(0 0 0);
  --popover-foreground: oklch(0.8686 0.2776 144.4661);
  --primary: oklch(0.8686 0.2776 144.4661);
  --primary-foreground: oklch(0 0 0);
  --secondary: oklch(0.3053 0.1039 142.4953);
  --secondary-foreground: oklch(0.8686 0.2776 144.4661);
  --muted: oklch(0.1887 0.0642 142.4953);
  --muted-foreground: oklch(0.5638 0.1872 143.2450);
  --accent: oklch(0.8686 0.2776 144.4661);
  --accent-foreground: oklch(0 0 0);
  --destructive: oklch(0.6280 0.2577 29.2339);
  --destructive-foreground: oklch(1.0000 0 0);
  --border: oklch(0.3053 0.1039 142.4953);
  --input: oklch(0 0 0);
  --ring: oklch(0.8686 0.2776 144.4661);
  --chart-1: oklch(0.8686 0.2776 144.4661);
  --chart-2: oklch(0.5638 0.1872 143.2450);
  --chart-3: oklch(0.3053 0.1039 142.4953);
  --chart-4: oklch(0.1179 0.0327 343.3438);
  --chart-5: oklch(0.8686 0.2776 144.4661);
  --sidebar: oklch(0.1149 0 0);
  --sidebar-foreground: oklch(0.8686 0.2776 144.4661);
  --sidebar-primary: oklch(0.8686 0.2776 144.4661);
  --sidebar-primary-foreground: oklch(0 0 0);
  --sidebar-accent: oklch(0.3053 0.1039 142.4953);
  --sidebar-accent-foreground: oklch(0.8686 0.2776 144.4661);
  --sidebar-border: oklch(0.3053 0.1039 142.4953);
  --sidebar-ring: oklch(0.8686 0.2776 144.4661);
  --font-sans: "VT323", "Courier New", monospace;
  --font-serif: Georgia, serif;
  --font-mono: "VT323", monospace;
  --radius: 0rem;
  --shadow-x: 0px;
  --shadow-y: 0px;
  --shadow-blur: 10px;
  --shadow-spread: 1px;
  --shadow-opacity: 0.2;
  --shadow-color: #00FF41;
  --shadow-2xs: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.10);
  --shadow-xs: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.10);
  --shadow-sm: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.20), 0px 1px 2px 0px hsl(135.2941 100% 50% / 0.20);
  --shadow: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.20), 0px 1px 2px 0px hsl(135.2941 100% 50% / 0.20);
  --shadow-md: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.20), 0px 2px 4px 0px hsl(135.2941 100% 50% / 0.20);
  --shadow-lg: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.20), 0px 4px 6px 0px hsl(135.2941 100% 50% / 0.20);
  --shadow-xl: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.20), 0px 8px 10px 0px hsl(135.2941 100% 50% / 0.20);
  --shadow-2xl: 0px 0px 10px 1px hsl(135.2941 100% 50% / 0.50);
  --tracking-normal: 0.1em;
  --spacing: 0.25rem;
}

.dark {
  --background: oklch(0 0 0);
  --foreground: oklch(0.8686 0.2776 144.4661);
  --card: oklch(0.1149 0 0);
  --card-foreground: oklch(0.8686 0.2776 144.4661);
  --popover: oklch(0 0 0);
  --popover-foreground: oklch(0.8686 0.2776 144.4661);
  --primary: oklch(0.8686 0.2776 144.4661);
  --primary-foreground: oklch(0 0 0);
  --secondary: oklch(0.3053 0.1039 142.4953);
  --secondary-foreground: oklch(0.8686 0.2776 144.4661);
  --muted: oklch(0.1887 0.0642 142.4953);
  --muted-foreground: oklch(0.5638 0.1872 143.2450);
  --accent: oklch(0.8686 0.2776 144.4661);
  --accent-foreground: oklch(0 0 0);
  --destructive: oklch(0.6280 0.2577 29.2339);
  --destructive-foreground: oklch(1.0000 0 0);
  --border: oklch(0.3053 0.1039 142.4953);
  --input: oklch(0 0 0);
  --ring: oklch(0.8686 0.2776 144.4661);
  --chart-1: oklch(0.8686 0.2776 144.4661);
  --chart-2: oklch(0.5638 0.1872 143.2450);
  --chart-3: oklch(0.3053 0.1039 142.4953);
  --chart-4: oklch(0.1179 0.0327 343.3438);
  --chart-5: oklch(0.8686 0.2776 144.4661);
  --sidebar: oklch(0.1149 0 0);
  --sidebar-foreground: oklch(0.8686 0.2776 144.4661);
  --sidebar-primary: oklch(0.8686 0.2776 144.4661);
  --sidebar-primary-foreground: oklch(0 0 0);
  --sidebar-accent: oklch(0.3053 0.1039 142.4953);
  --sidebar-accent-foreground: oklch(0.8686 0.2776 144.4661);
  --sidebar-border: oklch(0.3053 0.1039 142.4953);
  --sidebar-ring: oklch(0.8686 0.2776 144.4661);
  --font-sans: "VT323", "Courier New", monospace;
  --font-serif: Georgia, serif;
  --font-mono: "VT323", monospace;
  --radius: 0rem;
  --shadow-x: 0px;
  --shadow-y: 0px;
  --shadow-blur: 15px;
  --shadow-spread: 2px;
  --shadow-opacity: 0.4;
  --shadow-color: #00FF41;
  --shadow-2xs: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.20);
  --shadow-xs: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.20);
  --shadow-sm: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.40), 0px 1px 2px 1px hsl(135.2941 100% 50% / 0.40);
  --shadow: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.40), 0px 1px 2px 1px hsl(135.2941 100% 50% / 0.40);
  --shadow-md: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.40), 0px 2px 4px 1px hsl(135.2941 100% 50% / 0.40);
  --shadow-lg: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.40), 0px 4px 6px 1px hsl(135.2941 100% 50% / 0.40);
  --shadow-xl: 0px 0px 15px 2px hsl(135.2941 100% 50% / 0.40), 0px 8px 10px 1px hsl(135.2941 100% 50% / 0.40);
  --shadow-2xl: 0px 0px 15px 2px hsl(135.2941 100% 50% / 1.00);
}

@theme inline {
  --color-background: var(--background);
  --color-foreground: var(--foreground);
  --color-card: var(--card);
  --color-card-foreground: var(--card-foreground);
  --color-popover: var(--popover);
  --color-popover-foreground: var(--popover-foreground);
  --color-primary: var(--primary);
  --color-primary-foreground: var(--primary-foreground);
  --color-secondary: var(--secondary);
  --color-secondary-foreground: var(--secondary-foreground);
  --color-muted: var(--muted);
  --color-muted-foreground: var(--muted-foreground);
  --color-accent: var(--accent);
  --color-accent-foreground: var(--accent-foreground);
  --color-destructive: var(--destructive);
  --color-destructive-foreground: var(--destructive-foreground);
  --color-border: var(--border);
  --color-input: var(--input);
  --color-ring: var(--ring);
  --color-chart-1: var(--chart-1);
  --color-chart-2: var(--chart-2);
  --color-chart-3: var(--chart-3);
  --color-chart-4: var(--chart-4);
  --color-chart-5: var(--chart-5);
  --color-sidebar: var(--sidebar);
  --color-sidebar-foreground: var(--sidebar-foreground);
  --color-sidebar-primary: var(--sidebar-primary);
  --color-sidebar-primary-foreground: var(--sidebar-primary-foreground);
  --color-sidebar-accent: var(--sidebar-accent);
  --color-sidebar-accent-foreground: var(--sidebar-accent-foreground);
  --color-sidebar-border: var(--sidebar-border);
  --color-sidebar-ring: var(--sidebar-ring);

  --font-sans: var(--font-sans);
  --font-mono: var(--font-mono);
  --font-serif: var(--font-serif);

  --radius-sm: calc(var(--radius) - 4px);
  --radius-md: calc(var(--radius) - 2px);
  --radius-lg: var(--radius);
  --radius-xl: calc(var(--radius) + 4px);

  --shadow-2xs: var(--shadow-2xs);
  --shadow-xs: var(--shadow-xs);
  --shadow-sm: var(--shadow-sm);
  --shadow: var(--shadow);
  --shadow-md: var(--shadow-md);
  --shadow-lg: var(--shadow-lg);
  --shadow-xl: var(--shadow-xl);
  --shadow-2xl: var(--shadow-2xl);

  --tracking-tighter: calc(var(--tracking-normal) - 0.05em);
  --tracking-tight: calc(var(--tracking-normal) - 0.025em);
  --tracking-normal: var(--tracking-normal);
  --tracking-wide: calc(var(--tracking-normal) + 0.025em);
  --tracking-wider: calc(var(--tracking-normal) + 0.05em);
  --tracking-widest: calc(var(--tracking-normal) + 0.1em);
}
""";
}









