using FoxFanDownloader.ViewModels;
using System.Collections.ObjectModel;

namespace FoxFanDownloader.ViewModels;

public class Season : ViewModelBase
{
    public string Number { get; set; }
    public ObservableCollection<Series> Series { get; set; } = new ObservableCollection<Series>();
}
