namespace FoxFanDownloader;

public record CartoonDto
{
    public string Name { get; init; }
    public string Uri { get; init; }
    public string Image { get; init; }

    public SeasonsInfoDto SeasonsInfo { get; init; }
}


