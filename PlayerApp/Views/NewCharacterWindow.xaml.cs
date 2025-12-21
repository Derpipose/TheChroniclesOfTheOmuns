using System.Windows;
using PlayerApp.ViewModels;

namespace PlayerApp.Views;

public partial class NewCharacterWindow : Window {
    public NewCharacterWindow(NewCharacterViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}
