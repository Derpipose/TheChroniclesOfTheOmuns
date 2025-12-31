using Avalonia.Controls;
using Avalonia.Interactivity;
using PlayerAppAvalonia.Services;
using PlayerAppAvalonia.ViewModels;

namespace PlayerAppAvalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var contentArea = this.FindControl<ContentControl>("ContentArea");
        if (contentArea != null && DataContext is NavigationService navService)
        {
            contentArea.Bind(ContentControl.ContentProperty, new Avalonia.Data.Binding("CurrentViewModel") { Source = navService });
        }
    }

    private void MenuItem_Dashboard_Click(object? sender, RoutedEventArgs e)
    {
        // Dashboard will be created by DI container, just navigate to it
        // This is handled in App.xaml.cs during initialization
    }

    private void MenuItem_Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
