using Serilog;
using System.Collections.ObjectModel;
using PipManager.Windows.Models.Pages;
using Wpf.Ui.Abstractions.Controls;

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
            new AboutNugetItem
            {
                Name = "Antelcat.I18N.WPF",
                LicenseType = "MIT",
                Copyright = "Copyright © 2023 Feast",
                Url = "https://github.com/Antelcat/Antelcat.I18N"
            },
            new AboutNugetItem
            {
                Name = "CommunityToolkit.Mvvm",
                LicenseType = "MIT",
                Copyright = "Copyright © .NET Foundation and Contributors",
                Url = "https://github.com/CommunityToolkit/dotnet"
            },
            new AboutNugetItem
            {
                Name = "Meziantou.Framework.WPF",
                LicenseType = "MIT",
                Copyright = "Copyright © 2019 Gérald Barré",
                Url = "https://github.com/meziantou/Meziantou.Framework"
            },
            new AboutNugetItem
            {
                Name = "Microsoft.Extensions.Hosting",
                LicenseType = "MIT",
                Copyright = "Copyright © .NET Foundation and Contributors",
                Url = "https://github.com/dotnet/runtime"
            },
            new AboutNugetItem
            {
                Name = "Microsoft.Web.WebView2",
                LicenseType = "Custom License",
                Copyright = "Copyright © Microsoft Corporation. All rights reserved.",
                Url = "https://github.com/dotnet/runtime"
            },
            new AboutNugetItem
            {
                Name = "Microsoft.Xaml.Behaviors.Wpf",
                LicenseType = "MIT",
                Copyright = "Copyright © 2015 Microsoft",
                Url = "https://github.com/microsoft/XamlBehaviorsWpf"
            },
            new AboutNugetItem
            {
                Name = "Monaco Editor",
                LicenseType = "MIT",
                Copyright = "Copyright (c) 2016 - present Microsoft Corporation",
                Url = "https://github.com/microsoft/monaco-editor"
            },
            new AboutNugetItem
            {
                Name = "System.Text.Json",
                LicenseType = "MIT",
                Copyright = "Microsoft Corporation. All rights reserved.",
                Url = "https://github.com/dotnet/runtime"
            },
            new AboutNugetItem
            {
                Name = "Serilog",
                LicenseType = "Apache-2.0",
                Copyright = "Copyright © 2013-2020 Serilog Contributors",
                Url = "https://github.com/serilog/serilog"
            },
            new AboutNugetItem
            {
                Name = "Serilog.Extensions.Logging",
                LicenseType = "Apache-2.0",
                Copyright = "Copyright © 2013-2020 Serilog Contributors",
                Url = "https://github.com/serilog/serilog-extensions-logging"
            },
            new AboutNugetItem
            {
                Name = "Serilog.Sinks.Console",
                LicenseType = "Apache-2.0",
                Copyright = "Copyright © 2016 Serilog Contributors",
                Url = "https://github.com/serilog/serilog-sinks-console"
            },
            new AboutNugetItem
            {
                Name = "Serilog.Sinks.File",
                LicenseType = "Apache-2.0",
                Copyright = "Copyright © 2016 Serilog Contributors",
                Url = "https://github.com/serilog/serilog-sinks-file"
            },
            new AboutNugetItem
            {
                Name = "SharpZipLib",
                LicenseType = "MIT",
                Copyright = "Copyright © 2000-2018 SharpZipLib Contributors",
                Url = "https://github.com/icsharpcode/SharpZipLib"
            },
            new AboutNugetItem
            {
                Name = "ValueConverters",
                LicenseType = "MIT",
                Copyright = "Copyright © 2019 Thomas Galliker",
                Url = "https://github.com/thomasgalliker/ValueConverters.NET"
            },
            new AboutNugetItem
            {
                Name = "WPF-UI",
                LicenseType = "MIT",
                Copyright = "Copyright © 2021-2023 Leszek Pomianowski and WPF UI Contributors",
                Url = "https://github.com/lepoco/wpfui"
            },
            new AboutNugetItem
            {
                Name = "WPF-UI.Tray",
                LicenseType = "MIT",
                Copyright = "Copyright © 2021-2023 Leszek Pomianowski and WPF UI Contributors",
                Url = "https://github.com/lepoco/wpfui"
            },
            new AboutNugetItem
            {
                Name = "XamlFlair.WPF",
                LicenseType = "MIT",
                Copyright = "Copyright © 2019 XamlFlair",
                Url = "https://github.com/XamlFlair/XamlFlair"
            }
        ];

        Log.Information("[About] Initialized");
    }

    [ObservableProperty]
    private ObservableCollection<AboutNugetItem> _nugetLibraryList = [];

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