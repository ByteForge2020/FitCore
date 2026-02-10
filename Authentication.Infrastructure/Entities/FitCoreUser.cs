using Microsoft.AspNetCore.Identity;


namespace Authentication.Infrastructure.Entities
{
    public class FitCoreUser : IdentityUser
    {
        public Guid AccountId  { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public bool RefreshTokenIsLongLived { get; set; }
        public DateTime RefreshTokenCreated { get; set; }
    }
}