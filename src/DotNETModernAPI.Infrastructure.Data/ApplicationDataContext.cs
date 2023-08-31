using DotNETModernAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNETModernAPI.Infrastructure.Data;

public class ApplicationDataContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) =>
        base.OnModelCreating(builder);
}
