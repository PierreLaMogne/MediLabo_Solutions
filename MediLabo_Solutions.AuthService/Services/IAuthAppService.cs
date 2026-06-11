using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace MediLabo_Solutions.AuthService.Services
{
    public interface IAuthAppService
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    }
}
