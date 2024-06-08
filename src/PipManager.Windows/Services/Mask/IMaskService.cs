using PipManager.Windows.Controls.Mask;

namespace PipManager.Windows.Services.Mask;

public interface IMaskService
{
    public void SetMaskPresenter(MaskPresenter maskPresenter);

    public void Show(string message = "");

    public void Hide();
}