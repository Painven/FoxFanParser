namespace FoxFanDownloader;

public record SeasonDto
{
    public string Number { get; init; }
    public SeriesDto[] Series { get; init; }
}


