using System.ComponentModel.DataAnnotations;

namespace api.Entities.HR
{
    public class UserPhone: BaseEntity
    {
        public UserPhone()
        {
        }

        public UserPhone(string v, object value1, object value2, object value3)
        {
        }

        public int CandidateId { get; set; }        //to disable creating a column CandidateId1 during
        //public string PhoneNo { get; set; }   //user will have only mobile no
        [MinLength(10), MaxLength(15), Required]
        public string MobileNo { get; set; }
        public bool IsMain {get; set;}=false;
        public bool IsValid { get; set; }=true;
        public string Remarks { get; set; }
    }
}