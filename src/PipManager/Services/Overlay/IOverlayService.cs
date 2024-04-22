using PipManager.Models.Package;

namespace PipManager.Services.Overlay;

public interface IOverlayService
{
    public void ShowPackageUpdateOverlay(List<PackageUpdateItem> packageUpdates, System.Action callback);

}
