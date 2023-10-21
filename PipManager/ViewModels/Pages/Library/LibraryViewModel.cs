using PipManager.Models.Pages;
using PipManager.Models.PipInspection;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using PipManager.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using PipManager.Languages;
using MessageBox = System.Windows.MessageBox;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IConfigurationService _configurationService;

    public LibraryViewModel(INavigationService navigationService, IEnvironmentService environmentService,
        IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _environmentService = environmentService;
        _configurationService = configurationService;

        Theme.Apply(_configurationService.AppConfig.Personalization.Theme switch
        {
            "light" => ThemeType.Light,
            "dark" => ThemeType.Dark,
            _ => ThemeType.Dark
        });
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _ = RefreshLibrary();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Library] Initialized");
    }

    #region Delete Package

    [RelayCommand]
    private async Task DeletePackageAsync()
    {
        var messageBox = new Wpf.Ui.Controls.MessageBox                                             
        {
            Title = Lang.MsgBox_Title_Warning,
            Content = new Grid
            {
                Children =
                {
                    new System.Windows.Controls.TextBlock
                    {
                        Text = Lang.Msgbox_Message_LibraryDeletionWarning
                    },
                    new ListView
                    {
                        ItemTemplate = Application.Current.TryFindResource("LibraryDeletionListDataTemplate") as DataTemplate,
                        Margin = new Thickness(0, 20, 0, 0),
                        ItemsSource = LibraryList.Where(libraryListItem => libraryListItem.IsSelected)
                    }
                }
            },
            MinHeight = 300,
            MaxHeight = 500,
            MinWidth = 500,
            PrimaryButtonText = Lang.MsgBox_PrimaryButton_Action,
            CloseButtonText = Lang.MsgBox_CloseButton_Cancel
        };
        var result = await messageBox.ShowDialogAsync();
        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
        {
            return;
        }
    }

    #endregion

    [ObservableProperty] private int _libraryListLength;
    [ObservableProperty] private ObservableCollection<LibraryListItem> _libraryList = new();

    [ObservableProperty] private bool _loadingVisible;
    [ObservableProperty] private bool _environmentFoundVisible;
    [ObservableProperty] private bool _listVisible;

    [RelayCommand]
    private void NavigateToAddEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    [RelayCommand]
    private async Task RefreshLibrary()
    {
        EnvironmentFoundVisible = true;
        LoadingVisible = true;
        ListVisible = false;
        if (_configurationService.AppConfig.CurrentEnvironment == null)
        {
            LoadingVisible = false;
            EnvironmentFoundVisible = false;
            return;
        }
        var library = new List<PipMetadata>();
        await Task.Run(() =>
        {
            library = _environmentService.GetLibraries();
        }).ContinueWith(_ =>
        {
            LoadingVisible = false;
        });
        if (library != null)
        {
            LibraryList = new ObservableCollection<LibraryListItem>();
            foreach (var package in library)
            {
                LibraryList.Add(new LibraryListItem
                (
                    new SymbolIcon(SymbolRegular.Box24), package.Information.Name, package.Information.Version, package.Information.Summary, false
                ));
            }
            LibraryListLength = library.Count;
            ListVisible = true;
            Log.Information("[Library] Package list refreshed successfully");
        }
    }
}