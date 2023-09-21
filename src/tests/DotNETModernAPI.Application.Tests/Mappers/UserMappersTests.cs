using AutoMapper;
using Bogus;
using DotNETModernAPI.Application.Mappers;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Views;
using Xunit;

namespace DotNETModernAPI.Application.Tests.Mappers;

public class UserMappersTests
{
    public UserMappersTests() =>
        _mapper = new Mapper(new MapperConfiguration(mc => mc.AddProfile(new UserMappers())).CreateMapper().ConfigurationProvider);

    private readonly Mapper _mapper;

    [Fact(DisplayName = "UserRolesViewToListUsersQueryResponse - ValidData")]
    public void UserRolesViewToListUsersQueryResponse_ValidData_MustReturnMappedQueryResult()
    {
        // Arrange
        var userRoles = GenerateUserRolesView();

        // Act
        var queryResult = _mapper.Map<ListUsersQueryResponse>(userRoles);

        // Assert
        Assert.Equal(userRoles.Id, queryResult.Id);
        Assert.Equal(userRoles.UserName, queryResult.UserName);
        Assert.Equal(userRoles.Email, queryResult.Email);
        Assert.Equal(userRoles.EmailConfirmed, queryResult.EmailConfirmed);
        Assert.Equal(userRoles.Roles.First().Id, queryResult.Roles.First().Id);
        Assert.Equal(userRoles.Roles.First().Name, queryResult.Roles.First().Name);
        Assert.Equal(userRoles.Roles.Last().Id, queryResult.Roles.Last().Id);
        Assert.Equal(userRoles.Roles.Last().Name, queryResult.Roles.Last().Name);
    }

    private static RoleVO GenerateRoleVO() =>
        new(Guid.NewGuid(), "Admin");

    private static UserRolesView GenerateUserRolesView() =>
        new Faker<UserRolesView>().CustomInstantiator(f => new UserRolesView(Guid.NewGuid(), f.Internet.UserName(), f.Internet.Email(), true, new List<RoleVO>()
        {
            GenerateRoleVO(),
            GenerateRoleVO()
        }));
}
