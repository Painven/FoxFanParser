using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class AddNewCartoonSourceWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModel mainWindow;
    private readonly FoxFanParser parser;
    private readonly ISettingsStorage settingsStorage;

    public ObservableCollection<Cartoon> AllCartoonsExcludeAdded { get; set; } = new();

    Cartoon selectedCartoon;
    public Cartoon SelectedCartoon
    {
        get => selectedCartoon;
        set => Set(ref selectedCartoon, value);
    }

    bool parsingInProgress;
    public bool ParsingInProgress
    {
        get => parsingInProgress;
        set => Set(ref parsingInProgress, value);
    }

    public ICommand AddNewCartoonCommand { get; }
    public ICommand LoadedCommand { get; }

    public AddNewCartoonSourceWindowViewModel(MainWindowViewModel mainWindow, FoxFanParser parser, ISettingsStorage settingsStorage) 
    {
        AddNewCartoonCommand = new LambdaCommand(async e => await AddNew(e), e => SelectedCartoon != null && !ParsingInProgress);
        LoadedCommand = new LambdaCommand(async e => await Loaded());
        this.mainWindow = mainWindow;
        this.parser = parser;
        this.settingsStorage = settingsStorage;
    }

    private async Task Loaded()
    {
        var data = await parser.GetCartoonList();
        foreach (var cartoon in data)
        {
            if (mainWindow.Cartoons.FirstOrDefault(c => c.Uri == cartoon.Uri) == null)
            {
                AllCartoonsExcludeAdded.Add(cartoon);
            }
        }
    }

    public async Task AddNew(object window)
    {
        ParsingInProgress = true;
        try
        {
            var newCartoon = await parser.Parse(SelectedCartoon.Name, SelectedCartoon.Uri);
            SelectedCartoon = newCartoon;
            settingsStorage.SaveCartoon(newCartoon);
        }
        catch(Exception ex)
        {
            ParsingInProgress = false;
            MessageBox.Show("Ошибка парсинга: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            (window as Window).DialogResult = true;
            ParsingInProgress = false;
        }
             
    }
}
