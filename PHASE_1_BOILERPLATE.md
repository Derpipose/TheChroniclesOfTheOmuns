# Phase 1 Boilerplate: WPF + MVVM for Chronicles of the Omuns

This is a **production-ready starting point** for Phase 1. It uses your existing models, DBContext, and follows proper MVVM patterns.

---

## Project Structure

```
PlayerApp/
├── App.xaml
├── App.xaml.cs                    (DI setup)
├── MainWindow.xaml
├── MainWindow.xaml.cs
│
├── Views/
│   ├── DashboardView.xaml
│   ├── DashboardView.xaml.cs
│   ├── CharactersView.xaml
│   └── CharactersView.xaml.cs
│
├── ViewModels/
│   ├── BaseViewModel.cs           (abstract base)
│   ├── MainViewModel.cs           (root VM)
│   ├── DashboardViewModel.cs
│   └── CharactersViewModel.cs
│
├── Services/
│   ├── NavigationService.cs
│   ├── CharacterService.cs        (EF Core CRUD)
│   └── DialogService.cs           (optional, Phase 1.5)
│
└── Commands/
    └── RelayCommand.cs
```

---

## 1. BaseViewModel.cs

The foundation. Every ViewModel inherits from this.

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlayerApp.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```

---

## 2. RelayCommand.cs

No external dependencies. Clean and simple.

```csharp
using System;
using System.Windows.Input;

namespace PlayerApp.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute != null ? _ => canExecute() : null)
        {
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
            => CommandManager.InvalidateRequerySuggested();
    }
}
```

---

## 3. NavigationService.cs

Simple navigation controller. Doesn't need to be fancy.

```csharp
using System;
using PlayerApp.ViewModels;

namespace PlayerApp.Services
{
    public class NavigationService
    {
        private BaseViewModel _currentViewModel = null!;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnNavigated?.Invoke();
                }
            }
        }

        public event Action? OnNavigated;

        public void Navigate<T>(T viewModel) where T : BaseViewModel
            => CurrentViewModel = viewModel;
    }
}
```

---

## 4. CharacterService.cs

Uses your existing `PlayerApp.Models.Character` and `DBContext`.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Database;
using PlayerApp.Models;

namespace PlayerApp.Services
{
    public class CharacterService
    {
        private readonly ApplicationDbContext _dbContext;

        public CharacterService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Character>> GetAllCharactersAsync()
            => await _dbContext.Characters.ToListAsync();

        public async Task<Character?> GetCharacterByIdAsync(int id)
            => await _dbContext.Characters.FindAsync(id);

        public async Task<Character> CreateCharacterAsync(Character character)
        {
            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();
            return character;
        }

        public async Task<Character> UpdateCharacterAsync(Character character)
        {
            _dbContext.Characters.Update(character);
            await _dbContext.SaveChangesAsync();
            return character;
        }

        public async Task DeleteCharacterAsync(int id)
        {
            var character = await _dbContext.Characters.FindAsync(id);
            if (character != null)
            {
                _dbContext.Characters.Remove(character);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
```

---

## 5. MainViewModel.cs

Root ViewModel. Controls navigation between screens.

```csharp
using System.Collections.Generic;
using PlayerApp.Commands;
using PlayerApp.Services;
using PlayerApp.ViewModels;

namespace PlayerApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly CharactersViewModel _charactersViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _navigationService.CurrentViewModel;
            set
            {
                _navigationService.CurrentViewModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowCharactersCommand { get; }

        public MainViewModel(
            NavigationService navigationService,
            DashboardViewModel dashboardViewModel,
            CharactersViewModel charactersViewModel)
        {
            _navigationService = navigationService;
            _dashboardViewModel = dashboardViewModel;
            _charactersViewModel = charactersViewModel;

            ShowDashboardCommand = new RelayCommand(() => CurrentViewModel = _dashboardViewModel);
            ShowCharactersCommand = new RelayCommand(() => CurrentViewModel = _charactersViewModel);

            // Start with dashboard
            CurrentViewModel = _dashboardViewModel;

            _navigationService.OnNavigated += () => OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
```

---

## 6. DashboardViewModel.cs

Simple home screen. Phase 1 can be minimal.

```csharp
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerApp.Models;
using PlayerApp.Services;

namespace PlayerApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly CharacterService _characterService;
        private ObservableCollection<Character> _recentCharacters = new();

        public ObservableCollection<Character> RecentCharacters
        {
            get => _recentCharacters;
            set => SetProperty(ref _recentCharacters, value);
        }

        public DashboardViewModel(CharacterService characterService)
        {
            _characterService = characterService;
            _ = LoadRecentCharacters();
        }

        private async Task LoadRecentCharacters()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            RecentCharacters = new ObservableCollection<Character>(characters);
        }
    }
}
```

