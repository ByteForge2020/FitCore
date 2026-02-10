using Authentication.Application.Dto;
using Authentication.Application.Services.JwtService;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Commands.LogIn
{
    public class LogInHandler(
        UserManager<FitCoreUser> userManager,
        SignInManager<FitCoreUser> signInManager,
        IJwtService jwtService,
        AuthenticationDbContext authenticationDbContext,
        IMediator mediator)
        : IRequestHandler<LogInCommand, TokenPair>
    {

        public async Task<TokenPair> Handle(LogInCommand request, CancellationToken cancellationToken)
        {
            var tokens = await jwtService.GenerateTokenPair(request, cancellationToken);
            
            return tokens;
        }
    }
}