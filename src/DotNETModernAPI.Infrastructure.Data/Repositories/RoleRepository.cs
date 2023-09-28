using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNETModernAPI.Infrastructure.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    public RoleRepository(ApplicationDataContext applicationDataContext) =>
        _applicationDataContext = applicationDataContext;

    private readonly ApplicationDataContext _applicationDataContext;

    public async Task<IList<Role>> List() =>
        await _applicationDataContext.Roles.ToListAsync();
}
