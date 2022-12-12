namespace FoxFanDownloaderCore;

public record SeasonModel
{
    public string Number { get; init; }
    public SeriesModel[] Series { get; init; }
}


