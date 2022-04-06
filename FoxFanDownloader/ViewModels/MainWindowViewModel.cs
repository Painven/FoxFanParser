using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Multfilm> Multfilms { get; set; }
    public Multfilm SelectedMultfilm { get; set; }
    public ICommand ToolbarCommand1 { get; }
    public MainWindowViewModel()
    {
        Multfilms = new ObservableCollection<Multfilm>();
        var americanDad = new Multfilm()
        {
            Name = "American Dad",
            Seasons = new List<Season>(Enumerable.Range(1, 18).Select(n => new Season() { Number = n }))
        };
        Multfilms.Add(americanDad);
        SelectedMultfilm = Multfilms.First();
        ToolbarCommand1 = new LambdaCommand(e => { }, e => true);
    }
}

public class Multfilm : ViewModelBase
{
    public List<Season> Seasons { get; set; }
    public Season SelectedSeason { get; set; }
    public string Name { get; set; }
}

public class Season
{
    public int Number { get; set; }
}