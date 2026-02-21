using ShadcnBlazor.Docs.Models.Theme;

namespace ShadcnBlazor.Docs.Services.Theme;

public class ThemeFetcher(HttpClient http)
{
    public async Task<ThemeStateFull?> TryParseThemeAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var css = await TryGetContentsAsync(uri, cancellationToken);
        if (css is null) return null;

        // parse
        throw new NotImplementedException();
    }

    private async Task<string?> TryGetContentsAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}