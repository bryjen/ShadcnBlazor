namespace ShadcnBlazor.Docs.Pages.Components.DataTable;

/// <summary>
/// Sample data service for DataTable examples. Provides fake data with async delay.
/// </summary>
public static class DataTableSampleService
{
    /// <summary>
    /// Gets sample people data with a simulated 500ms delay (to demonstrate loading state).
    /// </summary>
    public static async Task<IReadOnlyList<PersonDto>> GetPeopleAsync()
    {
        await Task.Delay(500);
        return People;
    }

    public record PersonDto(string Name, string Email, string Role, int Experience);

    private static readonly PersonDto[] People =
    [
        new("Alice Johnson",   "alice@example.com",   "Engineer",  8),
        new("Bob Smith",       "bob@example.com",     "Designer",  5),
        new("Carol Williams",  "carol@example.com",   "Engineer",  10),
        new("David Brown",     "david@example.com",   "Manager",   12),
        new("Eve Davis",       "eve@example.com",     "Engineer",  6),
        new("Frank Miller",    "frank@example.com",   "Designer",  3),
        new("Grace Wilson",    "grace@example.com",   "Engineer",  9),
        new("Hank Moore",      "hank@example.com",    "Manager",   15),
    ];
}
