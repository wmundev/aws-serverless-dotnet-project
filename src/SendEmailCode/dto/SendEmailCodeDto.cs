using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SendEmailCode.dto
{
    public class SendEmailCodeDto
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}