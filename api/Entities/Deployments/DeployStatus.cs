using System.ComponentModel.DataAnnotations;

namespace api.Entities.Deployments
{
    public class DeployStatus: BaseEntity
    {
        [Required]
        public string StatusName { get; set; }
        public bool isOptional { get; set; }
        [Required]
        public int Sequence { get; set; }
        [Required]
        public int NextSequence {get; set;}
        public int WorkingDaysReqdForNextStage { get; set; }
    }
}