using Authentication.Application.Dto;
using Authentication.Application.Services.JwtService;
using MediatR;

namespace Authentication.Application.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, TokenPair>
    {
        private readonly IJwtService _jwtService;

        public RefreshTokenHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<TokenPair> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _jwtService.RefreshTokenPair(request.RefreshToken, cancellationToken);
        }
    }
}