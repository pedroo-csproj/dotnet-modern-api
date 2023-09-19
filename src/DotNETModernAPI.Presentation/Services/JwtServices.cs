using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNETModernAPI.Presentation.Services;

public class JwtServices
{
    public JwtServices(IConfiguration configuration) =>
        _configuration = configuration;

    private readonly IConfiguration _configuration;

    //TODO: apply the single responsability principle
    public string Generate(IList<Claim> claims)
    {
        claims.Add(new Claim("aud", _configuration["JWT:ValidAudience"]));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            claims,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddMinutes(2),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
