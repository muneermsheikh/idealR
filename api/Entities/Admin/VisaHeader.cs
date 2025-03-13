using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Wordprocessing;

namespace api.Entities.Admin
{
    public class VisaHeader:BaseEntity
    {
        [MaxLength(15), Required]
        public string VisaNo { get; set; }
        public string VisaDateH  { get; set; }   //**TODO** USE hiJRIDATE types
        public DateTime VisaDateG { get; set; }
        [Required]
        public int CustomerId   { get; set; }
        [MaxLength(50)]
        public string CustomerName { get; set; }
        public string VisaExpiryH { get; set; }
        public DateTime VisaExpiryG { get; set; }
        [MaxLength(40), Required]
        public string VisaSponsorName { get; set; }
        public ICollection<VisaItem> VisaItems { get; set; }
    }   

}