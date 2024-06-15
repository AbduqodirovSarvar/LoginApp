using AutoMapper;
using LoginApp.DB.Entities;
using LoginApp.Models.ViewModels;

namespace LoginApp.Services;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<User, UserViewModel>()
                .ReverseMap();
        CreateMap<Enum, EnumViewModel>()
            .ForMember(x => x.Id, y => y.MapFrom(z => Convert.ToInt32(z)))
            .ForMember(x => x.Name, y => y.MapFrom(z => z.ToString()));
    }
}
