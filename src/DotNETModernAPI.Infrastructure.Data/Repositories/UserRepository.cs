using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace DotNETModernAPI.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    public UserRepository(ApplicationDataContext applicationDataContext) =>
        _applicationDataContext = applicationDataContext;

    private readonly ApplicationDataContext _applicationDataContext;

    public async Task<IList<UserRolesView>> List() =>
        await _applicationDataContext.Users
            .Join(
                _applicationDataContext.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                _applicationDataContext.Roles,
                uur => uur.userRole.RoleId,
                r => r.Id,
                (userUserRole, role) => new { userUserRole, role })
            .GroupBy(ruur =>
                new { ruur.userUserRole.user.Id, ruur.userUserRole.user.UserName, ruur.userUserRole.user.Email, ruur.userUserRole.user.EmailConfirmed },
                ruur => new { ruur.role.Id, ruur.role.Name },
                (u, r) => new UserRolesView(u.Id, u.UserName, u.Email, u.EmailConfirmed, r.Select(r => new RoleVO(r.Id, r.Name)).ToList())).ToListAsync();
}
