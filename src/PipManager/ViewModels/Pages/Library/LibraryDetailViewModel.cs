using CommunityToolkit.Mvvm.Messaging;
using PipManager.Languages;
using PipManager.Models.Package;
using PipManager.Models.Pages;
using System.Collections.ObjectModel;
using PipManager.Services.Environment.Response;
using PipManager.Services.Toast;
using PipManager.Views.Pages.Library;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryDetailViewModel : ObservableObject, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;
    public record LibraryDetailMessage(PackageItem Package, List<PackageItem> Library);
    private bool _isInitialized;

    [ObservableProperty] private PackageItem? _package;
    [ObservableProperty] private List<PackageItem>? _library;

    #region Contact

    [ObservableProperty] private string? _author;
    [ObservableProperty] private string? _authorEmail;
    [ObservableProperty] private ObservableCollection<LibraryDetailProjectUrlModel>? _projectUrl;

    #endregion Contact

    #region Classifier

    [ObservableProperty] private string? _developmentStatus;
    [ObservableProperty] private ControlAppearance _developmentStatusAppearance = ControlAppearance.Primary;

    [ObservableProperty] private ObservableCollection<string>? _programmingLanguage;
    [ObservableProperty] private ObservableCollection<string>? _intendedAudience;
    [ObservableProperty] private ObservableCollection<string>? _operatingSystem;
    [ObservableProperty] private ObservableCollection<string>? _environment;
    [ObservableProperty] private ObservableCollection<string>? _topic;

    #endregion Classifier

    public LibraryDetailViewModel(INavigationService navigationService, IToastService toastService)
    {
        _navigationService = navigationService;
        _toastService = toastService;
        WeakReferenceMessenger.Default.Register<LibraryDetailMessage>(this, Receive);
    }

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
        _isInitialized = true;
    }
    
    [RelayCommand]
    private void NavigateToDependency(string name)
    {
        var targetPackage = Library!.FirstOrDefault(item => item!.Name!.Equals(name, StringComparison.CurrentCultureIgnoreCase), null);
        if (targetPackage is null)
        {
            _toastService.Error(Lang.LibraryDetail_Toast_PackageNotFound);
            return;
        }
        _navigationService.NavigateWithHierarchy(typeof(LibraryDetailPage));
        WeakReferenceMessenger.Default.Send(new LibraryDetailMessage(targetPackage, Library!));
    }

    private void Receive(object recipient, LibraryDetailMessage message)
    {
        Package = message.Package;
        Library = message.Library;
        
        #region Contact

        Author = Package.Author!.Count == 0 ? Lang.LibraryDetail_Unknown : string.Join(", ", Package.Author!);
        AuthorEmail = Package.AuthorEmail == "" ? Lang.LibraryDetail_Unknown : Package.AuthorEmail;
        ProjectUrl = new ObservableCollection<LibraryDetailProjectUrlModel>(Package.ProjectUrl!);

        #endregion Contact
        
        #region Classifier

        // Development Status
        DevelopmentStatus = Package.Classifier!.TryGetValue("Development Status", out var value) ? value[0].Split(" - ")[^1] : Lang.LibraryDetail_Unknown;
        DevelopmentStatusAppearance = DevelopmentStatus switch
        {
            "Planning" => ControlAppearance.Info,
            "Pre-Alpha" => ControlAppearance.Danger,
            "Alpha" => ControlAppearance.Caution,
            "Beta" => ControlAppearance.Caution,
            "Production/Stable" => ControlAppearance.Success,
            "Mature" => ControlAppearance.Success,
            "Inactive" => ControlAppearance.Caution,
            _ => ControlAppearance.Caution
        };

        // Programming Language
        ProgrammingLanguage = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Programming Language", [Lang.LibraryDetail_Unknown]).Select(item => item.Replace(" :: ", " ")));

        // Intended Audience
        IntendedAudience = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Intended Audience", [Lang.LibraryDetail_Unknown]));
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
        OperatingSystem = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Operating System", [Lang.LibraryDetail_Unknown]));
        if (OperatingSystem.Count == 1 && OperatingSystem[0] == "OS Independent")
        {
            OperatingSystem[0] = Lang.LibraryDetail_Classifier_OperatingSystem_OSIndependent;
        }
        // Environment
        Environment = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Environment", [Lang.LibraryDetail_Unknown]));
        // Topic
        Topic = new ObservableCollection<string>(Package.Classifier.GetValueOrDefault("Topic", [Lang.LibraryDetail_Unknown]));

        #endregion Classifier
    }
}