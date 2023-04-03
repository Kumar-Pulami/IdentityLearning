namespace IdentityLearningAPI.Configurations.MailConfig
{
    public class MailConfig
    {
        public String From { get; set; }
        public String DisplayName { get; set; }
        public String Password { get; set; }
        public String Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStarTls { get; set; }
    }
}
