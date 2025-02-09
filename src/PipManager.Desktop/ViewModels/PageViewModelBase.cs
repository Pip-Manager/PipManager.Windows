using System;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace PipManager.Desktop.ViewModels;

public abstract partial class PageViewModelBase(Type viewType, Symbol icon, VerticalAlignment direction, int index): ObservableObject
{
    public Type ViewType { get; } = viewType;
    
    [ObservableProperty]
    public partial Symbol Icon { get; set; } = icon;
    [ObservableProperty]
    public partial VerticalAlignment Direction { get; set; } = direction;
    [ObservableProperty]
    public partial int Index { get; set; } = index;

    protected internal virtual void OnNavigatedTo()
    {
        
    }
}