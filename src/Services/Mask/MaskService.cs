using System.Windows.Controls;
using PipManager.Windows.Controls.Mask;
using PipManager.Windows.Languages;

namespace PipManager.Windows.Services.Mask;

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
    
    private readonly object _lock = new object();
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task ShowAsync(string message = "", int delayMilliseconds = 0)
    {
        if (_presenter == null || _grid == null)
            throw new ArgumentNullException($"The MaskPresenter wasn't set previously.");

        // Set the loading texts
        var stackPanel = _grid.Children[0] as StackPanel;
        if (stackPanel != null)
        {
            var loadingTextBlock = stackPanel.Children[1] as TextBlock;
            var messageTextBlock = stackPanel.Children[2] as TextBlock;
            if (loadingTextBlock != null)
                loadingTextBlock.Text = Lang.Mask_Loading;
            if (messageTextBlock != null)
                messageTextBlock.Text = message;
        }

        if (delayMilliseconds > 0)
        {
            lock (_lock)
            {
                // Cancel any existing pending Show
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();
            }

            try
            {
                await Task.Delay(delayMilliseconds, _cancellationTokenSource.Token);
                _presenter.ShowGrid(_grid);
            }
            catch (TaskCanceledException)
            {
                // The delay was canceled, do not show the mask
            }
        }
        else
        {
            _presenter.ShowGrid(_grid);
        }
    }

    public void Hide()
    {
        if (_presenter == null)
            throw new ArgumentNullException($"The MaskPresenter didn't set previously.");
        _ = _presenter.HideGrid();
        
        lock (_lock)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}