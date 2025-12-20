using System;
using System.Windows;
using PlayerApp.ViewModels;

namespace PlayerApp.Services;

public class NavigationService
{
    private readonly object _sync = new();
    private BaseViewModel _currentViewModel = null!;

    public BaseViewModel CurrentViewModel
    {
        get
        {
            lock (_sync)
            {
                return _currentViewModel;
            }
        }
        set
        {
            lock (_sync)
            {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;
            }

            // Raise event on UI thread
            var handler = OnNavigated;
            if (handler != null)
            {
                Application.Current.Dispatcher.Invoke(handler);
            }
        }
    }

    public event Action? OnNavigated;

    public void Navigate<T>(T viewModel) where T : BaseViewModel
        => CurrentViewModel = viewModel;
}
