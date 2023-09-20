using AutoMapper;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Views;

namespace DotNETModernAPI.Application.Mappers;

public class UserMappers : Profile
{
    public UserMappers()
    {
        CreateMap<UserRolesView, ListUsersQueryResponse>()
            .ForMember(luqr => luqr.Id, mo => mo.MapFrom(urv => urv.Id))
            .ForMember(luqr => luqr.UserName, mo => mo.MapFrom(urv => urv.UserName))
            .ForMember(luqr => luqr.Email, mo => mo.MapFrom(urv => urv.Email))
            .ForMember(luqr => luqr.EmailConfirmed, mo => mo.MapFrom(urv => urv.EmailConfirmed));

        CreateMap<RoleVO, ListUsersRoleVO>()
            .ForMember(lurvo => lurvo.Id, mo => mo.MapFrom(rv => rv.Id))
            .ForMember(lurvo => lurvo.Name, mo => mo.MapFrom(rv => rv.Name));
    }
}
