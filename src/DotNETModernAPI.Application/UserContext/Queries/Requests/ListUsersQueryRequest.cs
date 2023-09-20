using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Queries.Requests;

public class ListUsersQueryRequest : IRequest<ResultWrapper<IEnumerable<ListUsersQueryResponse>>> { }
