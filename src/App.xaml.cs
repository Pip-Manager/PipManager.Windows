using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using PipManager.Core.Services.PackageSearchService;
using PipManager.Windows.Services;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Configuration;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.Services.Overlay;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.ViewModels.Pages.Action;
using PipManager.Windows.ViewModels.Pages.Environment;
using PipManager.Windows.ViewModels.Pages.Lab;
using PipManager.Windows.ViewModels.Pages.Library;
using PipManager.Windows.ViewModels.Pages.Overlay;
using PipManager.Windows.ViewModels.Pages.Search;
using PipManager.Windows.ViewModels.Pages.Tools;
using PipManager.Windows.ViewModels.Windows;
using PipManager.Windows.Views.Pages.About;
using PipManager.Windows.Views.Pages.Action;
using PipManager.Windows.Views.Pages.Environment;
using PipManager.Windows.Views.Pages.Lab;
using PipManager.Windows.Views.Pages.Library;
using PipManager.Windows.Views.Pages.Overlay;
using PipManager.Windows.Views.Pages.Search;
using PipManager.Windows.Views.Pages.Settings;
using PipManager.Windows.Views.Pages.Tools;
using PipManager.Windows.Views.Windows;
using Wpf.Ui;

namespace PipManager.Windows;

using AboutViewModel = ViewModels.Pages.About.AboutViewModel;
using SettingsViewModel = ViewModels.Pages.Settings.SettingsViewModel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog();
        })
        .ConfigureAppConfiguration(c => { c.SetBasePath(AppContext.BaseDirectory); })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddTransient(_ =>
            {
                var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }) { DefaultRequestVersion = HttpVersion.Version20 };
                client.DefaultRequestHeaders.Add("User-Agent", $"PipManager.Windows/{AppInfo.AppVersion}");
                client.Timeout = TimeSpan.FromSeconds(6);
                return client;
            });

            // Window
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IMaskService, MaskService>();
            services.AddSingleton<IToastService, ToastService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IActionService, ActionService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<IPackageSearchService, PackageSearchService>();
            services.AddSingleton<IOverlayService, OverlayService>();

            // Pages
            services.AddSingleton<LibraryPage>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<LibraryDetailPage>();
            services.AddSingleton<LibraryDetailViewModel>();
            services.AddSingleton<LibraryInstallPage>();
            services.AddSingleton<LibraryInstallViewModel>();

            services.AddSingleton<OverlayPage>();
            services.AddSingleton<OverlayViewModel>();

            services.AddSingleton<ActionPage>();
            services.AddSingleton<ActionViewModel>();
            services.AddSingleton<ActionExceptionPage>();
            services.AddSingleton<ActionExceptionViewModel>();

            services.AddSingleton<SearchPage>();
            services.AddSingleton<SearchViewModel>();
            services.AddSingleton<SearchDetailPage>();
            services.AddSingleton<SearchDetailViewModel>();

            services.AddSingleton<ToolsPage>();
            services.AddSingleton<ToolsViewModel>();
            
            services.AddSingleton<LabPage>();
            services.AddSingleton<LabViewModel>();

            services.AddSingleton<EnvironmentPage>();
            services.AddSingleton<EnvironmentViewModel>();
            services.AddSingleton<AddEnvironmentPage>();
            services.AddSingleton<AddEnvironmentViewModel>();

            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AboutPage>();
            services.AddSingleton<AboutViewModel>();
        }).Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T GetService<T>()
        where T : class
    {
        return Host.Services.GetService(typeof(T)) as T ?? throw new InvalidOperationException("Service not found.");
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void FreeConsole();

    private bool _showConsoleWindow;
    private bool _experimentMode;

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);

        for (var i = 0; i != e.Args.Length; ++i)
        {
            switch (e.Args[i])
            {
                case "/debug":
                    _showConsoleWindow = true;
                    break;

                case "/experiment":
                    _experimentMode = true;
                    break;
            }
        }
        var appStarting = new AppStarting
        {
            ShowConsoleWindow = _showConsoleWindow
        };
        appStarting.StartLogging();
        appStarting.LoadLanguage();
        appStarting.CachesDeletion();
        Host.Start();
        GetService<IConfigurationService>().DebugMode = _showConsoleWindow;
        GetService<IConfigurationService>().ExperimentMode = _experimentMode;
        GetService<MainWindowViewModel>().ExperimentMode = _experimentMode;
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        if (_showConsoleWindow)
        {
            FreeConsole();
        }
        await Host.StopAsync();
        Host.Dispose();
        Log.Information("Logging ended");
        await Log.CloseAndFlushAsync();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error($"Exception: {e.Exception}");
        Directory.CreateDirectory(AppInfo.CrushesDir);
        var file = Path.Combine(AppInfo.CrushesDir, $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        File.WriteAllText(file, e.Exception.ToString());
        var exceptionWindow = new ExceptionWindow();
        exceptionWindow.Initialize(e.Exception);
        exceptionWindow.ShowDialog();
    }
}