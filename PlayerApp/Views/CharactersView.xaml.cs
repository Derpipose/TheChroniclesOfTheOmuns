using System.Windows;
using System.Windows.Controls;
using PlayerApp.Models;
using PlayerApp.ViewModels;

namespace PlayerApp.Views;

public partial class CharactersView : UserControl {
    public CharactersView() {
        InitializeComponent();

        // Reload characters whenever this view is shown
        this.IsVisibleChanged += async (s, e) => {
            if (this.IsVisible && DataContext is CharactersViewModel vm) {
                await vm.LoadCharacters();
            }
        };
    }

    private void CharacterCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        if (sender is Border border && border.DataContext is Character character) {
            if (DataContext is CharactersViewModel vm) {
                vm.SelectedCharacter = character;
            }
        }
    }
}
