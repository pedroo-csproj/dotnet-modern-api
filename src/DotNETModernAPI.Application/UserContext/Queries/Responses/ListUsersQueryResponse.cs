namespace DotNETModernAPI.Application.UserContext.Queries.Responses;

public class ListUsersQueryResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public IEnumerable<ListUsersRoleVO> Roles { get; set; }
}

public class ListUsersRoleVO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
