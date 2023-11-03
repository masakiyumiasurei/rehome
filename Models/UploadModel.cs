using Microsoft.AspNetCore.Http;

namespace rehome.Models
{
    public class UploadModel
    {
        public string CustomField { get; set; }
        public IFormFile PostedFile { get; set; }
    }
}