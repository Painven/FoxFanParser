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
    public string Title { get; set; }
    public string Image { get; set; }
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
        OpenSourceVideoCommand = new LambdaCommand(OpenSourceVideo, e => !string.IsNullOrWhiteSpace(Uri));
    }

    private async void OpenSourceVideo(object obj)
    {
        PlayerJsonRoot data = await GetSource();
        if (data != null)
        {
            string exePath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
            string subtitleFileName = Path.GetFileName(data.subtitle);
            string workDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\subtitles";
            string localSutitlesFile = Path.Combine(workDirectory, subtitleFileName);
            string command = $"-vvv \"{data.file}\" --sub-file \"{subtitleFileName}\"";

            if (!Directory.Exists(workDirectory))
            {
                Directory.CreateDirectory(workDirectory);
            }
            if (!File.Exists(localSutitlesFile))
            {
                await (new WebClient()).DownloadFileTaskAsync(data.subtitle, localSutitlesFile);
            }

            var process = new ProcessStartInfo(exePath, command);
            process.WorkingDirectory = workDirectory;
            Process.Start(process);
        }
    }
    private async Task<PlayerJsonRoot> GetSource()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "*/*");

        var doc = new HtmlDocument();
        doc.LoadHtml(await client.GetStringAsync(Uri));

        string host = new Uri(Uri).Host.TrimEnd('/');
        if (!host.StartsWith("https"))
        {
            host = "https://" + host;
        }
        string? engSubSource = doc.DocumentNode.SelectSingleNode("//a[contains(text(), 'анг')]")?.GetAttributeValue("href", null);
        if (!string.IsNullOrWhiteSpace(engSubSource))
        {
            doc = new HtmlDocument();
            string detailsPageUri = $"{host}/{engSubSource}";
            doc.LoadHtml(await client.GetStringAsync(detailsPageUri));

            //var player = new Playerjs({id:'player616', file:'https://movies1.fox-fan.tv/video1/UZo-GznPGsPi65z9rxYFVQ/1649315780/americandad/6/original/616.mp4',comment:'616 серия', poster:'https://americandad.fox-fan.tv/seasons/616_big.jpg',subtitle:'https://americandad.fox-fan.tv/captions/eng/6/616.vtt'});
            string playerScriptString = Regex.Match(doc.DocumentNode.OuterHtml, @"var player = new Playerjs\((.*?)\)").Groups[1].Value;
            var player = JsonConvert.DeserializeObject<PlayerJsonRoot>(playerScriptString);

            return player;

        }

        return null;
    }
    private class PlayerJsonRoot
    {
        public string id { get; set; }
        public string file { get; set; }
        public string comment { get; set; }
        public string poster { get; set; }
        public string subtitle { get; set; }
    }

}