using System.Windows.Controls;
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
}
