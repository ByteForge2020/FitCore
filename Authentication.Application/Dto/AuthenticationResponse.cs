namespace Authentication.Application.Dto
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }
        public bool Succeeded { get; init; }
    }
}