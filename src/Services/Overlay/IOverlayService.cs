﻿using PipManager.Windows.Models.Package;

namespace PipManager.Windows.Services.Overlay;

public interface IOverlayService
{
    public void ShowPackageUpdateOverlay(List<PackageUpdateItem> packageUpdates, System.Action callback);

}