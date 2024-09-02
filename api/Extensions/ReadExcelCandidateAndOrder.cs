using System.Data;
using System.Data.Common;
using api.Data;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Entities.HR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Extensions
{
    public static class ReadExcelCandidateAndOrder
    {     
        public static async Task<string> ReadCandidateDataExcelFile(this DataContext context, string filePath, string Username)
        {
            // var professionId = label in row2, col3; data in row2, col4
            //  var ProfessionName = label in row2, col 5, data in row2, col6
            //  column titles in row 3 data starts from row 4
            var strError="";
            int rowTitle=2;
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows=0, columns=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["Sheet1"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    strError = ex.Message;
                    return strError;
                }
                
                int intProfessionId = 0, intProfessionName=0, intAssociateId=0, intApplicationNo=0;
                int intCandidateName=0, intEmail=0, intAlternateEmail=0, intAlternatePhone=0, intDob=0;
                int intMobileNo=0, intCurrentLocation=0, intPPNo=0, intAadharNo=0;
                int intEducation=0, intGender=0, intAge=0, intAddress=0;
    
                var DateRegistered = DateTime.Now;
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value.ToString();
                    switch (colTitle.ToLower()) {
                        case "applicationno": case "application no": intApplicationNo=col;break;
                        case "candidatename" :case "name": intCandidateName=col; break;
                        case "professionid": case "profession id":  intProfessionId=col;break;
                        case "categoryname": case "category name": case "profession name": intProfessionName=col;break;
                        case "agentid": case "agent id": case "associate id": case "associateid": intAssociateId=col;break;
                        case "emailid": case "email id": intEmail=col;break;
                        case "alternateemailid": case "alternate email": case "alternte emailid": intAlternateEmail=col;break;
                        case "alternatephone": intAlternatePhone=col;break;
                        case "dob": case "dateofbirth":intDob=col;break;
                        case "mobileno": case "mobile no": case "mobile no.": intMobileNo=col;break;
                        case "currentlocation" : case "current location": case "city": intCurrentLocation=col;break;
                        case "education": case "course": case "course1" : intEducation=col;break;
                        case "gender": intGender=col;break;
                        case "age": intAge=col;break;
                        case "address": intAddress=col;break;
                        case "ppno": case "pp no": case "passport": case "passportno": case "passport no": intPPNo=col;break;
                        case "aadhar": case "aadharno": case "adhaarno": case "aadhar no": intAadharNo=col;break;
                        default:break;
                    }
                }
                
                string CandidateName = "", EmailId = "", AlternateEmailId = "", AlternatePhone = "", DOB = "";
                string AssociateName = "", Source="", PassportNo="", AadharNo="";
                string MobileNo = "", CurrentLocation = "";
                string Education = "", Gendr = "", Age = "", Address = "", ProfessionName="";
                int ProfessionId = 0, AssociateId = 0, ApplicationNo=0;
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    var vappno = worksheet.Cells[row, intApplicationNo].Value.ToString();
                    ApplicationNo = Convert.ToInt32(vappno);
                    CandidateName = intCandidateName == 0 ? "" : worksheet.Cells[row, intCandidateName].Value.ToString() ?? "";
                    EmailId = intEmail == 0 ? "" : worksheet.Cells[row, intEmail].Value.ToString() ?? "";
                    AlternateEmailId = intAlternateEmail == 0 ? "" : worksheet.Cells[row, intAlternateEmail].Value.ToString() ?? "";
                    AlternatePhone = intAlternatePhone == 0 ? "" : worksheet.Cells[row, intAlternatePhone].Value.ToString() ?? "";
                    DOB = intDob == 0 ? "" : worksheet.Cells[row, intDob].Value.ToString() ?? "";
                    MobileNo = intMobileNo == 0 ? "": worksheet.Cells[row, intMobileNo].Value.ToString()  ?? "";
                    Address = intAddress == 0 ? "": worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    CurrentLocation = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCurrentLocation].Value.ToString() ?? "";
                    Education = intEducation == 0 ? "" : worksheet.Cells[row, intEducation].Value.ToString() ?? "";
                    Gendr = intGender == 0 ? "Male" : worksheet.Cells[row, intGender].Value.ToString() ?? "Male";
                    Gendr = Gendr == "m" ? "Male" : "Female";
                    Age =  intAge == 0 ? "" : worksheet.Cells[row, intAge].Value.ToString() ?? "";
                    Address = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    PassportNo = intPPNo == 0 ? "" : worksheet.Cells[row, intPPNo].Value.ToString() ?? "";
                    AadharNo = intAadharNo == 0 ? "" : worksheet.Cells[row, intAadharNo].Value.ToString() ?? "";
                    if(intProfessionId > 0) {
                        ProfessionId = Convert.ToInt32(worksheet.Cells[row, intProfessionId].Value);
                        ProfessionName = await context.GetProfessionNameFromId(ProfessionId);
                    }
                    if(intAssociateId > 0) {
                        AssociateId = Convert.ToInt32(worksheet.Cells[row, intAssociateId].Value);
                        AssociateName = await context.CustomerNameFromId(AssociateId);
                    }

                    if (!DateTime.TryParse(DOB, out DateTime dob))
                    {
                        dob = new DateTime();
                        if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                    }
                    
                    var appno = await context.Candidates.MaxAsync(x => x.ApplicationNo);
                    if(appno == 0) appno=1000;
                    
                    var newCandidate = new Candidate
                    {
                        ApplicationNo = ApplicationNo == 0 ? ++appno : ApplicationNo,
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        FirstName = CandidateName,
                        Source = Source,
                        DOB = dob,
                        Address = Address,
                        City = CurrentLocation,
                        Email = EmailId,
                        Created = DateRegistered,
                        UserPhones = new List<UserPhone>{new() {MobileNo=MobileNo, IsMain=true, IsValid=true, Remarks="imported thru XLS"}},
                        UserProfessions = new List<UserProfession>{new() {ProfessionId=ProfessionId, IsMain=true, ProfessionName=ProfessionName }}
                    };

                    if(!string.IsNullOrEmpty(AlternatePhone)) newCandidate.UserPhones.Add(new() {MobileNo=AlternatePhone, IsValid=true, IsMain=false});

                    context.Entry(newCandidate).State = EntityState.Added;
                }
            }

            int recAffected = 0;
            try {
                recAffected=await context.SaveChangesAsync();
            } catch (DbException ex) {
                strError = ex.Message;
            } catch (Exception ex) {
                strError = ex.Message;
            }

            return string.IsNullOrEmpty(strError) ? "" : strError;
        }

        public static async Task<string> ReadEmployeeDataExcelFile(this DataContext context, string filePath, string Username)
        {
            // var professionId = label in row2, col3; data in row2, col4
            //  var ProfessionName = label in row2, col 5, data in row2, col6
            //  column titles in row 3 data starts from row 4
            var strError="";
            int rowTitle=4;
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows=0, columns=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["Sheet1"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    strError = ex.Message;
                    return strError;
                }
                
                int intGender=0, intFirstName=0, intSecondName=0, intFamilyName=0;
                int intKnownAs=0, intUserName=0, intDepartment = 0;
                int intPosition=0, intQualification=0, intDOB=0, intAadharNo=0, intPPNo=0, intEmail=0;
                int intDOJ=0, intPlaceOfBirth=0, intCity=0, intAddress=0, intPIN=0, intPhoneNo=0, intMobileNo=0;
                int intHRSkill1ProfessionId=0, intHRSkill1SkillLevelName=0, intHRSkill1IsMain=0;
                int intHRSkill2ProfessionId=0, intHRSkill2SkillLevelName=0, intHRSkill2IsMain=0;
                int intHRSkill3ProfessionId=0, intHRSkill3SkillLevelName=0, intHRSkill3IsMain=0;
                int intOtherSkill1DataId=0, intOtherSkill1Level=0, intOtherSkill1IsMain=0;
                int intOtherSkill2DataId=0, intOtherSkill2Level=0, intOtherSkill2IsMain=0;
                int intOtherSkill3DataId=0, intOtherSkill3Level=0, intOtherSkill3IsMain=0;

                var DateRegistered = DateTime.Now;
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value.ToString();
                    switch (colTitle.ToLower()) {
                        case "gender": intGender=col;break;
                        case "employee name" :case "employeename": case "first name": case "firstname": intFirstName=col; break;
                        case "second name" :case "secondname": case "father's name": case "father name": intSecondName=col; break;
                        case "family name" :case "familyname": case "surname": intFamilyName=col; break;
                        case "knownas" :case "known as": intKnownAs=col; break;
                        case "department": case "dept": case "divn": intDepartment=col; break;
                        case "username": case "user name": intUserName=col;break;
                        case "position": case "designation": intPosition=col;break;
                        case "emailid": case "email id": intEmail=col;break;
                        case "qualification": intQualification=col;break;
                        case "dob": intDOB=col;break;
                        case "doj": case "date of joining": case "joining date": case "joined on": intDOJ=col;break;
                        case "place of birth": case "birth place": intPlaceOfBirth=col;break;
                        case "address": intAddress=col;break;
                        case "city" : intCity=col;break;
                        case "pin" : intPIN=col; break;
                        case "aadahar": case "aadhar no": intAadharNo=col; break;
                        case "pp no": case "ppno": case "passport": intPPNo=col; break;
                        case "email": case "official email": case "officialemail": intEmail=col;break;
                        case "phone": case "official phone no": case "officialph": intPhoneNo=col;break;
                        case "mobile": case "mobile no": intMobileNo=col;break;
                        
                        case "hrskill1professionid": case "hrskill1profid": case "hr skill1 professionid": intHRSkill1ProfessionId=col; break;
                        case "hrskill2professionid": case "hrskill2profid": case "hr skill2 professionid": intHRSkill2ProfessionId=col; break;
                        case "hrskill3professionid": case "hrskill3profid": case "hr skill3 professionid": intHRSkill3ProfessionId=col; break;
                        case "hrskill1skilllevelName": case "hrskill1skillName level": intHRSkill1SkillLevelName=col; break;
                        case "hrskill2skilllevel": case "hrskill2skill level": intHRSkill2SkillLevelName=col; break;
                        case "hrskill3skilllevel": case "hrskill3skill level": intHRSkill3SkillLevelName=col; break;
                        case "hrskill1ismain": case "hrskill1is main": intHRSkill1IsMain=col; break;
                        case "hrskill2ismain": case "hrskill2is main": intHRSkill2IsMain=col; break;
                        case "hrskill3ismain": case "hrskill3is main": intHRSkill3IsMain=col; break;

                        case "otherskill1skilldataid": case "otherskill1 dataid": case "other skill1 skilldataid": intOtherSkill1DataId=col; break;
                        case "otherskill2skilldataid": case "otherskill2 dataid": case "other skill2 skilldataid": intOtherSkill2DataId=col; break;
                        case "otherskill3skilldataid": case "otherskill3 dataid": case "other skill3 skilldataid": intOtherSkill3DataId=col; break;

                        case "otherskill1level": case "otherskill1 skill level": intOtherSkill1Level=col; break;
                        case "otherskill2level": case "otherskill2 skill level": intOtherSkill2Level=col; break;
                        case "otherskill3level": case "otherskill3 skill level": intOtherSkill3Level=col; break;

                        case "otherskill1ismain": case "otherskill1 ismain": intOtherSkill1IsMain=col; break;
                        case "otherskill2ismain": case "otherskill2 ismain": intOtherSkill2IsMain=col; break;
                        case "otherskill3ismain": case "otherskill3 ismain": intOtherSkill3IsMain=col; break;
                        
                        default:break;
                    }
                }
                
                string FirstName = "", SecondName = "", FamilyName = "", EmailId = "";
                string DOB = "", DOJ = "",  Department="";
                string PhoneNo = "", CurrentLocation = "", Qualification = "", MobileNo="";
                string Gendr = "",  Address = "", City="";
                int HRSkill1ProfessionId=0, HRSkill2ProfessionId=0, HRSkill3ProfessionId=0;
                string HRSkill1SkillLevelName="", HRSkill2SkillLevelName="", HRSkill3SkillLevelName="";
                bool HRSkill1IsMain=false, HRSkill2IsMain=false, HRSkill3IsMain=false;

                int OtherSkill1DataId=0, OtherSkill2DataId=0, OtherSkill3DataId=0;
                int OtherSkill1Level=0, OtherSkill2Level=0, OtherSkill3Level=0;
                bool OtherSkill1IsMain=false, OtherSkill2IsMain=false, OtherSkill3IsMain=false;
                
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    Gendr = intGender == 0 ? "Male" : worksheet.Cells[row, intGender].Value.ToString() ?? "Male";
                    Gendr = Gendr == "m" ? "Male" : "Female";
                    FirstName = intFirstName == 0 ? "" : worksheet.Cells[row, intFirstName].Value.ToString() ?? "";
                    SecondName = intSecondName == 0 ? "" : worksheet.Cells[row, intSecondName].Value.ToString() ?? "";
                    FamilyName = intFamilyName == 0 ? "" : worksheet.Cells[row, intFamilyName].Value.ToString() ?? "";
                    Department = intDepartment == 0 ? "" : worksheet.Cells[row, intDepartment].Value.ToString() ?? "";
                    Address = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    City = intCity == 0 ? "" : worksheet.Cells[row, intCity].Value.ToString() ?? "";
                    
                    PhoneNo = intPhoneNo == 0 ? "" : worksheet.Cells[row, intPhoneNo].Value.ToString() ?? "";
                    Qualification = intQualification == 0 ? "" : worksheet.Cells[row, intQualification].Value.ToString() ?? "";
                    
                    EmailId = intEmail == 0 ? "" : worksheet.Cells[row, intEmail].Value.ToString() ?? "";
                    DOB = intDOB == 0 ? "" : worksheet.Cells[row, intDOB].Value.ToString() ?? "";
                    DOJ = intDOJ == 0 ? "" : worksheet.Cells[row, intDOJ].Value.ToString() ?? "";
                    MobileNo = intMobileNo == 0 ? "": worksheet.Cells[row, intMobileNo].Value.ToString()  ?? "";
                    Address = intAddress == 0 ? "": worksheet.Cells[row, intAddress].Value.ToString() ?? "";

                    if(intHRSkill1ProfessionId > 0) {
                      HRSkill1ProfessionId = Convert.ToInt32(worksheet.Cells[row, intHRSkill1ProfessionId].Value.ToString());
                      HRSkill1SkillLevelName = worksheet.Cells[row, intHRSkill1SkillLevelName].Value.ToString();
                      HRSkill1IsMain = Convert.ToBoolean(worksheet.Cells[row, intHRSkill1IsMain].Value.ToString());
                    }

                    if(intOtherSkill1DataId > 0) {
                        OtherSkill1DataId = Convert.ToInt32(worksheet.Cells[row, intOtherSkill1DataId].Value.ToString());
                        OtherSkill1Level = Convert.ToInt32(worksheet.Cells[row, intOtherSkill1Level].Value.ToString());
                        OtherSkill1IsMain = Convert.ToBoolean(worksheet.Cells[row, intOtherSkill1IsMain].Value.ToString());
                    }

                    _ = DateTime.TryParse(DOB, out DateTime dob);
                    _ = DateTime.TryParse(DOJ, out DateTime doj);
                    

                    var newEmployee = new Employee
                    {
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        FirstName = FirstName, SecondName = SecondName, FamilyName=FamilyName,
                        DateOfBirth = dob, DateOfJoining=doj,
                        Address = Address,
                        City = CurrentLocation,
                        Email = EmailId,
                        PhoneNo = PhoneNo, Phone2 = MobileNo,
                        Department = Department,
                        HRSkills = new List<HRSkill>(),
                        EmployeeOtherSkills = new List<EmployeeOtherSkill>(),
                        EmployeeAttachments = new List<EmployeeAttachment>()
                    };

                    if(HRSkill1ProfessionId > 0) newEmployee.HRSkills.Add(new() {
                        ProfessionId=HRSkill1ProfessionId, SkillLevelName=HRSkill1SkillLevelName, IsMain=true});
                    if(HRSkill2ProfessionId > 0) newEmployee.HRSkills.Add(new() {
                        ProfessionId=HRSkill2ProfessionId, SkillLevelName= HRSkill2SkillLevelName, IsMain=HRSkill2IsMain});
                    if(HRSkill3ProfessionId > 0) newEmployee.HRSkills.Add(new() {
                        ProfessionId=HRSkill3ProfessionId, SkillLevelName= HRSkill3SkillLevelName, IsMain=HRSkill3IsMain});

                    if(OtherSkill1DataId != 0) newEmployee.EmployeeOtherSkills.Add(new () {
                        SkillDataId=OtherSkill1DataId, SkillLevel=OtherSkill1Level, IsMain=OtherSkill1IsMain});
                    if(OtherSkill2DataId != 0) newEmployee.EmployeeOtherSkills.Add(new () {
                        SkillDataId=OtherSkill2DataId, SkillLevel=OtherSkill2Level, IsMain=OtherSkill2IsMain});
                    if(OtherSkill3DataId != 0) newEmployee.EmployeeOtherSkills.Add(new () {
                        SkillDataId=OtherSkill3DataId, SkillLevel=OtherSkill3Level, IsMain=OtherSkill3IsMain});
                    
                    context.Entry(newEmployee).State = EntityState.Added;
                }
            }

            int recAffected = 0;
            try {
                recAffected=await context.SaveChangesAsync();
            } catch (DbException ex) {
                strError = ex.Message;
            } catch (Exception ex) {
                strError = ex.Message;
            }

            return string.IsNullOrEmpty(strError) ? "" : strError;
        }

        public static async Task<int> ReadOrderDataExcelFile(this DataContext context, string filePath, string Username)
        {
            //column title in row 2, data starts from row 3
            //string filePath = "D:\\IdealR_/Ideal/api/CandidateExcelData.xlsx";
            int rowTitle=2;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new();
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                int intOrderNo=0, intOrderDate=0, intCustomerId=0, intOrderRef=0, intOrderRefDate=0;
                int intProjectManagerId=0, intSalesmanId=0, intCompleteBy=0;
                int intCityOfWorking=0, intStatus=0, intForwardedToHRDeptOn=0;
                
                int intSrNo1=0, intProfessionId1=0, intSourceFrom1=0, intQnty1=0, intMinCV1=0, intMaxCV1=0, intStatus1=0;
                int intSrNo2=0, intProfessionId2=0, intSourceFrom2=0, intQnty2=0, intMinCV2=0, intMaxCV2=0, intStatus2=0;
                int intSrNo3=0, intProfessionId3=0, intSourceFrom3=0, intQnty3=0, intMinCV3=0, intMaxCV3=0, intStatus3=0;
                int intSrNo4=0, intProfessionId4=0, intSourceFrom4=0, intQnty4=0, intMinCV4=0, intMaxCV4=0, intStatus4=0;
                int intSrNo5=0, intProfessionId5=0, intSourceFrom5=0, intQnty5=0, intMinCV5=0, intMaxCV5=0, intStatus5=0;


                for(int col=3; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value.ToString();

                    switch (colTitle.ToLower()) {
                        case "orderno": case "order no": intOrderNo=col;break;
                        case "orderdate": case "order date": intOrderDate=col; break;
                        case "customerid": case "customer id": intCustomerId=col;break;
                        case "orderref": case "order ref": intOrderRef=col;break;
                        case "order ref date": case "order refdate": intOrderRefDate=col;break;
                        case "proj manager id": case "projmanager id": case "project manager id": intProjectManagerId=col;break;
                        case "salesman id": case "salesmanid": case "salesman": intSalesmanId=col;break;
                        case "complete by": case "complete before": intCompleteBy=col;break;
                        case "cityofworking": case "city of work": case "city of working": intCityOfWorking=col;break;
                        case "forwarded to hr dept on": case "forwardedtohrdepton": intForwardedToHRDeptOn=col;break;
                        case "status" : intStatus=col;break;

                        case "srno1": case "sr no1": intSrNo1=col;break;
                        case "professionid1": case "profession id1": intProfessionId1=col;break;
                        case "sourcefrom1": case "source from1": case "source from 1":  intSourceFrom1=col;break;
                        case "qnty1": case "quantity 1": case "quantity1": intQnty1=col;break;
                        case "mincv1": case "min cv1": intMinCV1=col;break;
                        case "maxcv1": case "max cv1" : intMaxCV1=col;break;
                        case "status1": case "status 1": intStatus1=col;break;
                        
                        case "srno2": case "sr no2": intSrNo2=col;break;
                        case "professionid2": case "profession id2": intProfessionId2=col;break;
                        case "sourcefrom2": case "source from2": case "source from 2":  intSourceFrom2=col;break;
                        case "qnty2": case "quantity 2": case "quantity2": intQnty2=col;break;
                        case "mincv2": case "min cv2": intMinCV2=col;break;
                        case "maxcv2": case "max cv2" : intMaxCV2=col;break;
                        case "status2": case "status 2": intStatus2=col;break;
                        
                        case "srno3": case "sr no3": intSrNo3=col;break;
                        case "professionid3": case "profession id3": intProfessionId3=col;break;
                        case "sourcefrom3": case "source from3": case "source from 3":  intSourceFrom3=col;break;
                        case "qnty3": case "quantity 3": case "quantity3": intQnty3=col;break;
                        case "mincv3": case "min cv3": intMinCV3=col;break;
                        case "maxcv3": case "max cv3" : intMaxCV3=col;break;
                        case "status3": case "status 3": intStatus3=col;break;
                        
                        case "srno4": case "sr no4": intSrNo4=col;break;
                        case "professionid4": case "profession id4": intProfessionId4=col;break;
                        case "sourcefrom4": case "source from4": case "source from 4":  intSourceFrom4=col;break;
                        case "qnty4": case "quantity 4": case "quantity4": intQnty4=col;break;
                        case "mincv4": case "min cv4": intMinCV4=col;break;
                        case "maxcv4": case "max cv4" : intMaxCV4=col;break;
                        case "status4": case "status 4": intStatus4=col;break;
                        
                        case "srno5": case "sr no5": intSrNo5=col;break;
                        case "professionid5": case "profession id5": intProfessionId5=col;break;
                        case "sourcefrom5": case "source from5": case "source from 5":  intSourceFrom5=col;break;
                        case "qnty5": case "quantity 5": case "quantity5": intQnty5=col;break;
                        case "mincv5": case "min cv5": intMinCV5=col;break;
                        case "maxcv5": case "max cv5" : intMaxCV5=col;break;
                        case "status5": case "status 5": intStatus5=col;break;

                        default:break;
                    }
                }

                for (int row = 3; row <= rows; row++)
                {
                    var newOrder = new Order
                    {
                        OrderNo = Convert.ToInt32(worksheet.Cells[row, intOrderNo].Value.ToString() ?? "0"), 
                        OrderDate = Convert.ToDateTime(worksheet.Cells[row, intOrderDate].Value.ToString()),
                        CustomerId = Convert.ToInt32(worksheet.Cells[row, intCustomerId].Value.ToString() ?? "0"),
                        ProjectManagerId = Convert.ToInt32(worksheet.Cells[row, intProjectManagerId].Value.ToString() ?? "0"),
                        SalesmanId = Convert.ToInt32(worksheet.Cells[row, intSalesmanId].Value.ToString() ?? "0"),
                        CompleteBy = Convert.ToDateTime(worksheet.Cells[row, intCompleteBy].Value.ToString() ?? ""),
                        CityOfWorking =  worksheet.Cells[row,intCityOfWorking].Value.ToString() ?? "",
                        Status = worksheet.Cells[row, intStatus].Value.ToString() ?? "",
                        ForwardedToHRDeptOn = Convert.ToDateTime(worksheet.Cells[row,intForwardedToHRDeptOn].Value.ToString() ?? ""),

                        OrderItems = new List<Entities.Admin.Order.OrderItem>{new() { 
                            SrNo = Convert.ToInt32(worksheet.Cells[row, intSrNo1].Value.ToString() ?? "1"),
                            ProfessionId = Convert.ToInt32(worksheet.Cells[row, intProfessionId1].Value.ToString() ?? "0"),
                            SourceFrom = worksheet.Cells[row, intSourceFrom1].Value.ToString() ?? "India",
                            Quantity = Convert.ToInt32(worksheet.Cells[row, intQnty1].Value.ToString() ?? "1"),
                            MinCVs = Convert.ToInt32(worksheet.Cells[row, intMinCV1].Value.ToString() ?? "3"),
                            MaxCVs = Convert.ToInt32(worksheet.Cells[row, intMaxCV1].Value.ToString() ?? "3"),
                            Status =  worksheet.Cells[row, intStatus1].Value.ToString() ?? "Active"
                        }
                        },
                    };      

                    var prof = worksheet.Cells[row, intSrNo2].Value.ToString();
                    if(!string.IsNullOrEmpty(prof)) {
                        newOrder.OrderItems.Add(new Entities.Admin.Order.OrderItem() {
                            SrNo = Convert.ToInt32(worksheet.Cells[row, intSrNo2].Value.ToString() ?? "2"),
                            ProfessionId = Convert.ToInt32(prof),
                            SourceFrom = worksheet.Cells[row, intSourceFrom2].Value.ToString() ?? "India",
                            Quantity = Convert.ToInt32(worksheet.Cells[row, intQnty2].Value.ToString() ?? "1"),
                            MinCVs = Convert.ToInt32(worksheet.Cells[row, intMinCV2].Value.ToString() ?? "3"),
                            MaxCVs = Convert.ToInt32(worksheet.Cells[row, intMaxCV2].Value.ToString() ?? "3"),
                            Status =  worksheet.Cells[row, intStatus2].Value.ToString() ?? "Active"
                        });
                    }

                    prof = worksheet.Cells[row, intSrNo3].Value.ToString();
                    if(!string.IsNullOrEmpty(prof)) {
                        newOrder.OrderItems.Add(new Entities.Admin.Order.OrderItem() {
                            SrNo = Convert.ToInt32(worksheet.Cells[row, intSrNo3].Value.ToString() ?? "3"),
                            ProfessionId = Convert.ToInt32(prof),
                            SourceFrom = worksheet.Cells[row, intSourceFrom3].Value.ToString() ?? "India",
                            Quantity = Convert.ToInt32(worksheet.Cells[row, intQnty3].Value.ToString() ?? "1"),
                            MinCVs = Convert.ToInt32(worksheet.Cells[row, intMinCV3].Value.ToString() ?? "3"),
                            MaxCVs = Convert.ToInt32(worksheet.Cells[row, intMaxCV3].Value.ToString() ?? "3"),
                            Status =  worksheet.Cells[row, intStatus3].Value.ToString() ?? "Active"
                        });
                    }
                    
                    prof = worksheet.Cells[row, intSrNo4].Value.ToString();
                    if(!string.IsNullOrEmpty(prof)) {
                        newOrder.OrderItems.Add(new Entities.Admin.Order.OrderItem() {
                            SrNo = Convert.ToInt32(worksheet.Cells[row, intSrNo4].Value.ToString() ?? "3"),
                            ProfessionId = Convert.ToInt32(prof),
                            SourceFrom = worksheet.Cells[row, intSourceFrom4].Value.ToString() ?? "India",
                            Quantity = Convert.ToInt32(worksheet.Cells[row, intQnty4].Value.ToString() ?? "1"),
                            MinCVs = Convert.ToInt32(worksheet.Cells[row, intMinCV4].Value.ToString() ?? "3"),
                            MaxCVs = Convert.ToInt32(worksheet.Cells[row, intMaxCV4].Value.ToString() ?? "3"),
                            Status =  worksheet.Cells[row, intStatus4].Value.ToString() ?? "Active"
                        });
                    }
                    
                    prof = worksheet.Cells[row, intSrNo5].Value.ToString();
                    if(!string.IsNullOrEmpty(prof)) {
                        newOrder.OrderItems.Add(new Entities.Admin.Order.OrderItem() {
                            SrNo = Convert.ToInt32(worksheet.Cells[row, intSrNo5].Value.ToString() ?? "3"),
                            ProfessionId = Convert.ToInt32(prof),
                            SourceFrom = worksheet.Cells[row, intSourceFrom5].Value.ToString() ?? "India",
                            Quantity = Convert.ToInt32(worksheet.Cells[row, intQnty5].Value.ToString() ?? "1"),
                            MinCVs = Convert.ToInt32(worksheet.Cells[row, intMinCV5].Value.ToString() ?? "3"),
                            MaxCVs = Convert.ToInt32(worksheet.Cells[row, intMaxCV5].Value.ToString() ?? "3"),
                            Status =  worksheet.Cells[row, intStatus5].Value.ToString() ?? "Active"
                        });
                    }
                    
                    context.Entry(newOrder).State = EntityState.Added;
                   
                }
            }

            var recAffected=await context.SaveChangesAsync();
            return recAffected;
        }

   

    }
}