namespace DotNETModernAPI.Domain.Views
{
    public class UserRolesView
    {
        public UserRolesView(Guid id, string userName, string email, bool emailConfirmed, IEnumerable<RoleVO> roles)
        {
            Id = id;
            UserName = userName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Roles = roles;
        }

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public IEnumerable<RoleVO> Roles { get; private set; }
    }

    public class RoleVO
    {
        public RoleVO(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
    }
}
