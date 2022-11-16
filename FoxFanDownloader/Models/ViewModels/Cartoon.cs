using FoxFanDownloader.ViewModels;

namespace FoxFanDownloader.ViewModels;

public class Cartoon : ViewModelBase
{
    public SeasonsInfo SeasonsInfo { get; set; } = new SeasonsInfo();
    public string Name { get; set; }
    public string Image { get; set; }
    public string Uri { get; set; }
}
