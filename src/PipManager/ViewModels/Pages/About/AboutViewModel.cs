using PipManager.Models.Pages;
using PipManager.Services.Configuration;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.About;

public partial class AboutViewModel(IConfigurationService configurationService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private string _appVersion = "Development";
    [ObservableProperty] private bool _debugMode;
    [ObservableProperty] private bool _experimentMode;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        DebugMode = configurationService.DebugMode;
        ExperimentMode = configurationService.ExperimentMode;
        AppVersion = AppInfo.AppVersion;
        _isInitialized = true;
        Log.Information("[About] Initialized");
    }

    [ObservableProperty]
    private ObservableCollection<AboutNugetLibraryListItem> _nugetLibraryList =
    [
        new AboutNugetLibraryListItem("Antelcat.I18N.WPF", "MIT", "Copyright (c) 2023 Feast", "https://github.com/Antelcat/Antelcat.I18N"),
        new AboutNugetLibraryListItem("CommunityToolkit.Mvvm", "MIT", "Copyright © .NET Foundation and Contributors", "https://github.com/CommunityToolkit/dotnet"),
        new AboutNugetLibraryListItem("Microsoft.Extensions.Hosting", "MIT", "Copyright © .NET Foundation and Contributors", "https://github.com/dotnet/runtime"),
        new AboutNugetLibraryListItem("Microsoft.Xaml.Behaviors.Wpf", "MIT", "Copyright (c) 2015 Microsoft", "https://github.com/microsoft/XamlBehaviorsWpf"),
        new AboutNugetLibraryListItem("Newtonsoft.Json", "MIT", "Copyright (c) 2007 James Newton-King", "https://github.com/JamesNK/Newtonsoft.Json"),
        new AboutNugetLibraryListItem("Serilog", "3.1.1", "Copyright © 2013-2020 Serilog Contributors", "https://github.com/serilog/serilog"),
        new AboutNugetLibraryListItem("Serilog.Sinks.Console", "Apache-2.0", "Copyright © 2016 Serilog Contributors", "https://github.com/serilog/serilog-sinks-console"),
        new AboutNugetLibraryListItem("Serilog.Sinks.File", "Apache-2.0", "Copyright © 2016 Serilog Contributors", "https://github.com/serilog/serilog-sinks-file"),
        new AboutNugetLibraryListItem("ValueConverters", "MIT", "Copyright (c) 2019 Thomas Galliker", "https://github.com/thomasgalliker/ValueConverters.NET"),
        new AboutNugetLibraryListItem("WPF-UI", "MIT", "Copyright (c) 2021-2023 Leszek Pomianowski and WPF UI Contributors", "https://github.com/lepoco/wpfui"),
        new AboutNugetLibraryListItem("WPF-UI.Tray", "MIT", "Copyright (c) 2021-2023 Leszek Pomianowski and WPF UI Contributors", "https://github.com/lepoco/wpfui"),
        new AboutNugetLibraryListItem("HtmlAgilityPack", "MIT", "Copyright © ZZZ Projects Inc.", "https://github.com/zzzprojects/html-agility-pack"),
    ];
}