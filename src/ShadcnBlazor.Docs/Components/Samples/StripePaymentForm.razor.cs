using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Sonner.Services;

namespace ShadcnBlazor.Docs.Components.Samples;

public partial class StripePaymentForm : ComponentBase
{
    [Inject]
    private SonnerService Sonner { get; set; } = null!;

    private readonly PaymentFormModel _model = new();

    private readonly List<CountryGroup> _countryGroups =
    [
        new()
        {
            Region = "Americas",
            Countries =
            [
                "United States",
                "Canada",
                "Mexico",
                "Brazil",
                "Argentina",
                "Chile",
            ]
        },
        new()
        {
            Region = "Europe",
            Countries =
            [
                "United Kingdom",
                "Germany",
                "France",
                "Spain",
                "Italy",
                "Netherlands",
                "Belgium",
                "Switzerland",
                "Sweden",
                "Norway",
                "Denmark",
                "Finland",
                "Poland",
                "Ireland",
                "Austria",
                "Czech Republic",
                "Hungary",
                "Romania",
                "Greece",
                "Portugal",
            ]
        },
        new()
        {
            Region = "Asia",
            Countries =
            [
                "Japan",
                "South Korea",
                "China",
                "India",
                "Singapore",
                "Hong Kong",
                "Thailand",
                "Malaysia",
                "Indonesia",
                "Philippines",
                "Vietnam",
                "Taiwan",
                "Pakistan",
                "Bangladesh",
            ]
        },
        new()
        {
            Region = "Africa",
            Countries =
            [
                "South Africa",
                "Nigeria",
                "Kenya",
                "Egypt",
            ]
        },
        new()
        {
            Region = "Middle East",
            Countries =
            [
                "Turkey",
                "Israel",
                "Saudi Arabia",
                "UAE",
            ]
        },
        new()
        {
            Region = "Oceania",
            Countries =
            [
                "Australia",
                "New Zealand",
            ]
        },
    ];

    private sealed class CountryGroup
    {
        public string Region { get; set; } = string.Empty;
        public List<string> Countries { get; set; } = [];
    }

    private async Task OnSubmit()
    {
        // SubmitSonner is defined in PaymentForm.razor @code section
        await Sonner.ShowHeadlessAsync(SubmitSonner);
    }

    private Task FillSampleData()
    {
        _model.CardNumber = "4532 1234 5678 9010";
        _model.ExpirationDate = "12 / 25";
        _model.Cvc = "123";
        _model.CardholderName = "Jane Doe";
        _model.Country = "United States";
        _model.Zip = "10001";
        return Task.CompletedTask;
    }

    private sealed class PaymentFormModel
    {
        [Required(ErrorMessage = "Card number is required")]
        [RegularExpression(@"^\d{4}\s\d{4}\s\d{4}\s\d{4}$", ErrorMessage = "Invalid card format")]
        public string? CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required")]
        [RegularExpression(@"^\d{2}\s/\s\d{2}$", ErrorMessage = "Use MM / YY format")]
        public string? ExpirationDate { get; set; }

        [Required(ErrorMessage = "CVC is required")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVC must be 3 digits")]
        public string? Cvc { get; set; }

        [Required(ErrorMessage = "Cardholder name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? CardholderName { get; set; }

        [Required(ErrorMessage = "Country is required", AllowEmptyStrings = false)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "ZIP code is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZIP code")]
        public string? Zip { get; set; }
    }
}