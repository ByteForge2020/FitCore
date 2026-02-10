using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Commands.CreateUserCommand
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly UserManager<FitCoreUser> _userManager;
        private readonly AuthenticationDbContext _authenticationDbContext;
        private readonly ILogger<CreateUserHandler> _logger;


        public CreateUserHandler(
            UserManager<FitCoreUser> userManager,
            AuthenticationDbContext authenticationDbContext,
            ILogger<CreateUserHandler> logger)
        {
            _userManager = userManager;
            _authenticationDbContext = authenticationDbContext;
            _logger = logger;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _userManager.CreateAsync(new FitCoreUser()
                    {
                        UserName = string.Join(request.FirstName, request.LastName),
                        Email = request.Email,
                        EmailConfirmed = true,
                    },                          
                    request.Password);

                await _authenticationDbContext.SaveChangesAsync(cancellationToken);

                if (userResult.Succeeded)
                {
                    await _authenticationDbContext.SaveChangesAsync(cancellationToken);
                    return "User creation succeeded.";
                }

                var errors = string.Join(". ", userResult.Errors.Select(e => $"{e.Code} - {e.Description}"));
                return $"User creation failed: {errors}";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred while creating a user");
                return $"{e.Message} {e.StackTrace}";
            }
        }
    }
}