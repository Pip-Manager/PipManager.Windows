using System.Windows.Controls;
using System.Windows.Media;
using PipManager.Controls;
using PipManager.Languages;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PipManager.Services.OverlayLoad;

public class OverlayLoadService:IOverlayLoadService
{
    private OverlayLoadPresenter? _presenter;
    private Grid? _grid;

    public void SetOverlayLoadPresenter(OverlayLoadPresenter overlayLoadPresenter)
    {
        _presenter = overlayLoadPresenter;
        _grid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(70, 0, 0, 0))
        };
        _grid.Children.Add(new ProgressRing{Width = 100, Height = 100, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, IsIndeterminate = true});
        _grid.Children.Add(new TextBlock{FontSize = 24, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 160, 0, 0)});
        _grid.Children.Add(new TextBlock{ HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 220, 0, 0) });
    }

    public OverlayLoadPresenter GetOverlayLoadPresenter() => _presenter ?? throw new ArgumentNullException("The OverlayLoadPresenter didn't set previously.");
    
    public void Show(string message)
    {
        if (_presenter == null || _grid == null)
            throw new ArgumentNullException("The OverlayLoadPresenter didn't set previously.");
        (_grid.Children[1] as TextBlock)!.Text = Lang.OverlayLoad_Loading;
        (_grid.Children[2] as TextBlock)!.Text = message;
        _presenter.ShowGrid(_grid);
    }

    public void Hide()
    {
        if (_presenter == null)
            throw new ArgumentNullException("The OverlayLoadPresenter didn't set previously.");
        _presenter.HideGrid();
    }
}