using AutoMapper;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Views;

namespace DotNETModernAPI.Application.Mappers;

public class UserMappers : Profile
{
    public UserMappers()
    {
        CreateMap<UserRolesView, ListUsersQueryResponse>()
            .ForMember(luqr => luqr.Id, mo => mo.MapFrom(u => u.Id))
            .ForMember(luqr => luqr.UserName, mo => mo.MapFrom(u => u.UserName))
            .ForMember(luqr => luqr.Email, mo => mo.MapFrom(u => u.Email))
            .ForMember(luqr => luqr.EmailConfirmed, mo => mo.MapFrom(u => u.EmailConfirmed));

        CreateMap<RoleVO, ListUsersRoleVO>()
            .ForMember(x => x.Id, mo => mo.MapFrom(r => r.Id))
            .ForMember(x => x.Name, mo => mo.MapFrom(r => r.Name));
    }
}
