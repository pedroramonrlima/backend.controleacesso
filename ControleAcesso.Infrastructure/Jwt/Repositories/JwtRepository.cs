using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Models.Token;
using ControleAcesso.Infrastructure.Jwt.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ControleAcesso.Infrastructure.Jwt.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private readonly JwtSettings _jwtSettings;

        public JwtRepository(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(string group, string login, string name)
        {
            var claims = new[]
            {
                new Claim("login", login),
                new Claim("group", group),
                new Claim("name", name),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentils = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.Expires);
            JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentils
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(int id,string login, string name, string departament, string manager,string title)
        {
            var claims = new[]
            {
                new Claim("id", id.ToString()),
                new Claim("login", login),
                new Claim("name", name),
                new Claim("departament", departament),
                new Claim("manager", manager),
                new Claim("title", title),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentils = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.Expires);
            JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentils
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(Token obj)
        {
         
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            string jsonString = JsonSerializer.Serialize(obj,options);
            var claims = new List<Claim>
            {
                new Claim("Employee", jsonString)
            };
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentils = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.Expires);
            JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentils
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
