using AutoMapper;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Views;

namespace DotNETModernAPI.Application.Mappers;

public class UserMappers : Profile
{
    public UserMappers()
    {
        CreateMap<UserRolesView, ListUsersQueryResponse>()
            .ForMember(luqr => luqr.Id, mce => mce.MapFrom(urv => urv.Id))
            .ForMember(luqr => luqr.UserName, mce => mce.MapFrom(urv => urv.UserName))
            .ForMember(luqr => luqr.Email, mce => mce.MapFrom(urv => urv.Email))
            .ForMember(luqr => luqr.EmailConfirmed, mce => mce.MapFrom(urv => urv.EmailConfirmed));

        CreateMap<RoleVO, ListUsersRoleVO>()
            .ForMember(lurvo => lurvo.Id, mce => mce.MapFrom(rv => rv.Id))
            .ForMember(lurvo => lurvo.Name, mce => mce.MapFrom(rv => rv.Name));
    }
}
