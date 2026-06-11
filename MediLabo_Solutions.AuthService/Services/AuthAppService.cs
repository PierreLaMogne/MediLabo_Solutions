using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediLabo_Solutions.AuthService.Services
{
    public class AuthAppService(IConfiguration configuration) : IAuthAppService
    {
        private const int MaxFailedAttempts = 5;
        private const int LockoutDurationMinutes = 15;
        private static int _failedAttempts = 0;
        private static DateTime? _lockoutEnd = null;

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            // Vérification d'un blocage du compte
            if (_lockoutEnd.HasValue && _lockoutEnd.Value > DateTime.UtcNow)
                throw new UnauthorizedException($"Compte temporairement bloqué. Réessayez après {_lockoutEnd.Value:HH:mm}.");

            // Validation du Username et du Password
            var adminUsername = configuration["AdminCredentials:Username"];
            var adminPassword = configuration["AdminCredentials:Password"];

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                throw new UnauthorizedException("Nom d'utilisateur ou mot de passe manquant.");

            if (request.Username != adminUsername || request.Password != adminPassword)
            {
                _failedAttempts++;
                if (_failedAttempts >= MaxFailedAttempts)
                {
                    _lockoutEnd = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _failedAttempts = 0;
                    throw new UnauthorizedException($"Trop de tentatives échouées. Compte bloqué jusqu'à {_lockoutEnd.Value:HH:mm}.");
                }
                throw new UnauthorizedException("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Réinitialisation des tentatives en cas de succès
            _failedAttempts = 0;
            _lockoutEnd = null;

            // Génération du token JWT
            var token = GenerateJwtToken(request.Username);

            return await Task.FromResult(new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Username = request.Username
            });
        }

        private JwtSecurityToken GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Praticien")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );
        }
    }
}
