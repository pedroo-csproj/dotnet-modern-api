using AutoMapper;
using DotNETModernAPI.Application.RoleContext;
using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Application.RoleContext.Queries.Requests;
using DotNETModernAPI.Application.RoleContext.Queries.Results;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DotNETModernAPI.Application.Tests.RoleContext;

public class RoleHandlersTests
{
    public RoleHandlersTests()
    {
        var roleStore = new Mock<IRoleStore<Role>>();
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _roleValidator = new Mock<IValidator<Role>>();
        _policies = new Mock<IOptions<PoliciesDTO>>();
        _roleRepository = new Mock<IRoleRepository>();
        _roleServices = new Mock<RoleServices>(_roleManager.Object, _roleValidator.Object, _policies.Object, _roleRepository.Object);
        _mapper = new Mock<IMapper>();
        _roleHandlers = new RoleHandlers(_roleServices.Object, _mapper.Object);
    }

    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IValidator<Role>> _roleValidator;
    private readonly Mock<IOptions<PoliciesDTO>> _policies;
    private readonly Mock<IRoleRepository> _roleRepository;
    private readonly Mock<RoleServices> _roleServices;
    private readonly Mock<IMapper> _mapper;
    private readonly RoleHandlers _roleHandlers;

    [Fact(DisplayName = "List - Valid Query")]
    public async void List_ValidQuery_MustReturnNoError()
    {
        // Arrange
        var queryRequest = new ListRolesQueryRequest();
        var roles = new List<Role>()
        {
            new Role("Admin"),
            new Role("Instructor"),
        };
        var listResult = new ResultWrapper<IList<Role>>(roles);
        var mappedQueryResult = roles.Select(r => new ListRolesQueryResult(r.Id, r.Name)).ToList();

        _roleServices.Setup(rs => rs.List()).Returns(Task.FromResult(listResult));
        _mapper.Setup(m => m.Map<IList<ListRolesQueryResult>>(listResult.Data)).Returns(mappedQueryResult);

        // Act
        var handleResult = await _roleHandlers.Handle(queryRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Equal(2, handleResult.Data.Count());
        Assert.Equal(roles.First().Id, handleResult.Data.First().Id);
        Assert.Equal(roles.First().Name, handleResult.Data.First().Name);
        Assert.Equal(roles.Last().Id, handleResult.Data.Last().Id);
        Assert.Equal(roles.Last().Name, handleResult.Data.Last().Name);

        _roleServices.Verify(rs => rs.List(), Times.Once);
        _mapper.Verify(m => m.Map<IList<ListRolesQueryResult>>(listResult.Data), Times.Once);
    }

    [Fact(DisplayName = "Create - Invalid Data")]
    public async void Create_InvalidData_MustReturnError()
    {
        // Arrange
        var commandRequest = GenerateCreateRoleCommandRequest();
        var errorCode = EErrorCode.RoleAlreadyExists;

        _roleServices.Setup(rs => rs.Create(commandRequest.Name)).Returns(Task.FromResult(new ResultWrapper<Guid>(errorCode)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.False(handleResult.Success);
        Assert.Equal(errorCode, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Null(handleResult.Data);

        _roleServices.Verify(rs => rs.Create(commandRequest.Name), Times.Once);
    }

    [Fact(DisplayName = "Create - Valid Data")]
    public async void Create_ValidData_MustReturnNoError()
    {
        // Arrange
        var commandRequest = GenerateCreateRoleCommandRequest();
        var roleId = Guid.NewGuid();

        _roleServices.Setup(rs => rs.Create(commandRequest.Name)).Returns(Task.FromResult(new ResultWrapper<Guid>(roleId)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Equal(roleId, handleResult.Data.Id);

        _roleServices.Verify(rs => rs.Create(commandRequest.Name), Times.Once);
    }

    [Fact(DisplayName = "AddClaimsToRole - Invalid Data")]
    public async void AddClaimsToRole_InvalidData_MustReturnInvalidPolicy()
    {
        // Arrange
        var claims = new List<string>() { "roles.listPolicies", "roles.create" };
        var commandRequest = new AddClaimsToRoleCommandRequest(Guid.NewGuid(), claims);

        _roleServices.Setup(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>())).Returns(Task.FromResult(new ResultWrapper(EErrorCode.InvalidPolicy)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.False(handleResult.Success);
        Assert.Equal(EErrorCode.InvalidPolicy, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _roleServices.Verify(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact(DisplayName = "AddClaimsToRole - Valid Data")]
    public async void AddClaimsToRole_ValidData_MustReturnNoError()
    {
        // Arrange
        var claims = new List<string>() { "roles.listPolicies", "roles.create" };
        var commandRequest = new AddClaimsToRoleCommandRequest(Guid.NewGuid(), claims);

        _roleServices.Setup(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>())).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _roleServices.Verify(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    private static CreateRoleCommandRequest GenerateCreateRoleCommandRequest() =>
        new("Admin");
}
