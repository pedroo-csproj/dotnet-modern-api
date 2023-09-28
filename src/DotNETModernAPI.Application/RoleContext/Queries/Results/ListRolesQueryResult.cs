namespace DotNETModernAPI.Application.RoleContext.Queries.Results;

public class ListRolesQueryResult
{
    public ListRolesQueryResult(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
}
