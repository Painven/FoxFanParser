using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsStorage settingsStorage;
    private readonly FoxFanParser parser;
    private SettingsRoot settings;
    public ObservableCollection<Multfilm> Multfilms { get; set; }

    bool isOneLineSubtitles;
    public bool IsOneLineSubtitles
    {
        get => isOneLineSubtitles;
        set => Set(ref isOneLineSubtitles, value);
    }

    Multfilm selectedMultfilm;
    public Multfilm SelectedMultfilm
    {
        get => selectedMultfilm;
        set => Set(ref selectedMultfilm, value);
    }
    public ICommand ParseMultfimCommand { get; }
    public ICommand LoadedCommand { get; }

    bool inProgress;
    
    public bool InProgress
    {
        get => inProgress;
        set => Set(ref inProgress, value);
    }
    public MainWindowViewModel(ISettingsStorage settingsStorage, FoxFanParser parser)
    {     
        ParseMultfimCommand = new LambdaCommand(ParseMultfilm, e => false);
        LoadedCommand = new LambdaCommand(Loaded, e => !InProgress);
        Multfilms = new ObservableCollection<Multfilm>();      
        this.settingsStorage = settingsStorage;
        this.parser = parser;
    }
    private void Loaded(object obj)
    {
        Multfilms.Clear();
        string storageDir = Path.GetDirectoryName(Application.Current.GetType().Assembly.Location) + "\\storage";
        foreach (var jsonFile in Directory.GetFiles(storageDir, "*.json")) 
        {
            Multfilms.Add(JsonConvert.DeserializeObject<Multfilm>(File.ReadAllText(jsonFile)));
        }

        settings = settingsStorage.GetSettings();
        SelectedMultfilm = Multfilms.FirstOrDefault(m => m.Name == settings.SelectedMultfilmName);
        IsOneLineSubtitles = settings.IsOneLineSubtitles;
        if (selectedMultfilm?.SeasonsInfo != null)
        {
            selectedMultfilm.SeasonsInfo.SelectedSeason = selectedMultfilm.SeasonsInfo.Seasons?.FirstOrDefault(s => s.Number == settings.SelectedSeasonNumber);
        }

        foreach(var m in Multfilms)
        {
            foreach(var season in m.SeasonsInfo.Seasons)
            {
                foreach(var series in season.Series)
                {
                    series.OpenVideoClicked += async (e) => VLC_Helper.OpenVideo(await parser.GetSourceVideoFromUri(e.Uri), IsOneLineSubtitles);
                }
            }
        }
    }
    private async void ParseMultfilm(object obj)
    {
        InProgress = true;
        try
        {
            var newMult = await parser.Parse("Cleaveland Show", "https://clevelandshow.fox-fan.tv/", 4);
            
            Multfilms.Add(newMult);
            SelectedMultfilm = newMult;
            
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            InProgress = false;
        }
    }
}
