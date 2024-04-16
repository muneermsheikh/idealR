using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MaxLength(8), MinLength(4)]
        public string Password { get; set; }
    }

}
