using System;
using System.Collections.Generic;
using PlayerAppAvalonia.ViewModels;

namespace PlayerAppAvalonia.Services;

public class NavigationService {
    private readonly object _sync = new();
    private readonly Stack<BaseViewModel> _navigationStack = new();
    private BaseViewModel _currentViewModel = null!;

    public BaseViewModel CurrentViewModel {
        get {
            lock (_sync) {
                return _currentViewModel;
            }
        }
        set {
            lock (_sync) {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;
            }

            // Raise event
            OnNavigated?.Invoke();
        }
    }

    public event Action? OnNavigated;

    public void Navigate<T>(T viewModel) where T : BaseViewModel {
        _navigationStack.Push(_currentViewModel);
        CurrentViewModel = viewModel;
    }

    public void GoBack() {
        if (_navigationStack.Count > 0) {
            CurrentViewModel = _navigationStack.Pop();
        }
    }
}

