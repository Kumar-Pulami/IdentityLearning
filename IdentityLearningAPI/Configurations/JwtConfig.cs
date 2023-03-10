namespace IdentityLearningAPI.Configurations
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }

        public String Audience { get; set; }

        public String Issuer { get; set; }
    }
}
