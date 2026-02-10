using Authentication.Application.Commands.LogIn;
using Authentication.Application.Dto;

namespace Authentication.Application.Services.JwtService
{
    public interface IJwtService
    {
        Task<TokenPair> GenerateTokenPair(LogInCommand request, CancellationToken ct);
        Task<TokenPair> RefreshTokenPair(string refreshToken, CancellationToken ct);
    }
}