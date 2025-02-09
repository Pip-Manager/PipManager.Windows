using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using PipManager.Desktop.Views;

namespace PipManager.Desktop.Controls;

public class InlineHyperlink : InlineUIContainer
{
    private readonly Underline _underline;

    public Span Content => _underline;

    public InlineHyperlink(string? href)
    {
        _underline = new Underline();

        var textBlock = new TextBlock
        {
            Inlines = [_underline]
        };

        var button = new Button
        {
            Cursor = new Cursor(StandardCursorType.Hand),
            Content = textBlock
        };

        if (href is not null)
        {
            var url = new Uri(href);

            button.Command = new AsyncRelayCommand(async () => await OpenUrl(url));

            ToolTip.SetTip(button, href);
        }

        Child = button;
    }
    
    private static async Task OpenUrl(Uri? url)
    {
        if (url is not null)
        {
            await App.GetService<MainWindow>().Launcher.LaunchUriAsync(url);
        }
    }
}