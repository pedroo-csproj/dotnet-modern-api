using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public Role(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        NormalizedName = Name.ToUpper();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
