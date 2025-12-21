using System.Windows;

namespace PlayerApp;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        => Application.Current.Shutdown();
}
