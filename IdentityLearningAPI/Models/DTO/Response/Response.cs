using Microsoft.OpenApi.Any;

namespace IdentityLearningAPI.Models.DTO.Response
{
    public class Response
    {
        public bool Success { get; set; }
        public AnyType Payload { get; set; }
        public List<string> Error { get; set; } = null;  
    }
}
