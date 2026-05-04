using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fitness_Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Fitness_Api.UniversalMethods;

public class JwtGenerator
{
    private readonly string _secretKey;

    public JwtGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] ?? throw new Exception("JWT key not found");
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new("userId", user.id_User.ToString()),
            new("roleId", user.Role_Id.ToString()),
            new("email", user.Email),
            new("name", user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.Client_Id.HasValue)
        {
            claims.Add(new Claim("clientId", user.Client_Id.Value.ToString()));
        }

        if (user.Trainer_Id.HasValue)
        {
            claims.Add(new Claim("trainerId", user.Trainer_Id.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
