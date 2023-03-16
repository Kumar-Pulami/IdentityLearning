namespace IdentityLearningAPI.Models
{
    public class AuthToken
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
