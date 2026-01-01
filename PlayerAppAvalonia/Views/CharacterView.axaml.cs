using Avalonia.Controls;
using PlayerAppAvalonia.ViewModels;
using PlayerAppAvalonia.Services;

namespace PlayerAppAvalonia.Views;

public partial class CharacterView : UserControl {
    public CharacterView() {
        InitializeComponent();
    }

    public CharacterView(AppCharacterService characterService) {
        InitializeComponent();
        DataContext = new CharacterViewModel(characterService);
    }

    protected override void OnInitialized() {
        base.OnInitialized();
        if (DataContext is CharacterViewModel viewModel) {
            // Optionally load a character on view initialization
            // await viewModel.LoadCharacter(characterId);
        }
    }
}
