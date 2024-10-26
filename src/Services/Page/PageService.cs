using Wpf.Ui.Abstractions;

namespace PipManager.Windows.Services.Page;

public class PageService(IServiceProvider serviceProvider) : INavigationViewPageProvider
{
    public object? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return serviceProvider.GetService(pageType) as FrameworkElement;
    }
}
