using FoxFanDownloader.ViewModels;
using System.Windows;

namespace FoxFanDownloader;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindowViewModel mainWindowViewModel = new();
        Window mainWindow = new MainWindow();
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();
    }
}
