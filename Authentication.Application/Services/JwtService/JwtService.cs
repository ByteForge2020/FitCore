using Authentication.Application.Commands.LogIn;
using Authentication.Application.Dto;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Entities;
using Common.Authorization;
using Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Authentication.Application.Services.JwtService
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        private readonly AuthenticationDbContext _authenticationDbContext;
        private readonly UserManager<FitCoreUser> _userManager;
        private readonly SignInManager<FitCoreUser> _signInManager;

        public JwtService(
            IOptions<JwtSettings> options,
            AuthenticationDbContext authenticationDbContext,
            UserManager<FitCoreUser> userManager,
            SignInManager<FitCoreUser> signInManager)
        {
            _authenticationDbContext = authenticationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = options.Value;
        }

        //for login. valid refresh is not required 
        public async Task<TokenPair> GenerateTokenPair(LogInCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return new TokenPair {Succeeded = false};

            var userClaims = GetClaimsFromUser(user).ToList();

            var tokenPair = new TokenPair()
            {
                AccessToken = GenerateToken(userClaims, true),
                RefreshToken = GenerateToken(userClaims, false),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                Succeeded = true
            };
            user.RefreshToken = tokenPair.RefreshToken;
            user.RefreshTokenExpires = tokenPair.RefreshTokenExpiry;
            user.RefreshTokenCreated = DateTime.UtcNow;
            await _authenticationDbContext.SaveChangesAsync(ct);
            return tokenPair;
        }

        //for refresh. valid refresh is required 
        public async Task<TokenPair> RefreshTokenPair(string refreshToken, CancellationToken ct)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(refreshToken);

            var userEmail = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            if (userEmail is null)
            {
                return new TokenPair {Succeeded = false};
            }

            // for refreshing
            var user = await _authenticationDbContext.FitCoreUsers
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.Email == userEmail, ct);
            if (user is null)
            {
                throw new InvalidOperationException("No token to refresh for this user");
            }

            // var user = await _authenticationDbContext.SchedulerUsers
            //     .FirstOrDefaultAsync(x => x.Email == userEmail, ct);

            if (user.RefreshToken != refreshToken)
            {
                throw new InvalidOperationException("Token is not valid");
            }

            var now = DateTime.UtcNow;

            if (user.RefreshTokenExpires <= now)
            {
                user.RefreshToken = null;
                return new TokenPair {Succeeded = false};
            }

            var userClaims = GetClaimsFromUser(user).ToList();

            var newTokenPair = new TokenPair()
            {
                AccessToken = GenerateToken(claims: userClaims, shortTermToken: true),
                RefreshToken = GenerateToken(claims: userClaims, shortTermToken: false),
                RefreshTokenExpiry = DateTime.Now.AddDays(7),
                Succeeded = true
            };

            user.RefreshToken = newTokenPair.RefreshToken;
            user.RefreshTokenExpires = newTokenPair.RefreshTokenExpiry;
            user.RefreshTokenCreated = DateTime.Now;
            await _authenticationDbContext.SaveChangesAsync(ct);

            return newTokenPair;
        }

        private string GenerateToken(IEnumerable<Claim> claims, bool shortTermToken)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(
                source:
                Convert.FromBase64String(_settings.RsaPrivateKey), //TODO: move to a safer place(AWS Secrets Manager?)
                bytesRead: out int _);

            var credentials = new SigningCredentials(
                    key: new RsaSecurityKey(rsa),
                    algorithm: SecurityAlgorithms.RsaSha256
                )
                {CryptoProviderFactory = new CryptoProviderFactory() {CacheSignatureProviders = false}};

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = shortTermToken ? DateTime.Now.AddMinutes(30) : DateTime.Now.AddDays(7),
                SigningCredentials = credentials,
                Issuer = "jwt-fitcore",
                Audience = "jwt-fitcore"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static IEnumerable<Claim> GetClaimsFromUser(FitCoreUser user)
        {
            return new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtTokenClaims.UserId, user.Id),
                new(JwtTokenClaims.AccountId, user.AccountId.ToString()),
            };
        }
    }
}