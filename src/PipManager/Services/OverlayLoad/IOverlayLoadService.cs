using PipManager.Controls;

namespace PipManager.Services.OverlayLoad;

public interface IOverlayLoadService
{
    public void SetOverlayLoadPresenter(OverlayLoadPresenter overlayLoadPresenter);

    public OverlayLoadPresenter GetOverlayLoadPresenter();

    public void Show(string message);

    public void Hide();
}