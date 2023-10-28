using PipManager.Controls;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Services.OverlayLoad;

public interface IOverlayLoadService
{
    public void SetOverlayLoadPresenter(OverlayLoadPresenter overlayLoadPresenter);

    public OverlayLoadPresenter GetOverlayLoadPresenter();

    public void Show(string message);

    public void Hide();
}