---

## 7. CharactersViewModel.cs

Manage all characters: list, create, delete.

```csharp
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerApp.Commands;
using PlayerApp.Models;
using PlayerApp.Services;

namespace PlayerApp.ViewModels
{
    public class CharactersViewModel : BaseViewModel
    {
        private readonly CharacterService _characterService;
        private ObservableCollection<Character> _characters = new();
        private Character? _selectedCharacter;

        public ObservableCollection<Character> Characters
        {
            get => _characters;
            set => SetProperty(ref _characters, value);
        }

        public Character? SelectedCharacter
        {
            get => _selectedCharacter;
            set => SetProperty(ref _selectedCharacter, value);
        }

        public RelayCommand LoadCharactersCommand { get; }
        public RelayCommand DeleteCharacterCommand { get; }

        public CharactersViewModel(CharacterService characterService)
        {
            _characterService = characterService;
            LoadCharactersCommand = new RelayCommand(async _ => await LoadCharacters());
            DeleteCharacterCommand = new RelayCommand(async _ => await DeleteSelectedCharacter());

            _ = LoadCharacters();
        }

        private async Task LoadCharacters()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            Characters = new ObservableCollection<Character>(characters);
        }

        private async Task DeleteSelectedCharacter()
        {
            if (SelectedCharacter?.Id > 0)
            {
                await _characterService.DeleteCharacterAsync(SelectedCharacter.Id);
                await LoadCharacters();
            }
        }
    }
}
```

---

## 8. App.xaml.cs

Dependency Injection Setup. The magic happens here.

```csharp
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayerApp.Database;
using PlayerApp.Services;
using PlayerApp.ViewModels;

namespace PlayerApp
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();

            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=chronicle.db"));

            // Register Services
            services.AddScoped<CharacterService>();
            services.AddSingleton<NavigationService>();

            // Register ViewModels
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<CharactersViewModel>();
            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create DB if it doesn't exist
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.EnsureCreated();
            }

            // Show MainWindow with MainViewModel
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            MainWindow = new MainWindow { DataContext = mainViewModel };
            MainWindow.Show();
        }
    }
}
```

---

## 9. App.xaml

Register DataTemplates for automatic view resolution.

```xml
<Application x:Class="PlayerApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:PlayerApp"
             xmlns:vm="clr-namespace:PlayerApp.ViewModels"
             xmlns:v="clr-namespace:PlayerApp.Views">

    <Application.Resources>
        <!-- DataTemplates automatically bind ViewModels to Views -->
        <DataTemplate DataType="{x:Type vm:DashboardViewModel}">
            <v:DashboardView />
        </DataTemplate>
        
        <DataTemplate DataType="{x:Type vm:CharactersViewModel}">
            <v:CharactersView />
        </DataTemplate>

        <!-- Global Styles -->
        <Style TargetType="Window">
            <Setter Property="FontFamily" Value="Segoe UI"/>
            <Setter Property="FontSize" Value="12"/>
        </Style>
    </Application.Resources>
</Application>
```

---

## 10. MainWindow.xaml

Root window with menu navigation.

```xml
<Window x:Class="PlayerApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Chronicles of the Omuns - Character Manager"
        Height="700"
        Width="1000"
        WindowStartupLocation="CenterScreen">

    <DockPanel>
        <!-- Menu Bar -->
        <Menu DockPanel.Dock="Top" Height="30">
            <MenuItem Header="View">
                <MenuItem Header="Dashboard" Command="{Binding ShowDashboardCommand}"/>
                <MenuItem Header="Characters" Command="{Binding ShowCharactersCommand}"/>
                <Separator/>
                <MenuItem Header="Exit" Click="MenuItem_Exit_Click"/>
            </MenuItem>
            <MenuItem Header="Help">
                <MenuItem Header="About"/>
            </MenuItem>
        </Menu>

        <!-- Main Content Area - automatic view injection via ContentControl -->
        <ContentControl Content="{Binding CurrentViewModel}" DockPanel.Dock="Bottom"/>
    </DockPanel>
</Window>
```

---

## 11. MainWindow.xaml.cs

Minimal code-behind. Just UI plumbing.

```csharp
using System.Windows;

namespace PlayerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();
    }
}
```

---

