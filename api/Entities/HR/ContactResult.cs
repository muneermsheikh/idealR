using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api.Entities.HR
{
    public class ContactResult
    {
        [Key]
        public string Status { get; set; }
        public string IsActive { get; set; }
    }
}