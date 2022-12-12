namespace FoxFanDownloaderCore;

public record CartoonModel
{
    public string Name { get; init; }
    public string Uri { get; init; }
    public string Image { get; init; }

    public SeasonsInfoModel SeasonsInfo { get; init; }
}


