namespace IdentityLearningAPI.Models.DTO
{
    public class Mail
    {
        public List<string> To { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public List<IFormFile>? Attachments { get; set; }
    }
}
