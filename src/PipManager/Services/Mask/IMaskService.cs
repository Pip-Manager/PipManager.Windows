using PipManager.Controls.Mask;

namespace PipManager.Services.Mask;

public interface IMaskService
{
    public void SetMaskPresenter(MaskPresenter maskPresenter);

    public void Show(string message = "");

    public void Hide();
}