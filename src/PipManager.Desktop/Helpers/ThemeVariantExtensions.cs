using System;
using Avalonia.Styling;

namespace PipManager.Desktop.Helpers;


public static class ThemeVariantExtensions
{
    public static string ToThemeVariantConfigType(this ThemeVariant themeVariant)
        => (string)themeVariant.Key switch
        {
            "Light" => "light",
            "Dark" => "dark",
            _ => throw new ArgumentOutOfRangeException(nameof(themeVariant))
        };
    
    public static ThemeVariant ToThemeVariant(this string themeVariantConfigType)
        => themeVariantConfigType switch
        {
            "light" => ThemeVariant.Light,
            "dark" => ThemeVariant.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(themeVariantConfigType))
        };
}