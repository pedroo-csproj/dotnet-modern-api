using DotNETModernAPI.Domain.Entities;

namespace DotNETModernAPI.Domain.Repositories;

public interface IRoleRepository
{
    Task<IList<Role>> List();
}
