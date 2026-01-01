using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using PlayerApp.Models;
using PlayerAppAvalonia.ViewModels;

namespace PlayerAppAvalonia.Views;

public partial class CharactersView : UserControl
{
    private Border? _lastSelectedBorder;

    public CharactersView()
    {
        InitializeComponent();
        DataContextChanged += async (s, e) =>
        {
            if (DataContext is CharactersViewModel vm)
            {
                await vm.LoadCharacters();
            }
        };
    }

    private void CharacterCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is Character character)
        {
            if (DataContext is CharactersViewModel vm)
            {
                vm.SelectedCharacter = character;
                
                // Update visual feedback
                if (_lastSelectedBorder != null)
                {
                    _lastSelectedBorder.BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0"));
                    _lastSelectedBorder.BorderThickness = new Thickness(1);
                }
                
                border.BorderBrush = new SolidColorBrush(Color.Parse("#27AE60"));
                border.BorderThickness = new Thickness(3);
                _lastSelectedBorder = border;
            }
        }
    }

    private void CharacterCard_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.DataContext is Character character)
        {
            if (DataContext is CharactersViewModel vm)
            {
                vm.SelectedCharacter = character;
                vm.ViewCharacterCommand.Execute(null);
            }
        }
    }
}
