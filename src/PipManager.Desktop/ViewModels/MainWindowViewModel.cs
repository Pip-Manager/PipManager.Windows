using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PipManager.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(IEnumerable<PageViewModelBase> pageViewModels)
    {
        // Pages
        var pageViewModelsGroup = pageViewModels.GroupBy(vm => vm.Direction).ToArray();
        TopPageViewModels.AddRange(pageViewModelsGroup
            .Where(g => g.Key == VerticalAlignment.Top)
            .SelectMany(g => g)
            .OrderBy(viewModel => viewModel.Index));
        BottomPageViewModels.AddRange(pageViewModelsGroup
            .Where(g => g.Key == VerticalAlignment.Bottom)
            .SelectMany(g => g)
            .OrderBy(viewModel => viewModel.Index));
    }

    private bool _navigating;

    #region TopPageViewModels

    [ObservableProperty]
    public partial AvaloniaList<PageViewModelBase> TopPageViewModels { get; set; } = [];
    [ObservableProperty]
    public partial PageViewModelBase? SelectedTopPageViewModel { get; set; }
    [ObservableProperty]
    public partial int SelectedTopPageViewModelIndex { get; set; }

    partial void OnSelectedTopPageViewModelChanged(PageViewModelBase? oldValue, PageViewModelBase? newValue)
    {
        if (_navigating)
        {
            return;
        }
        _navigating = true;
        SelectedBottomPageViewModelIndex = -1;
        if (newValue != null)
        {
            Navigate(newValue);
        }
        _navigating = false;
    }

    #endregion

    #region BottomPageViewModels

    [ObservableProperty]
    public partial AvaloniaList<PageViewModelBase> BottomPageViewModels { get; set; } = [];
    [ObservableProperty]
    public partial PageViewModelBase? SelectedBottomPageViewModel { get; set; }
    [ObservableProperty]
    public partial int SelectedBottomPageViewModelIndex { get; set; }
    
    partial void OnSelectedBottomPageViewModelChanged(PageViewModelBase? oldValue, PageViewModelBase? newValue)
    {
        if (_navigating)
        {
            return;
        }
        _navigating = true;
        SelectedTopPageViewModelIndex = -1;
        if (newValue != null)
        {
            Navigate(newValue);
        }
        _navigating = false;
    }

    #endregion
    
    [ObservableProperty]
    public partial string AppVersion { get; set; } = AppInfo.AppVersion;
    
    [ObservableProperty]
    public partial object? CurrentPage { get; set; }

    [RelayCommand]
    private void Navigate(PageViewModelBase pageViewModel)
    {
        var currentPage = (UserControl)App.GetService(pageViewModel.ViewType);
        currentPage.DataContext = pageViewModel;
        CurrentPage = currentPage;
        pageViewModel.OnNavigatedTo();
    }
}