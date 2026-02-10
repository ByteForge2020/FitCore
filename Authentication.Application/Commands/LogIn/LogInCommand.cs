using Authentication.Application.Dto;
using MediatR;

namespace Authentication.Application.Commands.LogIn
{
    public sealed class LogInCommand : IRequest<TokenPair>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        // public string CaptchaToken { get; set; }
    }
}