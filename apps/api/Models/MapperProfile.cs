using AutoMapper;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Saves;
using SnowrunnerMerger.Shared.DTOs.Auth;
using SnowrunnerMerger.Shared.DTOs.Groups;

namespace SnowrunnerMerger.Api.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, GroupMemberDto>();
        CreateMap<User, UserDto>();
        CreateMap<SaveGroup, GroupDto>();
        CreateMap<StoredSaveInfo, StoredSaveDto>();
    }
}