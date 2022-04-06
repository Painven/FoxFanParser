using FoxFanDownloader.ViewModels;
using System.Collections.ObjectModel;

namespace FoxFanDownloader.ViewModels;

public class SeasonsInfo : ViewModelBase
{
    public ObservableCollection<Season> Seasons { get; set; } = new ObservableCollection<Season>();

    Season selectedSeason;
    public Season SelectedSeason
    {
        get => selectedSeason;
        set => Set(ref selectedSeason, value);
    }
}
