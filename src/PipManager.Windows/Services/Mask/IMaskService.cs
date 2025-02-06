using PipManager.Windows.Controls.Mask;

namespace PipManager.Windows.Services.Mask;

public interface IMaskService
{
    public void SetMaskPresenter(MaskPresenter maskPresenter);

    public void Show(string message = "");
    public Task ShowAsync(string message = "", int delayMilliseconds = 0);

    public void Hide();
}