using System.Collections.ObjectModel;
using PipManager.Controls.Library;
using System.Windows.Controls.Primitives;
using PipManager.Models.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryInstallViewModel(INavigationService navigationService, IContentDialogService contentDialogService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    private readonly IContentDialogService _contentDialogService = contentDialogService;

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallPackages = new();

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
        _isInitialized = true;
    }

    [RelayCommand]
    private async void AddTask()
    {
        var custom = new InstallAddContentDialog(_contentDialogService.GetContentPresenter());
        _preInstallPackages.Add(new LibraryInstallPackageItem
        {
            PackageName = await custom.ShowAsync()
        });
    }

    [RelayCommand]
    private void DeleteTask(object? parameter)
    {
        var target = -1;
        for (int index = 0; index < _preInstallPackages.Count; index++)
        {
            if (ReferenceEquals(_preInstallPackages[index].PackageName, parameter))
            {
                target = index;
            }
        }

        if (target != -1)
        {
            _preInstallPackages.RemoveAt(target);
        }
    }
}