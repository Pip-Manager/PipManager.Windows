using PipManager.Models;
using Serilog;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private IEnumerable<DataColor> _colors = null!;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        var random = new Random();
        var colorCollection = new List<DataColor>();

        for (int i = 0; i < 8192; i++)
            colorCollection.Add(
                new DataColor
                {
                    Color = new SolidColorBrush(
                        Color.FromArgb(
                            (byte)200,
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250)
                        )
                    )
                }
            );

        Colors = colorCollection;

        _isInitialized = true;
        Log.Information("[Search] Initialized");
    }
}