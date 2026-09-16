using MudBlazor;

namespace Heracles.Web.Components.Theme;

public static class HeraclesTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1976D2",
            Secondary = "#546E7A",

            Background = "#F5F7F9",
            Surface = "#FFFFFF",

            TextPrimary = "#20262D",
            TextSecondary = "#65717D",
            Divider = "#DCE1E5",

            Success = "#2E7D32",
            Warning = "#ED6C02",
            Error = "#D32F2F",
            Info = "#0288D1"
        },

        PaletteDark = new PaletteDark
        {
            Primary = "#42A5F5",
            Secondary = "#90A4AE",

            Background = "#12171D",
            Surface = "#1B222A",

            TextPrimary = "#F1F4F6",
            TextSecondary = "#AAB4BE",
            Divider = "#35404A",

            Success = "#66BB6A",
            Warning = "#FFA726",
            Error = "#EF5350",
            Info = "#29B6F6"
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["system-ui", "-apple-system", "BlinkMacSystemFont", "\"Segoe UI\"", "sans-serif"],
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.5"
            },
            H4 = new H4Typography
            {
                FontSize = "1.75rem",
                FontWeight = "600",
                LineHeight = "1.25"
            },
            H6 = new H6Typography
            {
                FontSize = "1.25rem",
                FontWeight = "600",
                LineHeight = "1.3"
            },
            Body2 = new Body2Typography
            {
                FontSize = "0.8125rem",
                FontWeight = "400",
                LineHeight = "1.5"
            },
            Caption = new CaptionTypography
            {
                FontSize = "0.75rem",
                FontWeight = "500",
                LineHeight = "1.4"
            }
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "4px"
        }
    };
}