using PipManager.Controls;
using System.Windows.Media;
using PipManager.Languages;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.Services.Toast;

public class ToastService : IToastService
{
    private readonly IThemeService _themeService;

    private readonly SolidColorBrush _infoIconSolidColorBrush = new(Color.FromRgb(0, 159, 170));
    private readonly SolidColorBrush _warningIconSolidColorBrush = new(Color.FromRgb(157, 93, 0));
    private readonly SolidColorBrush _errorIconSolidColorBrush = new(Color.FromRgb(196, 43, 28));

    public ToastService(IThemeService themeService)
    {
        _themeService = themeService;
    }

    private SolidColorBrush GetForegroundColorFromTheme() => _themeService.GetTheme() switch
    {
        ApplicationTheme.Light => new SolidColorBrush(Colors.Black),
        ApplicationTheme.Dark => new SolidColorBrush(Colors.White),
        _ => new SolidColorBrush(Colors.Transparent)
    };

    private SolidColorBrush GetInfoBackgroundColorFromTheme() => _themeService.GetTheme() switch
    {
        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(244, 244, 244)),
        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(39, 39, 39)),
        _ => new SolidColorBrush(Colors.Transparent)
    };

    private SolidColorBrush GetWarningBackgroundColorFromTheme() => _themeService.GetTheme() switch
    {
        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(255, 244, 206)),
        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(67, 53, 25)),
        _ => new SolidColorBrush(Colors.Transparent)
    };

    private SolidColorBrush GetErrorBackgroundColorFromTheme() => _themeService.GetTheme() switch
    {
        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(253, 231, 233)),
        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(68, 39, 38)),
        _ => new SolidColorBrush(Colors.Transparent)
    };

    private SolidColorBrush GetBorderBrushFromTheme() => _themeService.GetTheme() switch
    {
        ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(229, 229, 229)),
        ApplicationTheme.Dark => new SolidColorBrush(Color.FromRgb(29, 29, 29)),
        _ => new SolidColorBrush(Colors.Transparent)
    };

    public void Info(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Notice, message, new ToastOptions { Icon = SymbolRegular.Info24, Time = 2000, Background = GetInfoBackgroundColorFromTheme(), IconForeground = _infoIconSolidColorBrush, Foreground = GetForegroundColorFromTheme(), BorderBrush = GetBorderBrushFromTheme() });
    }

    public void Warning(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Warning, message, new ToastOptions { Icon = SymbolRegular.Warning24, Time = 2000, Background = GetWarningBackgroundColorFromTheme(), IconForeground = _warningIconSolidColorBrush, Foreground = GetForegroundColorFromTheme(), BorderBrush = GetBorderBrushFromTheme() });
    }

    public void Error(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Error, message, new ToastOptions { Icon = SymbolRegular.ShieldError24, Time = 2000, Background = GetBorderBrushFromTheme(), IconForeground = _errorIconSolidColorBrush, Foreground = GetForegroundColorFromTheme(), BorderBrush = GetBorderBrushFromTheme() });
    }
}