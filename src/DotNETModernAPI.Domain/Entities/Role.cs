using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public Role(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public override string NormalizedName { get => Name.ToUpper(); }

    public void UpdateName(string name) =>
        Name = name;
}