## 12. DashboardView.xaml

Placeholder Phase 1 dashboard. Expand as needed.

```xml
<UserControl x:Class="PlayerApp.Views.DashboardView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Grid Background="White">
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
            <TextBlock Text="Welcome to Chronicles of the Omuns" 
                       FontSize="28" FontWeight="Bold" 
                       Margin="0,0,0,20"/>
            
            <TextBlock Text="Select an option from the menu to begin." 
                       FontSize="14" Foreground="Gray"/>
            
            <StackPanel Margin="0,40,0,0" Orientation="Horizontal" HorizontalAlignment="Center" Spacing="20">
                <Button Content="Manage Characters" Padding="20,10" FontSize="14"/>
                <Button Content="New Campaign" Padding="20,10" FontSize="14"/>
            </StackPanel>
        </StackPanel>
    </Grid>
</UserControl>
```

---

## 13. DashboardView.xaml.cs

Code-behind can be empty. MVVM handles it.

```csharp
using System.Windows.Controls;

namespace PlayerApp.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }
    }
}
```

---

## 14. CharactersView.xaml

List all characters with basic CRUD.

```xml
<UserControl x:Class="PlayerApp.Views.CharactersView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Grid Background="White" Margin="20">
        <StackPanel>
            <TextBlock Text="Characters" FontSize="24" FontWeight="Bold" Margin="0,0,0,20"/>
            
            <!-- Character List -->
            <DataGrid ItemsSource="{Binding Characters}"
                      SelectedItem="{Binding SelectedCharacter}"
                      AutoGenerateColumns="False"
                      Height="400"
                      Margin="0,0,0,20">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="*"/>
                    <DataGridTextColumn Header="Class" Binding="{Binding Class}" Width="150"/>
                    <DataGridTextColumn Header="Race" Binding="{Binding Race}" Width="150"/>
                    <DataGridTextColumn Header="Level" Binding="{Binding Level}" Width="80"/>
                </DataGrid.Columns>
            </DataGrid>

            <!-- Action Buttons -->
            <StackPanel Orientation="Horizontal" Spacing="10">
                <Button Content="New Character" Padding="15,8" FontSize="12"/>
                <Button Content="Edit" Padding="15,8" FontSize="12"/>
                <Button Content="Delete" 
                        Command="{Binding DeleteCharacterCommand}"
                        Padding="15,8" FontSize="12" 
                        Foreground="White" Background="Red"/>
            </StackPanel>
        </StackPanel>
    </Grid>
</UserControl>
```

---

## 15. CharactersView.xaml.cs

```csharp
using System.Windows.Controls;

namespace PlayerApp.Views
{
    public partial class CharactersView : UserControl
    {
        public CharactersView()
        {
            InitializeComponent();
        }
    }
}
```

---

## Installation Steps

### 1. Add Required NuGet Packages

```powershell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Extensions.DependencyInjection
```

### 2. Create the Folder Structure

```
PlayerApp/
├── Commands/
├── Views/
├── ViewModels/
├── Services/
```

### 3. Copy Files in Order

1. `RelayCommand.cs` → Commands/
2. `BaseViewModel.cs` → ViewModels/
3. `NavigationService.cs` → Services/
4. `CharacterService.cs` → Services/
5. ViewModels (Dashboard, Characters, Main)
6. Views (Dashboard, Characters)
7. Update `App.xaml` and `App.xaml.cs`
8. Update `MainWindow.xaml` and `MainWindow.xaml.cs`

### 4. Build and Run

```powershell
dotnet build
dotnet run
```

---

## What This Gives You

✅ Full MVVM separation of concerns  
✅ Dependency injection working out of the box  
✅ Navigation between screens without code-behind logic  
✅ Real EF Core integration with your existing DBContext  
✅ Uses your actual Character model, not a toy example  
✅ Async/await throughout for database operations  
✅ ObservableCollections for live UI updates  
✅ RelayCommands for all user actions  
✅ Ready to extend with Phase 2 features  

---

## Next Steps (Phase 1.5)

Once Phase 1 is solid, consider:

- Character creation dialog/form
- Character detail view
- Import/export functionality
- Basic theme/styling pass
- Unit tests for services and ViewModels

---

## Notes

- All database operations are async. Don't block the UI thread.
- Use `SetProperty()` helper in BaseViewModel to reduce boilerplate.
- Keep ViewModels stateless where possible; let services manage data.
- Each View should have zero business logic. That's what the ViewModel is for.
- Navigation is controlled by commands, not code-behind events.
