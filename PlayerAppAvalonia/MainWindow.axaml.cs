using Avalonia.Controls;
using Avalonia.Interactivity;
using PlayerAppAvalonia.Services;
using PlayerAppAvalonia.ViewModels;

namespace PlayerAppAvalonia;

public partial class MainWindow : Window
{
    private NavigationService? _navigationService;
    private DashboardViewModel? _dashboardViewModel;
    private CharactersViewModel? _charactersViewModel;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (s, e) =>
        {
            if (DataContext is NavigationService navService)
            {
                _navigationService = navService;
            }
        };
    }

    public void SetDependencies(NavigationService navigationService, DashboardViewModel dashboardVm, CharactersViewModel charactersVm)
    {
        _navigationService = navigationService;
        _dashboardViewModel = dashboardVm;
        _charactersViewModel = charactersVm;
    }

    private void MenuItem_Dashboard_Click(object? sender, RoutedEventArgs e)
    {
        if (_navigationService != null && _dashboardViewModel != null)
        {
            _navigationService.CurrentViewModel = _dashboardViewModel;
        }
    }

    private void MenuItem_Characters_Click(object? sender, RoutedEventArgs e)
    {
        if (_navigationService != null && _charactersViewModel != null)
        {
            _navigationService.CurrentViewModel = _charactersViewModel;
        }
    }

    private void MenuItem_Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
