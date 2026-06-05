using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.DTOs;
using Server.Domain.Entities;

namespace Server.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(User? User, string? Error)> RegisterAsync(RegisterRequest request);
        Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request);
        Task<(AuthResponse? Response, string? Error)> RefreshTokenAsync(RefreshTokenRequest request);
        Task<(bool Success, string? Error)> VerifyEmailAsync(string token);
        Task<string?> ResendConfirmationAsync(string email);
    }
}