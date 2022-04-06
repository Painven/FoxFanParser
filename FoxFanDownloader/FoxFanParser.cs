using FoxFanDownloader.ViewModels;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
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
    public async Task<Multfilm> Parse()
    {
        var mult = new Multfilm();

        mult.Name = "American Dad";
        mult.SeasonsInfo = new SeasonsInfo();

        string HOST = "https://americandad.fox-fan.tv/";

        int total = 18;


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
       return mult;
    }
}