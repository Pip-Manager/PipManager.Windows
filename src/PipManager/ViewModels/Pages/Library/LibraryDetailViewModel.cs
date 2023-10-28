using PipManager.Languages;
using PipManager.Models.Pages;
using PipManager.Services.Environment;
using System.Collections.ObjectModel;
using PipManager.Models;
using PipManager.Services.OverlayLoad;
using Wpf.Ui;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryDetailViewModel : ObservableObject, INavigationAware
{
    public record LibraryDetailMessage(PackageItem Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private PackageItem? _package;

    #region Contact

    [ObservableProperty] private string? _author;
    [ObservableProperty] private string? _authorEmail;
    [ObservableProperty] private ObservableCollection<LibraryDetailProjectUrlModel>? _projectUrl;

    #endregion

    #region Classifier

    [ObservableProperty] private string? _developmentStatus;
    [ObservableProperty] private ControlAppearance _developmentStatusAppearance = ControlAppearance.Primary;

    [ObservableProperty] private ObservableCollection<string>? _programmingLanguage;
    [ObservableProperty] private ObservableCollection<string>? _intendedAudience;
    [ObservableProperty] private ObservableCollection<string>? _operatingSystem;
    [ObservableProperty] private ObservableCollection<string>? _environment;
    [ObservableProperty] private ObservableCollection<string>? _topic;

    #endregion

    public LibraryDetailViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        WeakReferenceMessenger.Default.Register<LibraryDetailMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
    }

    public void OnNavigatedFrom()
    {
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    public void Receive(object recipient, LibraryDetailMessage message)
    {
        Package = message.Package;

        #region Contact

        Author = Package.Author.Count == 0 ? Lang.LibraryDetail_Unknown : string.Join(", ", Package.Author);
        AuthorEmail = Package.AuthorEmail == "" ? Lang.LibraryDetail_Unknown : Package.AuthorEmail;
        ProjectUrl = new ObservableCollection<LibraryDetailProjectUrlModel>(Package.ProjectUrl);

        #endregion

        #region Classifier

        // Development Status
        DevelopmentStatus = Package.Classifier.TryGetValue("Development Status", out var value) ? value[0].Split(" - ")[^1] : Lang.LibraryDetail_Unknown;
        DevelopmentStatusAppearance = DevelopmentStatus switch
        {
            "Planning" => ControlAppearance.Info,
            "Pre-Alpha" => ControlAppearance.Danger,
            "Alpha" => ControlAppearance.Caution,
            "Beta" => ControlAppearance.Caution,
            "Production/Stable" => ControlAppearance.Success,
            "Mature" => ControlAppearance.Success,
            "Inactive" => ControlAppearance.Caution,
            _ => ControlAppearance.Secondary
        };

        // Programming Language
        ProgrammingLanguage = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Programming Language", new List<string> { Lang.LibraryDetail_Unknown }).Select(item => item.Replace(" :: ", " ")));

        // Intended Audience
        IntendedAudience = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Intended Audience", new List<string> { Lang.LibraryDetail_Unknown }));
        if (IntendedAudience[0] != Lang.LibraryDetail_Unknown)
        {
            IntendedAudience = new ObservableCollection<string>(IntendedAudience.Select(item => item switch
            {
                "Customer Service" => Lang.LibraryDetail_Classifier_IntendedAudience_CustomerService,
                "Developers" => Lang.LibraryDetail_Classifier_IntendedAudience_Developers,
                "Education" => Lang.LibraryDetail_Classifier_IntendedAudience_Education,
                "End Users/Desktop" => Lang.LibraryDetail_Classifier_IntendedAudience_EndUsersDesktop,
                "Financial and Insurance Industry" => Lang.LibraryDetail_Classifier_IntendedAudience_FinancialAndInsuranceIndustry,
                "Healthcare Industry" => Lang.LibraryDetail_Classifier_IntendedAudience_HealthcareIndustry,
                "Information Technology" => Lang.LibraryDetail_Classifier_IntendedAudience_InformationTechonology,
                "Legal Industry" => Lang.LibraryDetail_Classifier_IntendedAudience_LegalIndustry,
                "Manufacturing" => Lang.LibraryDetail_Classifier_IntendedAudience_Manufacturing,
                "Other Audience" => Lang.LibraryDetail_Classifier_IntendedAudience_OtherAudience,
                "Religion" => Lang.LibraryDetail_Classifier_IntendedAudience_Religion,
                "Science/Research" => Lang.LibraryDetail_Classifier_IntendedAudience_ScienceResearch,
                "System Administrators" => Lang.LibraryDetail_Classifier_IntendedAudience_SystemAdministrators,
                "Telecommunications Industry" => Lang.LibraryDetail_Classifier_IntendedAudience_TelecommunicationsIndustry,
                _ => item
            }));
        }
        // Operating System
        OperatingSystem = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Operating System", new List<string> { Lang.LibraryDetail_Unknown }));
        if (OperatingSystem.Count == 1 && OperatingSystem[0] == "OS Independent")
        {
            OperatingSystem[0] = Lang.LibraryDetail_Classifier_OperatingSystem_OSIndependent;
        }
        // Environment
        Environment = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Environment", new List<string> { Lang.LibraryDetail_Unknown }));
        // Topic
        Topic = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Topic", new List<string> { Lang.LibraryDetail_Unknown }));

        #endregion
    }
}