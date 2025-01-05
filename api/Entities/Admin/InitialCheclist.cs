using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class InitialChecklist: BaseEntity
    {
        public string RaName {get; set;}
        public string IdealUsername {get; set;}
        public string DocControllerAdminAppUserId {get; set;}
        public string DocControllerAdminAppUsername {get; set;}
        public int AdminManagerEmpId {get; set;}
        public string AdminManagerAppUsername {get; set;}
        public string AdminManagerFullname {get; set;}
        public string AdminManagerDesignation {get; set;}
        public string DocControllerAdminAppUserEmail {get; set;}
        public int HrSupervisorId {get; set;}
        public string HrSupervisorname {get; set;}
        public string HRSupAppUserId {get; set;}
        public string tempPassword {get; set;}
        public bool InitialEmployeesdone {get; set;}
        public bool InitialCategoriesdone  {get; set;}
        public bool InitialCustomerListdone  {get; set;}
        public bool InitialDLdone {get; set;}
        public bool InitialCandidatesdone {get; set;}
        public bool MailSettingsdone {get; set;}

    }
}