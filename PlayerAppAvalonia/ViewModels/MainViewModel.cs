using PlayerAppAvalonia.Commands;
using PlayerAppAvalonia.Services;

namespace PlayerAppAvalonia.ViewModels;

public class MainViewModel : BaseViewModel {
    private readonly NavigationService _navigationService;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly CharactersViewModel _charactersViewModel;

    public BaseViewModel CurrentViewModel {
        get => _navigationService.CurrentViewModel;
        set {
            _navigationService.CurrentViewModel = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand ShowDashboardCommand { get; }
    public RelayCommand ShowCharactersCommand { get; }

    public MainViewModel(
        NavigationService navigationService,
        DashboardViewModel dashboardViewModel,
        CharactersViewModel charactersViewModel) {
        _navigationService = navigationService;
        _dashboardViewModel = dashboardViewModel;
        _charactersViewModel = charactersViewModel;

        ShowDashboardCommand = new RelayCommand(() => CurrentViewModel = _dashboardViewModel);
        ShowCharactersCommand = new RelayCommand(() => CurrentViewModel = _charactersViewModel);

        // Start with dashboard
        CurrentViewModel = _dashboardViewModel;
    }
}
