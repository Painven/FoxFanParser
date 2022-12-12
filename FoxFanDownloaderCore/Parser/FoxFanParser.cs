using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FoxFanDownloaderCore;

public partial class FoxFanParser
{
    private readonly HttpClient client;

    public FoxFanParser()
    {
        client = new HttpClient();
    }

    public async Task<CartoonModel> Parse(string name, string host)
    {
        var cartoon = new CartoonModel()
        {
            Name = name,
            Uri = host,
            SeasonsInfo = new SeasonsInfoModel()
        };
        int lastSeasonNumber = await GetLastSeasonNumberForCartoon(host);

        for (int i = lastSeasonNumber; i >= 1; i--)
        {
            var season = await ParseSeason(host, i);
            cartoon.SeasonsInfo.Seasons.Add(season);
        }

        return cartoon;
    }

    public async Task<int> GetLastSeasonNumberForCartoon(string host)
    {
        var html = await client.GetStringAsync(host);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        string lastSeason = doc.DocumentNode.SelectSingleNode("//div[@class='numberSeason']/h1").InnerText.Trim();
        int lastSeasonNumber = int.Parse(Regex.Match(lastSeason, @"^(\d+)-й Сезон").Groups[1].Value);

        return lastSeasonNumber;
    }

    public async Task<SeasonModel> ParseSeason(string host, int current_season)
    {
        var html = await client.GetStringAsync($"{host}/season.php?id={current_season}");
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var season = new SeasonModel()
        {
            Number = current_season.ToString(),
            Series = doc.DocumentNode.SelectNodes("//td[1]/a[contains(@href, 'series.php')]")
                    .Select((a, number) => new SeriesModel()
                    {
                        Title = Regex.Match(a.GetAttributeValue("title", null), @"^(.*?)\((.*?)\)(.*)$").Groups[2].Value,
                        Uri = host + "/" + a.GetAttributeValue("href", null),
                        Image = host + "/" + a.SelectSingleNode(".//img")?.GetAttributeValue("src", null),
                        Number = (number + 1).ToString()
                    }).ToArray()
        };

        return season;
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

    public async Task<CartoonModel[]> GetCartoonList()
    {
        const string HOST = "https://mult.love/";
        const string pattern = @"^Смотреть «(.*?)» \((.*?)\) онлайн на (.*?)$";

        var doc = new HtmlDocument();
        var str = await client.GetStringAsync(HOST);
        doc.LoadHtml(str);

        var cartoonItems = doc.DocumentNode.SelectNodes("//div[@id='links']/ul/li")
            .Select(li => new CartoonModel()
            {
                Name = Regex.Match(li.SelectSingleNode(".//img").GetAttributeValue("alt", null), pattern).Groups[2].Value,
                Image = HOST + li.SelectSingleNode(".//img").GetAttributeValue("src", null),
                Uri = li.SelectSingleNode("./a").GetAttributeValue("href", null)
            }).ToArray();

        return cartoonItems;
    }
}