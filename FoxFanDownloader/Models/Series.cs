using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace FoxFanDownloader.ViewModels;

public class Series : ViewModelBase
{
    public event Action<Series> OpenVideoClicked;
    public string Title { get; set; }
    public string Image { get; set; }
    [JsonIgnore]
    public string LocalImage
    {
        get
        {
            string dir = Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\images";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string local = dir + "\\" + Image.ComputeMd5Hash() + Path.GetExtension(Image);
            if (!File.Exists(local))
            {
                new WebClient().DownloadFile(Image, local);
            }
            return local;
        }
    }
    public string Uri { get; set; }
    public string Number { get; set; }
    public ICommand OpenSourceVideoCommand { get; }
    public Series()
    {
        OpenSourceVideoCommand = new LambdaCommand(e => OpenVideoClicked?.Invoke(this), e => !string.IsNullOrWhiteSpace(Uri));
    }    
}