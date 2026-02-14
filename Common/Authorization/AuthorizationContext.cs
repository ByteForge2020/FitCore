using Microsoft.AspNetCore.Http;

namespace Common.Authorization
{
    public class AuthorizationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid ThrowOrGetAccountId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var accountIdClaim = httpContext.Request.Headers[JwtTokenClaims.AccountId].FirstOrDefault();
            if (accountIdClaim == null || !Guid.TryParse(accountIdClaim, out var accountId))
            {
                throw new InvalidOperationException("AccountId is missing or invalid");
            }

            return accountId;
        }
    }
}
