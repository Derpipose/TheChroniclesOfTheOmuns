using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using PlayerAppAvalonia.ViewModels;
using PlayerAppAvalonia.Views;

namespace PlayerAppAvalonia.Services;

public class NavigationService : INotifyPropertyChanged {
    private readonly object _sync = new();
    private readonly Stack<BaseViewModel> _navigationStack = new();
    private BaseViewModel _currentViewModel = null!;
    private Control _currentView = null!;

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
                _currentView = CreateViewForViewModel(value);
            }

            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(CurrentView));
        }
    }

    public Control CurrentView {
        get {
            lock (_sync) {
                return _currentView;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Navigate<T>(T viewModel) where T : BaseViewModel {
        _navigationStack.Push(_currentViewModel);
        CurrentViewModel = viewModel;
    }

    public void GoBack() {
        if (_navigationStack.Count > 0) {
            CurrentViewModel = _navigationStack.Pop();
        }
    }

    private Control CreateViewForViewModel(BaseViewModel viewModel) {
        return viewModel switch {
            DashboardViewModel => new DashboardView { DataContext = viewModel },
            CharactersViewModel => new CharactersView { DataContext = viewModel },
            NewCharacterViewModel => new NewCharacterView { DataContext = viewModel },
            EditCharacterViewModel => new EditCharacterView { DataContext = viewModel },
            _ => new TextBlock { Text = "Unknown View" }
        };
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

