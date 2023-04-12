using Microsoft.OpenApi.Any;
using System.Diagnostics.CodeAnalysis;

namespace IdentityLearningAPI.Models.DTO.Response
{
    public class Response<T>
    {
        public bool Success { get; set; }
        
        [AllowNull]
        public T? Payload { get; set; }

        public List<string>? Error { get; set; } = null;  
    }
}