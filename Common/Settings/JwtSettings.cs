namespace Common.Settings
{
    public class JwtSettings
    {
        public string RsaPrivateKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenShortExpirationMinutes { get; set; }
        public int RefreshTokenLongExpirationMinutes { get; set; }
        public int AdminAccessTokenExpirationMinutes { get; set; }
        public int AdminRefreshTokenExpirationMinutes { get; set; }
    }
}