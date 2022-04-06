using FoxFanDownloader.ViewModels;
using HtmlAgilityPack;
using Newtonsoft.Json;
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
}