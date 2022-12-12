namespace FoxFanDownloaderCore;

public class CartoonUpdatesChecker
{
    private readonly FoxFanParser parser;
    private readonly ISettingsStorage settingsStorage;

    public CartoonUpdatesChecker(FoxFanParser parser, ISettingsStorage settingsStorage)
    {
        this.parser = parser;
        this.settingsStorage = settingsStorage;
    }

    public async Task<bool> CheckUpdatesAndSave(CartoonModel cartoon)
    {
        bool hasUpdates = false;

        int lastServerSeasonNumber = await parser.GetLastSeasonNumberForCartoon(cartoon.Uri);
        int lastCurrentSeasonNumber = cartoon.SeasonsInfo.Seasons.Max(s => int.Parse(s.Number));
        if (lastServerSeasonNumber > lastCurrentSeasonNumber)
        {
            // has updates - new season
            for (int i = (lastCurrentSeasonNumber + 1); i <= lastServerSeasonNumber; i++)
            {
                SeasonModel newSeason = await parser.ParseSeason(cartoon.Uri, i);
                cartoon.SeasonsInfo.Seasons.Insert(0, newSeason);
            }
            hasUpdates = true;
        }
        else if (lastServerSeasonNumber == lastCurrentSeasonNumber)
        {
            SeasonModel lastServerSeason = await parser.ParseSeason(cartoon.Uri, lastCurrentSeasonNumber);
            SeasonModel lastLocalSeason = cartoon.SeasonsInfo.Seasons.OrderByDescending(s => int.Parse(s.Number))
                .FirstOrDefault();

            if (lastLocalSeason != null && lastServerSeason.Series.Count() > lastLocalSeason.Series.Count())
            {
                // has updates - new series
                cartoon.SeasonsInfo.Seasons.Remove(lastLocalSeason);
                cartoon.SeasonsInfo.Seasons.Insert(0, lastServerSeason);

                hasUpdates = true;
            }
        }

        if (hasUpdates)
        {
            settingsStorage.SaveCartoon(cartoon);
        }

        return hasUpdates;
    }

    public async Task<string[]> CheckUpdatesAndSave(IEnumerable<CartoonModel> cartoons)
    {
        var tasks = cartoons.Select(c => CheckUpdatesAndSave(c)).ToArray();

        bool[] updateResults = await Task.WhenAll(tasks);

        var cartoonsWithUpdates = cartoons
            .Where((c, index) => updateResults[index]).Select(c => c.Name)
            .ToArray();
        return cartoonsWithUpdates;
    }
}
