using AutoMapper;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Saves;
using SnowrunnerMerger.Api.Models.Saves.Dtos;

namespace SnowrunnerMerger.Api.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, GroupMemberDto>();
        CreateMap<SaveGroup, GroupDto>();
    }
}