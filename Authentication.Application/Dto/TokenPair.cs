namespace Authentication.Application.Dto
{
    public class TokenPair
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public bool Succeeded { get; set; }
    }
}