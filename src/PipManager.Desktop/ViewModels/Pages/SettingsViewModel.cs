using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using PipManager.Core.Configuration;
using PipManager.Desktop.Helpers;
using PipManager.Desktop.Languages;
using PipManager.Desktop.Views.Pages;
using Serilog;

namespace PipManager.Desktop.ViewModels.Pages;

public partial class SettingsViewModel: PageViewModelBase
{
    public SettingsViewModel() : base(typeof(SettingsPage), Symbol.Settings, VerticalAlignment.Bottom, 1)
    {
        // Theme Init
        Theme = Configuration.AppConfig.Personalization.Theme.ToThemeVariant();

        // Language Init
        foreach (var languagePair in GetLanguage.LanguageList)
        {
            Languages.Add(languagePair.Key);
        }

        var language = Configuration.AppConfig.Personalization.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
    }

    #region Theme

    [ObservableProperty] public partial ThemeVariant Theme { get; set; } = null!;

    partial void OnThemeChanged(ThemeVariant value)
    {
        Application.Current!.RequestedThemeVariant = (string)value.Key switch
        {
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
        Configuration.AppConfig.Personalization.Theme = value.ToThemeVariantConfigType();
        Configuration.Save();
        Log.Information($"[Settings] Theme variant changed to {value}");
    }

    #endregion

    #region Language

    [ObservableProperty]
    public partial ObservableCollection<string> Languages { get; set; } = ["Auto"];
    [ObservableProperty] 
    public partial string Language { get; set; } = "Auto";

    [RelayCommand]
    private void ChangeLanguage()
    {
        var language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        I18NExtension.Culture = language != "Auto" ? new CultureInfo(language) : CultureInfo.CurrentCulture;
        Configuration.AppConfig.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        Configuration.Save();
        Log.Information("[Settings] Language changed to {Language}", Language);
    }

    #endregion
}