namespace IdentityLearningAPI.Models.DTO
{
    public class AuthToken
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
