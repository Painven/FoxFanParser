using FoxFanDownloader.ViewModels;
using System.Windows;

namespace FoxFanDownloader;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    ISettingsStorage settingsStorage;
    MainWindowViewModel mainWindowViewModel;
    protected override void OnStartup(StartupEventArgs e)
    {
        FoxFanParser parser = new FoxFanParser();
        settingsStorage = new JsonSettingsStorage("settings.json");
        
        mainWindowViewModel = new(settingsStorage, parser);
        Window mainWindow = new MainWindow();
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        var settingsRoot = new SettingsRoot();
        settingsRoot.SelectedMultfilmName = mainWindowViewModel.SelectedMultfilm?.Name ?? string.Empty;
        settingsRoot.SelectedSeasonNumber = mainWindowViewModel.SelectedMultfilm?.SeasonsInfo?.SelectedSeason?.Number ?? string.Empty;
        settingsRoot.IsOneLineSubtitles = mainWindowViewModel.IsOneLineSubtitles;
        settingsStorage.SaveSettings(settingsRoot);
    }
}
