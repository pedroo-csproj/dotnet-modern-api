namespace DotNETModernAPI.Application.UserContext.Queries.Responses;

public class ListUsersQueryResponse
{
    public ListUsersQueryResponse(Guid id, string userName, string email, bool emailConfirmed, IEnumerable<ListUsersRoleVO> roles)
    {
        Id = id;
        UserName = userName;
        Email = email;
        EmailConfirmed = emailConfirmed;
        Roles = roles;
    }

    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public IEnumerable<ListUsersRoleVO> Roles { get; set; }
}

public class ListUsersRoleVO
{
    public ListUsersRoleVO(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
}
