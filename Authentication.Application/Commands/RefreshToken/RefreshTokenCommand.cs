using Authentication.Application.Dto;
using MediatR;

namespace Authentication.Application.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<TokenPair>
    {
        public string RefreshToken { get; set; }
    }
}