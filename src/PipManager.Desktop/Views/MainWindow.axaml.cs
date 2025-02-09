using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PipManager.Desktop.ViewModels;
using Serilog;

namespace PipManager.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if(e.GetCurrentPoint(this).Position.Y <= 40)
            BeginMoveDrag(e);
    }

    private void MinimizeButton_OnClick(object? sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;
    
    private void MaximizeButton_OnClick(object? sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Log.Information("[App] PipManager is shutting down");
        Log.CloseAndFlush();
        Close();
        Environment.Exit(0);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if(change.Property == WindowStateProperty && change.NewValue is WindowState windowState)
        {
            Padding = windowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
        }
    }
}