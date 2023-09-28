using AutoMapper;
using DotNETModernAPI.Application.RoleContext.Queries.Results;
using DotNETModernAPI.Domain.Entities;

namespace DotNETModernAPI.Application.Mappers;

public class RoleMappers : Profile
{
    public RoleMappers()
    {
        CreateMap<Role, ListRolesQueryResult>()
            .ForMember(lrqs => lrqs.Id, mce => mce.MapFrom(r => r.Id))
            .ForMember(lrqs => lrqs.Name, mce => mce.MapFrom(r => r.Name));
    }
}
