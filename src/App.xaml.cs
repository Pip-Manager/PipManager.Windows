using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using Windows.Win32;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Wpf;
using PipManager.Core.Services;
using PipManager.Windows.Extensions;
using PipManager.Windows.Services;
using PipManager.Windows.ViewModels.Windows;
using PipManager.Windows.Views.Windows;

namespace PipManager.Windows;

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

            services.AddHttpClient(AppInfo.AppVersion);

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            
            services.RegisterServices();
            services.RegisterViews();
            services.RegisterViewModels();
        }).Build();

    public static T GetService<T>() where T : class
        => Host.Services.GetService(typeof(T)) as T ?? throw new InvalidOperationException("Service not found.");

    private bool _showConsoleWindow;
    public static bool IsDebugMode { get; private set; }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);

        for (var i = 0; i != e.Args.Length; ++i)
        {
            switch (e.Args[i])
            {
                case "/debug":
                    _showConsoleWindow = true;
                    break;
            }
        }
        IsDebugMode = _showConsoleWindow;
        AppStarting.CreateDirectories();
        AppStarting.LoadConfig();
        AppStarting.StartLogging(_showConsoleWindow);
        AppStarting.LoadLanguage();
        AppStarting.CachesDeletion();
        Host.Start();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        if (_showConsoleWindow) PInvoke.FreeConsole();
        await Host.StopAsync();
        Host.Dispose();
        Log.Information("Logging ended");
        await Log.CloseAndFlushAsync();
    }

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