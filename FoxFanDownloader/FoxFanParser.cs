using FoxFanDownloader.ViewModels;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoxFanDownloader;

public class FoxFanParser
{
    readonly HttpClient client;
    public FoxFanParser()
    {
        client = new HttpClient();
    }
    public async Task<Multfilm> Parse(string name, string host, int seasons)
    {
        var mult = new Multfilm();

        mult.Name = name;
        mult.SeasonsInfo = new SeasonsInfo();

        string HOST = host;

        int total = seasons;


        for (int current_season = 1; current_season <= total; current_season++)
        {
            var uri = $"{HOST}season.php?id={current_season}";
            var html = await client.GetStringAsync(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var season = new Season()
            {
                Number = current_season.ToString(),
                Series = new ObservableCollection<Series>(doc.DocumentNode.SelectNodes("//td[1]/a[contains(@href, 'series.php')]")
                        .Select((a,number) => new Series()
                        {
                            Title = Regex.Match(a.GetAttributeValue("title", null), @"^(.*?)\((.*?)\)(.*)$").Groups[2].Value,
                            Uri = HOST + a.GetAttributeValue("href", null),
                            Image = HOST + a.SelectSingleNode(".//img")?.GetAttributeValue("src", null),
                            Number = (number + 1).ToString()
                        }))
            };

            mult.SeasonsInfo.Seasons.Add(season);
        }

        string jsonName = $"mult_{name.ToLower().Replace(" ", "_")}.json";
        File.WriteAllText(jsonName, JsonConvert.SerializeObject(mult));

        return mult;
    }
    public async Task<PlayerJsonRoot> GetSourceVideoFromUri(string uri)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "*/*");

        var doc = new HtmlDocument();
        doc.LoadHtml(await client.GetStringAsync(uri));

        string host = new Uri(uri).Host.TrimEnd('/');
        if (!host.StartsWith("https"))
        {
            host = "https://" + host;
        }
        string? engSubSource1 = doc.DocumentNode.SelectSingleNode("//a[contains(text(), 'анг')]")?.GetAttributeValue("href", null);
        string? engSubSource2 = doc.DocumentNode.SelectSingleNode("//a[contains(text(), 'eng')]")?.GetAttributeValue("href", null);
        string? engSubSource = new[] { engSubSource1, engSubSource2 }.FirstOrDefault(sub => !string.IsNullOrWhiteSpace(sub));

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
    public class PlayerJsonRoot
    {
        public string id { get; set; }
        public string file { get; set; }
        public string comment { get; set; }
        public string poster { get; set; }
        public string subtitle { get; set; }
    }
}