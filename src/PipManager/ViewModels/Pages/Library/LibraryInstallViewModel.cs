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

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallPackages = [];

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
        PreInstallPackages.Add(new LibraryInstallPackageItem
        {
            PackageName = await custom.ShowAsync()
        });
    }

    [RelayCommand]
    private void DeleteTask(object? parameter)
    {
        var target = -1;
        for (int index = 0; index < PreInstallPackages.Count; index++)
        {
            if (ReferenceEquals(PreInstallPackages[index].PackageName, parameter))
            {
                target = index;
            }
        }

        if (target != -1)
        {
            PreInstallPackages.RemoveAt(target);
        }
    }
}