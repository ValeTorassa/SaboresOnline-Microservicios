using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthAPI.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // Opciones JWT inyectadas desde la configuración
        private readonly JwtOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        // Método para generar un token JWT a partir de un objeto ApplicationUser
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            // Manejador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // Clave secreta utilizada para firmar el token, convertida a bytes desde la configuración
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            // Lista de reclamaciones (claims) que se incluirán en el token
            var claimList = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
            };

            // claimnames es cuando vos las definis y claimtypes es cuando usas las de identity
            claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Descripción del token que incluye configuraciones como emisor, audiencia, reclamaciones, tiempo de expiración, etc.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7),  // Token expira después de 7 días
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Creación del token utilizando el descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Escritura del token como una cadena
            return tokenHandler.WriteToken(token);
        }
    }
}
