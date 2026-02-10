using Authentication.Application.Commands.CreateUserCommand;
using Authentication.Application.Commands.LogIn;
using Authentication.Application.Commands.RefreshToken;
using Authentication.Application.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        private const string RefreshTokenKey = "X-Refresh-Token";

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateUser([FromBody] CreateUserCommand command, CancellationToken ct)
        {
            return await _mediator.Send(command, ct);
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> Login(
            [FromBody] LogInCommand command,
            CancellationToken ct)
        {
            var loginResponse = await _mediator.Send(command, ct);

            if (!loginResponse.Succeeded)
            {
                return Unauthorized(new {Message = "Invalid login credentials."});
            }

            Response.Cookies.Append(
                RefreshTokenKey,
                loginResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.SpecifyKind(loginResponse.RefreshTokenExpiry, DateTimeKind.Utc)
                }
            );

            return Ok(new AuthenticationResponse
            {
                AccessToken = loginResponse.AccessToken,
                Succeeded = true
            });
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> Refresh(CancellationToken ct)
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
            {
                return BadRequest();
            }

            var refreshCommand = new RefreshTokenCommand
            {
                RefreshToken = refreshToken
            };

            var refreshResponse = await _mediator.Send(refreshCommand, ct);

            Response.Cookies.Append(
                RefreshTokenKey,
                refreshResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.SpecifyKind(refreshResponse.RefreshTokenExpiry, DateTimeKind.Utc)
                }
            );

            var authResponse = new AuthenticationResponse
            {
                AccessToken = refreshResponse.AccessToken,
                Succeeded = true
            };

            return Ok(authResponse);
        }
    }
}