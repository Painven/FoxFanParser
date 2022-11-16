using FoxFanDownloader.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class SeasonsInfo : ViewModelBase
{
    public event Action OnRefresSeasonsClick;

    public ObservableCollection<Season> Seasons { get; set; } = new ObservableCollection<Season>();

    Season selectedSeason;
    public Season SelectedSeason
    {
        get => selectedSeason;
        set => Set(ref selectedSeason, value);
    }

    public ICommand RefreshSeasonsCommand { get; }

    bool isRefreshInProgress;
    public bool IsRefreshInProgress
    {
        get => isRefreshInProgress;
        set => Set(ref isRefreshInProgress, value);
    }

    public SeasonsInfo()
    {
        RefreshSeasonsCommand = new LambdaCommand(e => OnRefresSeasonsClick?.Invoke(), e => !IsRefreshInProgress);
    }

    public void SelectLastSeason()
    {
        SelectedSeason = Seasons.MaxBy(s => int.Parse(s.Number));
    }
}
