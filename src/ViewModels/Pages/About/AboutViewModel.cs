using Serilog;
using System.Collections.ObjectModel;
using PipManager.Windows.Models.Pages;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.About;

public partial class AboutViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private string _appVersion = "Development";
    [ObservableProperty] private bool _debugMode;

    private void InitializeViewModel()
    {
        DebugMode = App.IsDebugMode;
        AppVersion = AppInfo.AppVersion;
        _isInitialized = true;
        NugetLibraryList =
        [
            new AboutNugetLibraryListItem("Antelcat.I18N.WPF", "MIT", "Copyright (c) 2023 Feast",
                "https://github.com/Antelcat/Antelcat.I18N"),
            new AboutNugetLibraryListItem("CommunityToolkit.Mvvm", "MIT",
                "Copyright © .NET Foundation and Contributors", "https://github.com/CommunityToolkit/dotnet"),
            new AboutNugetLibraryListItem("HtmlAgilityPack", "MIT", "Copyright © ZZZ Projects Inc.",
                "https://github.com/zzzprojects/html-agility-pack"),
            new AboutNugetLibraryListItem("Meziantou.Framework.WPF", "MIT", "Copyright (c) 2019 Gérald Barré",
                "https://github.com/meziantou/Meziantou.Framework"),
            new AboutNugetLibraryListItem("Microsoft.Extensions.Hosting", "MIT",
                "Copyright © .NET Foundation and Contributors", "https://github.com/dotnet/runtime"),
            new AboutNugetLibraryListItem("Microsoft.Web.WebView2", "Custom License",
                "© Microsoft Corporation. All rights reserved.", "https://github.com/dotnet/runtime"),
            new AboutNugetLibraryListItem("Microsoft.Xaml.Behaviors.Wpf", "MIT", "Copyright (c) 2015 Microsoft",
                "https://github.com/microsoft/XamlBehaviorsWpf"),
            new AboutNugetLibraryListItem("Newtonsoft.Json", "MIT", "Copyright (c) 2007 James Newton-King",
                "https://github.com/JamesNK/Newtonsoft.Json"),
            new AboutNugetLibraryListItem("pythonnet", "MIT", "Copyright (c) 2006-2021 the contributors of the Python.NET project",
                "https://github.com/pythonnet/pythonnet"),
            new AboutNugetLibraryListItem("Serilog", "Apache-2.0", "Copyright © 2013-2020 Serilog Contributors",
                "https://github.com/serilog/serilog"),
            new AboutNugetLibraryListItem("Serilog.Extensions.Logging", "Apache-2.0",
                "Copyright © 2013-2020 Serilog Contributors", "https://github.com/serilog/serilog-extensions-logging"),
            new AboutNugetLibraryListItem("Serilog.Sinks.Console", "Apache-2.0",
                "Copyright © 2016 Serilog Contributors", "https://github.com/serilog/serilog-sinks-console"),
            new AboutNugetLibraryListItem("Serilog.Sinks.File", "Apache-2.0", "Copyright © 2016 Serilog Contributors",
                "https://github.com/serilog/serilog-sinks-file"),
            new AboutNugetLibraryListItem("SharpZipLib", "MIT", "Copyright © 2000-2018 SharpZipLib Contributors",
                "https://github.com/icsharpcode/SharpZipLib"),
            new AboutNugetLibraryListItem("ValueConverters", "MIT", "Copyright (c) 2019 Thomas Galliker",
                "https://github.com/thomasgalliker/ValueConverters.NET"),
            new AboutNugetLibraryListItem("WPF-UI", "MIT",
                "Copyright (c) 2021-2023 Leszek Pomianowski and WPF UI Contributors",
                "https://github.com/lepoco/wpfui"),
            new AboutNugetLibraryListItem("WPF-UI.Tray", "MIT",
                "Copyright (c) 2021-2023 Leszek Pomianowski and WPF UI Contributors",
                "https://github.com/lepoco/wpfui"),
            new AboutNugetLibraryListItem("XamlFlair.WPF", "MIT",
                "Copyright (c) 2019 XamlFlair",
                "https://github.com/XamlFlair/XamlFlair")
        ];
        Log.Information("[About] Initialized");
    }

    [ObservableProperty]
    private ObservableCollection<AboutNugetLibraryListItem> _nugetLibraryList = [];

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}