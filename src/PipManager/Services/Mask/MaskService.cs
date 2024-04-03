using PipManager.Controls.Mask;
using PipManager.Languages;
using System.Windows.Controls;

namespace PipManager.Services.Mask;

public class MaskService : IMaskService
{
    private MaskPresenter? _presenter;
    private Grid? _grid;

    public void SetMaskPresenter(MaskPresenter maskPresenter)
    {
        _presenter = maskPresenter;
        _grid = Application.Current.TryFindResource("MaskGrid") as Grid;
    }


    public void Show(string message = "")
    {
        if (_presenter == null || _grid == null)
            throw new ArgumentNullException($"The MaskPresenter didn't set previously.");
        ((_grid.Children[0] as StackPanel)!.Children[1] as TextBlock)!.Text = Lang.Mask_Loading;
        ((_grid.Children[0] as StackPanel)!.Children[2] as TextBlock)!.Text = message;
        _ = _presenter.ShowGrid(_grid);
    }

    public void Hide()
    {
        if (_presenter == null)
            throw new ArgumentNullException($"The MaskPresenter didn't set previously.");
        _ = _presenter.HideGrid();
    }
}