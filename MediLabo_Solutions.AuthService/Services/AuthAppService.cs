using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediLabo_Solutions.AuthService.Services
{
    public class AuthAppService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration) : IAuthAppService
    {
        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            // Validation des entrées
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                throw new BadRequestException("Nom d'utilisateur ou mot de passe manquant.");

            // Recherche de l'utilisateur
            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new UnauthorizedException("Nom d'utilisateur ou mot de passe incorrect.");

            // Vérification du verrouillage
            if (await userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);
                throw new UnauthorizedException($"Compte temporairement bloqué. Réessayez après {lockoutEnd?.LocalDateTime:HH:mm}.");
            }

            // Tentative de connexion
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);
                    throw new UnauthorizedException($"Trop de tentatives échouées. Compte bloqué jusqu'à {lockoutEnd?.LocalDateTime:HH:mm}.");
                }
                throw new UnauthorizedException("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Récupération du rôle de l'utilisateur
            var roles = await userManager.GetRolesAsync(user);

            // Génération du token JWT
            var token = await GenerateJwtToken(user, roles);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Username = user.UserName!
            };
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            // Ajout des rôles en tant que claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds
            );
        }
    }
}
