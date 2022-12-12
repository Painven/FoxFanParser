using AutoMapper;
using FoxFanDownloader.ViewModels;
using FoxFanDownloaderCore;

namespace FoxFanDownloader.AutoMapper;

public class CartoonMapperProfile : Profile
{
    public CartoonMapperProfile()
    {
        this.CreateMap<CartoonModel, Cartoon>();
        this.CreateMap<Cartoon, CartoonModel>();

        this.CreateMap<SeasonsInfoModel, SeasonsInfo>();
        this.CreateMap<SeasonsInfo, SeasonsInfoModel>();

        this.CreateMap<SeasonModel, Season>();
        this.CreateMap<Season, SeasonModel>();

        this.CreateMap<SeriesModel, Series>();
        this.CreateMap<Series, SeriesModel>();
    }
}
