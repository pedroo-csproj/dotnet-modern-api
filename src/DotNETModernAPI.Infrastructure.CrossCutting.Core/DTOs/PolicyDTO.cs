namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;

public class PolicyDTO
{
    public IEnumerable<string> Users { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
