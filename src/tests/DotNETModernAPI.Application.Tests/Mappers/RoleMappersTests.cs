using AutoMapper;
using DotNETModernAPI.Application.Mappers;
using DotNETModernAPI.Application.RoleContext.Queries.Results;
using DotNETModernAPI.Domain.Entities;
using Xunit;

namespace DotNETModernAPI.Application.Tests.Mappers;

public class RoleMappersTests
{
    public RoleMappersTests() =>
        _mapper = new Mapper(new MapperConfiguration(mc => mc.AddProfile(new RoleMappers())).CreateMapper().ConfigurationProvider);

    private readonly Mapper _mapper;

    [Fact(DisplayName = "RoleToListRolesQueryResult - Valid Data")]
    public void RoleToListRolesQueryResult_ValidData_MustReturnMappedQueryResult()
    {
        // Arrange
        var role = new Role("Admin");

        // Act
        var queryResult = _mapper.Map<ListRolesQueryResult>(role);

        // Assert
        Assert.Equal(role.Id, queryResult.Id);
        Assert.Equal(role.Name, queryResult.Name);
    }
}
