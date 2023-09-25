namespace DotNETModernAPI.Application.RoleContext.Commands.Results;

public class CreateRoleCommandResult
{
    public CreateRoleCommandResult(Guid id) =>
        Id = id;

    public Guid Id { get; private set; }
}
