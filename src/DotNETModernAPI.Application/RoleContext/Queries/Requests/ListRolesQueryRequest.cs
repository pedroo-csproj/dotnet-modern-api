using DotNETModernAPI.Application.RoleContext.Queries.Results;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Queries.Requests;

public class ListRolesQueryRequest : IRequest<ResultWrapper<IList<ListRolesQueryResult>>> { }
