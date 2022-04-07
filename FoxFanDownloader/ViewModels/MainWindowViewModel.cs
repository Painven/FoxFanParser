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
    private SettingsRoot settings;
    public ObservableCollection<Multfilm> Multfilms { get; set; }

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
    public MainWindowViewModel(ISettingsStorage settingsStorage)
    {
        
        ParseMultfimCommand = new LambdaCommand(ParseMultfilm, e => false);
        LoadedCommand = new LambdaCommand(Loaded, e => !InProgress);

        Multfilms = new ObservableCollection<Multfilm>();
        
        AddTestItems();
        this.settingsStorage = settingsStorage;
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
        if (selectedMultfilm?.SeasonsInfo != null)
        {
            selectedMultfilm.SeasonsInfo.SelectedSeason = selectedMultfilm.SeasonsInfo.Seasons?.FirstOrDefault(s => s.Number == settings.SelectedSeasonNumber);
        }
    }

    private async void ParseMultfilm(object obj)
    {
        InProgress = true;
        try
        {
            var newMult = await (new FoxFanParser()).Parse("Cleaveland Show", "https://clevelandshow.fox-fan.tv/", 4);
            
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

    private void AddTestItems()
    {
        var americanDad = new Multfilm()
        {
            Name = "American Dad",
            SeasonsInfo = new SeasonsInfo()
            {
                Seasons = new ObservableCollection<Season>(Enumerable.Range(1, 18).Select(n => new Season() { Number = (18 + 1 - n).ToString() }))
            }
        };
        Multfilms.Add(americanDad);
        var familyGuy = new Multfilm()
        {
            Name = "Family Guy",
            SeasonsInfo = new SeasonsInfo()
            {
                Seasons = new ObservableCollection<Season>(Enumerable.Range(1, 4).Select(n => new Season() { Number = (4 + 1 - n).ToString() }))
            }
        };
        Multfilms.Add(familyGuy);
        SelectedMultfilm = Multfilms.First();
    }
}
