using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace PipManager.Windows.Converters;

public class PackageUrlIconTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return SymbolRegular.Link24;
        }

        return (string)value switch
        {
            "homepage" or "home" => SymbolRegular.Home24,
            "download" => SymbolRegular.ArrowDownload24,
            "changelog" or "changes" or "release notes" => SymbolRegular.ClipboardTextEdit24,
            "bug tracker" or "issue tracker" or "bug reports" or "issues" or "tracker" => SymbolRegular.Bug24,
            "source code" or "source" or "repository" or "code" => SymbolRegular.Code24,
            "funding" or "donate" or "donations" => SymbolRegular.Money24,
            "documentation" => SymbolRegular.Document24,
            "commercial" => SymbolRegular.PeopleMoney24,
            "support" => SymbolRegular.PersonSupport24,
            "chat" or "q & a" => SymbolRegular.ChatHelp24,
            _ => SymbolRegular.Link24
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}