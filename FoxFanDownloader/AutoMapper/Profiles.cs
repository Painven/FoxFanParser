using AutoMapper;
using FoxFanDownloader.ViewModels;

namespace FoxFanDownloader.AutoMapper;

public class CartoonMapperProfile : Profile
{
    public CartoonMapperProfile()
    {
        this.CreateMap<CartoonDto, Cartoon>();
        this.CreateMap<Cartoon, CartoonDto>();

        this.CreateMap<SeasonsInfoDto, SeasonsInfo>();
        this.CreateMap<SeasonsInfo, SeasonsInfoDto>();

        this.CreateMap<SeasonDto, Season>();
        this.CreateMap<Season, SeasonDto>();

        this.CreateMap<SeriesDto, Series>();
        this.CreateMap<Series, SeriesDto>();
    }
}
