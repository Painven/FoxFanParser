using FoxFanDownloader.ViewModels;
using System.Diagnostics;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class Series : ViewModelBase
{
    public string Title { get; set; }
    public string Image { get; set; }
    public string Uri { get; set; }
    public string Number { get; set; }

    public ICommand ClickedCommand { get; }
    public Series()
    {
        ClickedCommand = new LambdaCommand(e =>
        {
            var escapedUrl = Uri.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(escapedUrl) { UseShellExecute = true });
        }, e => !string.IsNullOrWhiteSpace(Uri));
    }
}