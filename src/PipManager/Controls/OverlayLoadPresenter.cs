using System.Windows.Controls;

namespace PipManager.Controls;

public class OverlayLoadPresenter : ContentPresenter
{
    public new Grid? Content
    {
        get => (Grid)GetValue(ContentProperty);
        protected set => SetValue(ContentProperty, value);
    }

    public async Task ShowGrid(Grid? grid)
    {
        Content = grid;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Visibility = Visibility.Visible;
        });
    }

    public async Task HideGrid()
    {
        await Application.Current.Dispatcher.InvokeAsync(()=>
        {
            Visibility = Visibility.Collapsed;
        });
        Content = null;
    }
}