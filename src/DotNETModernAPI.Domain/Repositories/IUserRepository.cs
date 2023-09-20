using DotNETModernAPI.Domain.Views;

namespace DotNETModernAPI.Domain.Repositories;

public interface IUserRepository
{
    Task<IList<UserRolesView>> List();
}
