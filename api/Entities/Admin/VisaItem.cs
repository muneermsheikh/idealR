using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace api.Entities.Admin
{
    public class VisaItem: BaseEntity
    {
        public int SrNo { get; set; }
        public int VisaHeaderId { get; set; }
        public string VisaCategoryArabic { get; set; }
        [Required, MinLength(5), MaxLength(50)]
        public string VisaCategoryEnglish { get; set; }
        [Required, MaxLength(15)]
        public string VisaConsulate { get; set; }
        [Required, Range(1,250)]
        public int VisaQuantity { get; set; }
        public bool VisaCanceled { get; set; }=false;
    }
}