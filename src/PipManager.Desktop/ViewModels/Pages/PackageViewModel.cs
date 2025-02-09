using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;
using PipManager.Desktop.Views.Pages;

namespace PipManager.Desktop.ViewModels.Pages;

public partial class PackageViewModel() : PageViewModelBase(typeof(PackagePage), Symbol.Chat, VerticalAlignment.Top, 0)
{
    [ObservableProperty]
    public partial string Text { get; set; } = "Hello, World!";
}
