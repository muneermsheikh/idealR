using System.Data;
using System.Data.Common;
using api.Data;
using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Master;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Extensions
{
    public static class ReadExcelSheet
    {   
        public static async Task<ReturnStringsDto> ReadProspectiveCandidateDataExcelFile(this DataContext context, string filePath, string Username)
        {
            // var CategoryRef - label in row2, column1, data in row2, column 2  //this is redendent
            // var OrderItemId = label in row2, col1, data in row3, col2 ;
            // var professionId = label in row2, col3; data in row2, col4       //this is redendent
            //var ProfessionName = label in row2, col 5, data in row2, col6     //this is redendent
            //column titles in row 4, data starts from row 5
            var dtoErr = new ReturnStringsDto();

            int rowTitle=4;     //data starts from this row
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
                    dtoErr.ErrorString = ex.Message;
                    return dtoErr;
                }
                
                string CategoryRef="", ProfessionName=""; 
                var Source="";
                int OrderItemId = 0, ProfessionId = 0;
                int intOrderItemIdRow=2;
                for(int col=1; col <= 3; col++){
                    try {
                        var colTitle = worksheet.Cells[intOrderItemIdRow, col].Value?.ToString();
                        switch (colTitle.ToLower()) {
                            case "orderitemid": case "order item id": case "orderitem id": 
                                OrderItemId=Convert.ToInt32((worksheet.Cells[intOrderItemIdRow, col+1].Value ?? "0").ToString()); 
                                break;
                            case "source":
                                Source = (worksheet.Cells[intOrderItemIdRow, col+1].Value ?? "Times Job").ToString(); 
                                break;
                            default:break;
                        }
                    } catch (Exception ex) {
                        dtoErr.ErrorString = ex.Message;
                        return dtoErr;
                    }
                }

                if(OrderItemId == 0) {
                    dtoErr.ErrorString = "OrderItem Id Not defined";
                    return dtoErr;
                }

                var query = await (from item in context.OrderItems where item.Id == OrderItemId
                        join order in context.Orders on item.OrderId equals order.Id
                        join cat in context.Professions on item.ProfessionId equals cat.Id
                        select new {ProfessionId=item.ProfessionId, 
                        CategoryRef=order.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName,
                        ProfessionName=cat.ProfessionName}
                ).FirstOrDefaultAsync();

                if(query==null) {
                    dtoErr.ErrorString = "Invalid OrderItem Id";
                    return dtoErr;
                }

                ProfessionName = query.ProfessionName;
                var lng = query.CategoryRef.Length;
                var cutLength = lng > 50 ? 50 : lng;
                CategoryRef = query.CategoryRef[..cutLength];
                ProfessionId = query.ProfessionId;
                
                
                //DataTable dataTable = new();
                
                int intCandidateName=0, intEmail=0, intAlternateEmail=0, intAlternatePhone=0, intDob=0;
                int intMobileNo=0, intAlternateNo=0, intResumeTitle=0, intKeySkills=0, intWorkExp=0, intCurrentLocation=0;
                int intEducation=0, intGender=0, intAge=0, intAddress=0, intCity=0, intResumeId=0,intDesignation=0;
    
                var DateRegistered = DateTime.Now;
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "candidatename" :case "name": intCandidateName=col; break;
                        case "emailid": case "email id": intEmail=col;break;
                        case "alternateemailid": case "alternate email": case "alternte emailid": case "alternate email id": intAlternateEmail=col;break;
                        case "alternatephone": intAlternatePhone=col;break;
                        case "dob": case "date of birth": intDob=col;break;
                        case "phone number": intMobileNo=col;break;
                        case "alternatenumber": case "alternate number": case "alternate contact no.": case "alternate contact no": intAlternateNo=col;break;
                        case "resumetitle": intResumeTitle=col;break;
                        case "keyskills": intKeySkills=col;break;
                        case "workexp": case "work experience": intWorkExp=col;break;
                        case "currentlocation" : case "current location": intCurrentLocation=col;break;
                        case "education": case "course": case "course1" : intEducation=col;break;
                        case "gender": intGender=col;break;
                        case "age": intAge=col;break;
                        case "address": intAddress=col;break;
                        case "city": intCity=col;break;
                        case "resumeid": case "resume id": intResumeId=col;break;
                        case "designation": intDesignation=col;break;
                        default:break;
                    }
                }
                
                string CandidateName = "", EmailId = "", AlternateEmailId = "", AlternatePhone = "", DOB = "";
                string MobileNo = "", AlternateNumber = "", ResumeTitle = "", KeySkills = "", WorkExp = "", CurrentLocation = "";
                string Education = "", Gendr = "", Age = "", Address = "", City="", ResumeId = "", Designation = "";
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    CandidateName = intCandidateName == 0 ? "" : worksheet.Cells[row, intCandidateName].Value?.ToString() ?? "";
                    EmailId = intEmail == 0 ? "" : worksheet.Cells[row, intEmail].Value?.ToString() ?? "";
                    AlternateEmailId = intAlternateEmail == 0 ? "" : worksheet.Cells[row, intAlternateEmail].Value?.ToString() ?? "";
                    AlternatePhone = intAlternatePhone == 0 ? "" : worksheet.Cells[row, intAlternatePhone].Value?.ToString() ?? "";
                    DOB = intDob == 0 ? "" : worksheet.Cells[row, intDob].Value?.ToString() ?? "";
                    MobileNo = intMobileNo == 0 ? "": worksheet.Cells[row, intMobileNo].Value?.ToString()  ?? "";
                    AlternateNumber = intAlternatePhone == 0 ? "" : worksheet.Cells[row, intAlternateNo].Value?.ToString() ?? "";
                    ResumeTitle = intResumeTitle == 0 ? "" : worksheet.Cells[row, intResumeTitle].Value?.ToString() ?? "";
                    KeySkills = intKeySkills == 0 ? "" : worksheet.Cells[row, intKeySkills].Value?.ToString() ?? "";
                    WorkExp = intWorkExp == 0 ? "" : worksheet.Cells[row, intWorkExp].Value?.ToString() ?? "";
                    Address = intAddress == 0 ? "": worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    CurrentLocation = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCurrentLocation].Value?.ToString() ?? "";
                    Education = intEducation == 0 ? "" : worksheet.Cells[row, intEducation].Value?.ToString() ?? "";
                    Gendr = intGender == 0 ? "Male" : worksheet.Cells[row, intGender].Value?.ToString() ?? "Male";
                    Gendr = Gendr == "m" ? "Male" : "Female";
                    Age =  intAge == 0 ? "" : worksheet.Cells[row, intAge].Value?.ToString() ?? "";
                    Address =  intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    City = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCity == 0 ? intCurrentLocation : intCity].Value?.ToString() ?? "";
                    ResumeId = intResumeId == 0 ? "" : worksheet.Cells[row, intResumeId].Value?.ToString() ?? "";
                    Designation = intDesignation == 0 ? "" : worksheet.Cells[row, intDesignation].Value?.ToString() ?? "";
                    var newProspectiveCandidate = new ProspectiveCandidate
                    {
                        CategoryRef = CategoryRef,
                        OrderItemId = OrderItemId,
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        CandidateName = CandidateName.Length > 49 ? CandidateName[..49]: CandidateName ,
                        PersonId = ResumeId,
                        ResumeId = ResumeId,
                        Source = Source,
                        //DateOfBirth = dob,
                        Age = Age.Length > 10 ? Age[..10] : Age,
                        Address = Address,
                        City = City,
                        PhoneNo =  MobileNo.Length > 20 ? MobileNo[..20] : MobileNo,
                        AlternateNumber = AlternateNumber,
                        CurrentLocation = CurrentLocation,
                        Email = EmailId,
                        AlternateEmail = AlternateEmailId,
                        ProfessionId = ProfessionId,
                        ProfessionName = ProfessionName,
                        DateRegistered = DateRegistered,
                        ResumeTitle = ResumeTitle.Length > 49 ? ResumeTitle[..49] : ResumeTitle,
                        Education = Education,
                        WorkExperience = WorkExp,
                        Username=Username
                    };

                    if ( DOB != "" && Age != "" && !DateTime.TryParse(DOB, out DateTime dob))
                    {
                        dob = new DateTime();
                        if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                        newProspectiveCandidate.DateOfBirth = dob;
                    }

                    context.Entry(newProspectiveCandidate).State = EntityState.Added;
                }
            }

            bool isSaved = false;
            int recAffected = 0;
            do
                {
                    try
                    {
                        recAffected += await context.SaveChangesAsync();
                        isSaved = true;
                        dtoErr.SuccessString = recAffected + " records copied";
                    }
                    catch (DbUpdateException ex)
                    {
                        foreach (var entry in ex.Entries) {
                            Console.Write("Prospective candidates Exception - " + ex.InnerException.Message);

                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoErr.ErrorString += ex.Message;
                        }
                    }
                    catch (DbException ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }

                    catch (Exception ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }
                }
            while (!isSaved);

            return dtoErr;
        }

        public static async Task<ReturnStringsDto> ReadNaukriProspectiveCandidateDataExcelFile(this DataContext context, string filePath, string Username)
        {
            // var CategoryRef - label in row2, column1, data in row2, column 2  //this is redendent
            // var OrderItemId = label in row2, col1, data in row3, col2 ;
            // var professionId = label in row2, col3; data in row2, col4       //this is redendent
            //var ProfessionName = label in row2, col 5, data in row2, col6     //this is redendent
            //column titles in row 4, data starts from row 5
            var dtoErr = new ReturnStringsDto();

            int rowTitle=4;     //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows=0, columns=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["MySheet"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    dtoErr.ErrorString = ex.Message;
                    return dtoErr;
                }
                
                string CategoryRef="", ProfessionName=""; 
                var Source="";
                int OrderItemId = 0, ProfessionId = 0;
                int intOrderItemIdRow=2;
                for(int col=1; col < 4; col++){
                    try {
                        var colTitle = worksheet.Cells[intOrderItemIdRow, col].Value?.ToString();
                        switch (colTitle.ToLower()) {
                            case "categoryref": case "category ref": 
                                CategoryRef = worksheet.Cells[intOrderItemIdRow, col+1].Value.ToString() ?? "";
                                if(string.IsNullOrEmpty(CategoryRef)) {
                                    dtoErr.ErrorString = "Category Ref not defined";
                                } else {
                                    int position = CategoryRef.IndexOf("-");
                                    if(position==-1) {
                                        dtoErr.ErrorString = "Category Ref should contain a hyphen '-' separating order no with category SrNo";
                                    } else {
                                        var qry = await (from item in context.OrderItems where item.SrNo == Convert.ToInt32(CategoryRef.Substring(position+1))
                                            join order in context.Orders on item.OrderId equals order.Id where order.OrderNo == Convert.ToInt32(CategoryRef.Substring(position+1))
                                            select new {ProfessionId = item.ProfessionId, OrderItemId=item.Id}).FirstOrDefaultAsync();
                                    if(qry == null) {
                                        dtoErr.ErrorString = "Invalid Category Ref";
                                    } else {
                                        OrderItemId = qry.OrderItemId;
                                        ProfessionId = qry.ProfessionId;
                                    }
                                }       
                                }
                            if(!string.IsNullOrEmpty(dtoErr.ErrorString)) return dtoErr;
                                break;
                            case "source":
                                Source = (worksheet.Cells[intOrderItemIdRow, col+1].Value ?? "Naukri.Com").ToString(); 
                                break;
                            default:break;
                        }
                    } catch (Exception ex) {
                        dtoErr.ErrorString = ex.Message;
                        return dtoErr;
                    }
                }

                if(OrderItemId == 0) {
                    dtoErr.ErrorString = "OrderItem Id Not defined";
                    return dtoErr;
                }

                var query = await (from item in context.OrderItems where item.Id == OrderItemId
                        join order in context.Orders on item.OrderId equals order.Id
                        join cat in context.Professions on item.ProfessionId equals cat.Id
                        select new {ProfessionId=item.ProfessionId, 
                        CategoryRef=order.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName,
                        ProfessionName=cat.ProfessionName}
                ).FirstOrDefaultAsync();

                if(query==null) {
                    dtoErr.ErrorString = "Invalid OrderItem Id";
                    return dtoErr;
                }

                ProfessionName = query.ProfessionName;
                var lng = query.CategoryRef.Length;
                var cutLength = lng > 50 ? 50 : lng;
                CategoryRef = query.CategoryRef[..cutLength];
                ProfessionId = query.ProfessionId;
                
                
                //DataTable dataTable = new();
                
                int intCandidateName=0, intEmail=0, intDob=0;
                int intMobileNo=0, intResumeTitle=0, intKeySkills=0, intWorkExp=0, intCurrentLocation=0;
                int intEducation=0, intGender=0, intAge=0, intAddress=0, intCity=0, intDesignation=0;
    
                var DateRegistered = DateTime.Now;
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "name": intCandidateName=col; break;
                        case "emailid": case "email id": intEmail=col;break;
                        //case "alternateemailid": case "alternate email": case "alternte emailid": case "alternate email id": intAlternateEmail=col;break;
                        //case "alternatephone": intAlternatePhone=col;break;
                        case "dob": case "date of birth": intDob=col;break;
                        case "phone": case "mobile no": case "mobile no.": intMobileNo=col;break;
                        //case "alternatenumber": case "alternate number": case "alternate contact no.": case "alternate contact no": intAlternateNo=col;break;
                        case "job title": intResumeTitle=col;break;
                        case "key skills": intKeySkills=col;break;
                        case "total experience": intWorkExp=col;break;
                        case "current location": intCurrentLocation=col;break;
                        case "education": case "under graduation degree" : intEducation=col;break;
                        case "gender": intGender=col;break;

                        case "age": intAge=col;break;
                        case "permanent address": intAddress=col;break;
                        case "home town/city": intCity=col;break;
                        //case "resumeid": case "resume id": intResumeId=col;break;
                        case "curr. company designation": intDesignation=col;break;
                        default:break;
                    }
                }
                
                string CandidateName = "", EmailId = "", DOB = "";
                string MobileNo = "", ResumeTitle = "", KeySkills = "", WorkExp = "", CurrentLocation = "";
                string Education = "", Gendr = "", Age = "", Address = "", City="", ResumeId = "", Designation = "";
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    CandidateName = intCandidateName == 0 ? "" : worksheet.Cells[row, intCandidateName].Value?.ToString() ?? "";
                    EmailId = intEmail == 0 ? "" : worksheet.Cells[row, intEmail].Value?.ToString() ?? "";
                    DOB = intDob == 0 ? "" : worksheet.Cells[row, intDob].Value.ToString() ?? "";
                    MobileNo = intMobileNo == 0 ? "": worksheet.Cells[row, intMobileNo].Value?.ToString()  ?? "";
                    ResumeTitle = intResumeTitle == 0 ? "" : worksheet.Cells[row, intResumeTitle].Value?.ToString() ?? "";
                    KeySkills = intKeySkills == 0 ? "" : worksheet.Cells[row, intKeySkills].Value?.ToString() ?? "";
                    WorkExp = intWorkExp == 0 ? "" : worksheet.Cells[row, intWorkExp].Value?.ToString() ?? "";
                    Address = intAddress == 0 ? "": worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    CurrentLocation = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCurrentLocation].Value?.ToString() ?? "";
                    Education = intEducation == 0 ? "" : worksheet.Cells[row, intEducation].Value?.ToString() ?? "";
                    Gendr = intGender == 0 ? "Male" : worksheet.Cells[row, intGender].Value?.ToString() ?? "Male";
                    Gendr = Gendr == "m" ? "Male" : "Female";
                    Age =  intAge == 0 ? "" : worksheet.Cells[row, intAge].Value?.ToString() ?? "";
                    Address =  intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    City = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCity == 0 ? intCurrentLocation : intCity].Value?.ToString() ?? "";
                    Designation = intDesignation == 0 ? "" : worksheet.Cells[row, intDesignation].Value?.ToString() ?? "";

                    var isDOB = DateTime.TryParse(DOB, out DateTime dtDOB);
                    var personId= Guid.NewGuid().ToString();
                    if(personId.Length > 44) personId=personId[..44];
                    var newProspectiveCandidate = new ProspectiveCandidate
                    {
                        PersonId = personId,
                        CategoryRef = CategoryRef,
                        OrderItemId = OrderItemId,
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        CandidateName = CandidateName.Length > 49 ? CandidateName[..49] : CandidateName, 
                        ResumeId = ResumeId,
                        Source = Source,
                        DateOfBirth = isDOB ? dtDOB : new DateTime(1900,1,1),
                        Age = Age.Length > 10 ? Age[..10] : Age,
                        Address = Address,
                        City = City,
                        PhoneNo = MobileNo.Length > 15 ? MobileNo[..15] : MobileNo,
                        CurrentLocation = CurrentLocation,
                        Email = EmailId,
                        ProfessionId = ProfessionId,
                        ProfessionName = ProfessionName,
                        DateRegistered = DateRegistered,
                        ResumeTitle = ResumeTitle.Length > 49 ? ResumeTitle[..49] : ResumeTitle,
                        Education = Education,
                        WorkExperience = WorkExp,
                        Username = Username
                    };

                    if ( DOB != "" && Age != "" && !DateTime.TryParse(DOB, out DateTime dob))
                    {
                        dob = new DateTime();
                        if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                        newProspectiveCandidate.DateOfBirth = dob;
                    }

                    context.Entry(newProspectiveCandidate).State = EntityState.Added;
                }
            }

            bool isSaved = false;
            int recAffected = 0;
            do
                {
                    try
                    {
                        recAffected += await context.SaveChangesAsync();
                        isSaved = true;
                        dtoErr.SuccessString = recAffected + " records copied";
                    }
                    catch (DbUpdateException ex)
                    {
                        foreach (var entry in ex.Entries) {
                            Console.Write("Prospective candidates Exception - " + ex.InnerException.Message);

                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoErr.ErrorString += ex.Message;
                        }
                    }
                    catch (DbException ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }

                    catch (Exception ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }
                }
            while (!isSaved);

            return dtoErr;
        }

        public static async Task<string> ReadCustomerDataExcelFile(this DataContext context, string filePath, string Username)
        {
            //column title in row 2, data starts from row 3
            //string filePath = "D:\\IdealR_/Ideal/api/CandidateExcelData.xlsx";
            string Error="";
            int rowTitle=2;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //DataTable dataTable = new();
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                int intCustomerName=0, intAddress=0, intAddress2=0, intCity=0, intCreatedOn=0;
                int intCountry=0, intIndustryType1=0, intIndustryType2=0, intIndustryType3=0, intStatus=0;
                int intWebsite=0, intEmail=0, intCustomerType=0, intKnownAs=0, intPin=0;
                int intDistrict=0, intState=0, intPhone1=0, intPhone2=0, intIntroduction=0, intIsBlacklisted=0;
                int intAppUserId1=0, intGender1=0, intKnownAs1=0, intOfficialName1=0, intOfficialTitle1=0,intOfficialDesignation1=0,
                    intDept1=0, intMobileNo1=0,intOfficialEmail1=0,intUsername1=0;
                int intAppUserId2=0, intGender2=0, intKnownAs2=0, intOfficialName2=0, intOfficialTitle2=0,
                    intOfficialDesignation2=0, intDept2=0, intMobileNo2=0,intOfficialEmail2=0, intUsername2=0;
                int intAppUserId3=0, intGender3=0, intKnownAs3=0, intOfficialName3=0, intOfficialTitle3=0,
                    intOfficialDesignation3=0, intDept3=0, intMobileNo3=0,intOfficialEmail3=0, intUsername3=0;
                int intCustomerStatus=0;
                
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();

                    switch (colTitle.ToLower()) {
                        case "customertype": case "customer type": intCustomerType=col;break;
                        case "customername": case "customer name": intCustomerName=col; break;
                        case "knownas": case "known as": intKnownAs=col;break;
                        case "add": case "address": intAddress=col;break;
                        case "add2": case "address2": intAddress2=col;break;
                        case "city": intCity=col;break;
                        case "pin": intPin=col;break;
                        case "district": case "dist": intDistrict=col;break;
                        case "state": intState=col;break;
                        case "country": intCountry=col;break;
                        case "email": intEmail=col;break;
                        case "website": intWebsite=col;break;
                        case "phone": intPhone1=col;break;
                        case "phone2": intPhone2=col;break;
                        case "createdon": intCreatedOn=col;break;
                        case "introduction": intIntroduction=col;break;
                        case "customerstatus": case "customer status": intStatus=col;break;
                        case "isblacklisted": intIsBlacklisted=col;break;

                        case "industrytype1": case "industry type 1": case "industry type1": intIndustryType1=col;break;
                        case "industrytype2":  case "industry type 2": case "industry type2": intIndustryType2=col;break;
                        case "industrytype3": case "industry type 3": case "industry type3": intIndustryType3=col;break;

                        case "gender1": intGender1=col;break;

                        case "appuserid1": case "appuserid 1": intAppUserId1=col;break;
                        case "username1": case "user name1": intUsername1=col;break;
                        case "officialname1": case "official name 1": case "officialname 1": case "official name1": intOfficialName1=col;break;
                        case "officialtitle1": case "official title 1": case "officialtitle 1":  intOfficialTitle1=col;break;
                        case "designation1": case "designation 1": intOfficialDesignation1=col;break;
                        case "dept1": case "dept 1": case "department1": case "department 1": intDept1=col;break;
                        case "officialmobile1": case "official mobile1" : case "official mobile 1": intMobileNo1=col;break;
                        case "officialemail1":case "official email1": case "official email 1": intOfficialEmail3=col;break;

                        case "appuserid2": case "appuserid 2":  intAppUserId2=col;break;
                        case "username2": case "user name2":  intUsername2=col;break;
                        case "officialname2":case "official name 2": case "officialname 2": case "official name2": intOfficialName2=col;break;
                        case "officialtitle2": case "officialtitle 2": case "official title 2":intOfficialTitle2=col;break;
                        case "designation2": case "designation 2": intOfficialDesignation2=col;break;
                        case "dept2": case "dept 2": case "department 2": case "department2": intDept2=col;break;
                        case "officialmobile2": case "official mobile2": case "officialmobile 2": intMobileNo2=col;break;
                        case "officialemail2": case "official email2": case "officialemail 2": intOfficialEmail2=col;break;

                        case "appuserid3": intAppUserId3=col;break;
                        case "username3": case "username 3": intUsername3=col;break;
                        case "officialname3": case "official name 3": case "official  name3": case "officialname 3":intOfficialName3=col;break;
                        case "officialtitle3": case "official title 3": case "officialtitle 3": case "official title3": intOfficialTitle3=col;break;
                        case "designation3": case "designation 3": intOfficialDesignation3=col;break;
                        case "dept3": case "dept 3": case "department 3": case "department3": intDept3=col;break;
                        case "officialmobile3": case "official mobile3": intMobileNo3=col;break;
                        case "officialemail3": intOfficialEmail3=col;break;

                        default:break;
                    }
                }

                var customerOfficials = new List<CustomerOfficial>();
                var customerIndustries = new List<CustomerIndustry>();
                var agencySpecialties = new List<AgencySpecialty>();

                for (int row = rowTitle+1; row <= rows; row++)
                {
                    var CustomerType = intCustomerType==0 ? "" : worksheet.Cells[row, intCustomerType].Value?.ToString() ?? "Customer";
                    var CustomerName = intCustomerName==0 ? "" : worksheet.Cells[row, intCustomerName].Value?.ToString() ?? "";
                    if(string.IsNullOrEmpty(CustomerName)) continue;

                    var KnownAs =  intKnownAs==0 ? "" : worksheet.Cells[row, intKnownAs].Value?.ToString() ?? "";
                    var Add = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    var Add2 = intAddress2 == 0 ? "" : worksheet.Cells[row, intAddress2].Value?.ToString() ?? "";
                    var City = intCity==0 ? "" : worksheet.Cells[row, intCity].Value?.ToString() ?? "";
                    var District = intDistrict == 0 ? "" :worksheet.Cells[row, intDistrict].Value?.ToString() ?? "";
                    var State = intState == 0 ? "" : worksheet.Cells[row, intState].Value?.ToString() ?? "";
                    var Country = intCountry == 0 ? "" :  worksheet.Cells[row, intCountry].Value?.ToString() ?? "";
                    var Email = intEmail == 0 ? "" :  worksheet.Cells[row,intEmail].Value?.ToString() ?? "";
                    var Website = intWebsite == 0 ? "" :  worksheet.Cells[row, intWebsite].Value?.ToString() ?? "";
                    var Phone = intPhone1 == 0 ? "" :  worksheet.Cells[row,intPhone1].Value?.ToString() ?? "";
                    var Phone2 = intPhone2 == 0 ? "" :  worksheet.Cells[row, intPhone2].Value?.ToString() ?? "";
                    var CreatedOn =intCreatedOn == 0 ? "" :  worksheet.Cells[row,intCreatedOn].Value?.ToString() ?? "";
                    var Introduction = intIntroduction == 0 ? "" : worksheet.Cells[row, intIntroduction].Value?.ToString() ?? "";
                    var CustomerStatus = intCustomerStatus == 0 ? "" :  worksheet.Cells[row, intCustomerStatus].Value?.ToString();
                    var IsBlacklisted = intIsBlacklisted == 0 ? "" : worksheet.Cells[row, intIsBlacklisted].Value?.ToString();

                    var IndustryType1 = intIndustryType1 == 0 ? "" : worksheet.Cells[row, intIndustryType1].Value?.ToString() ?? "";
                    var IndustryType2 = intIndustryType2 == 0 ? "" : worksheet.Cells[row, intIndustryType2].Value?.ToString() ?? "";
                    var IndustryType3 = intIndustryType3 == 0 ? "" : worksheet.Cells[row, intIndustryType3].Value?.ToString() ?? "";
                    
                    var AppUserId1 = intAppUserId1 == 0 ? "" : worksheet.Cells[row, intAppUserId1].Value?.ToString() ?? "0";
                    var Username1 = intUsername1 == 0 ? "" : worksheet.Cells[row, intUsername1].Value?.ToString() ?? "";
                    var Gender1 = intGender1 == 0 ? "" : worksheet.Cells[row, intGender1].Value?.ToString() ?? "Male";
                    var OfficialTitle1 =intOfficialTitle1 == 0 ? "" :  worksheet.Cells[row, intOfficialTitle1].Value?.ToString() ?? "";
                    var OfficialName1  =intOfficialName1 == 0 ? "" :  worksheet.Cells[row, intOfficialName1].Value?.ToString() ?? "";
                    var OfficialKnownAs1  = intKnownAs1 == 0 ? "" :  worksheet.Cells[row, intKnownAs1].Value?.ToString() ?? "";
                    var Designation1 = intOfficialDesignation1 == 0 ? "" :  worksheet.Cells[row, intOfficialDesignation1].Value?.ToString() ?? "";
                    var Dept1 = intDept1 == 0 ? "" :  worksheet.Cells[row, intDept1].Value?.ToString() ?? "";
                    var OfficialMobile1 = intMobileNo1 == 0 ? "" : worksheet.Cells[row, intMobileNo1].Value?.ToString() ?? "";
                    var OfficialEmail1 = intOfficialEmail1 == 0 ? "" :  worksheet.Cells[row, intOfficialEmail1].Value?.ToString() ?? "";
                    
                    var AppUserId2 = intAppUserId2 == 0 ? "" : worksheet.Cells[row, intAppUserId2].Value?.ToString() ?? "0";
                    var Username2 = intUsername2 == 0 ? "" : worksheet.Cells[row, intUsername2].Value?.ToString() ?? "";
                    var Gender2 =intGender2 == 0 ? "" :  worksheet.Cells[row, intGender2].Value?.ToString() ?? "Male";
                    var OfficialTitle2 = intOfficialTitle2 == 0 ? "" :  worksheet.Cells[row, intOfficialTitle2].Value?.ToString() ?? "";
                    var OfficialName2  = intOfficialName2 == 0 ? "" : worksheet.Cells[row, intOfficialName2].Value?.ToString() ?? "";
                    var OfficialKnownAs2  = intKnownAs2 == 0 ? "" : worksheet.Cells[row, intKnownAs2].Value?.ToString() ?? "";
                    var Designation2 = intOfficialDesignation2 == 0 ? "" : worksheet.Cells[row, intOfficialDesignation2].Value?.ToString() ?? "";
                    var Dept2 = intDept2 == 0 ? "" : worksheet.Cells[row, intDept2].Value?.ToString() ?? "";
                    var OfficialMobile2 = intMobileNo2 == 0 ? "" : worksheet.Cells[row, intMobileNo2].Value?.ToString() ?? "";
                    var OfficialEmail2 = intOfficialEmail2 == 0 ? "" : worksheet.Cells[row, intOfficialEmail2].Value?.ToString() ?? "";

                    var AppUserId3 = intAppUserId3 == 0 ? "" : worksheet.Cells[row, intAppUserId3].Value?.ToString() ?? "0";
                    var Username3 = intUsername3 == 0 ? "" : worksheet.Cells[row, intUsername3].Value?.ToString() ?? "";
                    var Gender3 = intGender3 == 0 ? "" : worksheet.Cells[row, intGender3].Value?.ToString() ?? "Male";
                    var OfficialTitle3 = intOfficialTitle3 == 0 ? "" :  worksheet.Cells[row, intOfficialTitle3].Value?.ToString() ?? "";
                    var OfficialName3  = intOfficialName3 == 0 ? "" : worksheet.Cells[row, intOfficialName3].Value?.ToString() ?? "";
                    var OfficialKnownAs3  = intKnownAs3 == 0 ? "" : worksheet.Cells[row, intKnownAs3].Value?.ToString() ?? "";
                    var Designation3 = intOfficialDesignation3 == 0 ? "" : worksheet.Cells[row, intOfficialDesignation3].Value?.ToString() ?? "";
                    var Dept3 = intDept3 == 0 ? "" : worksheet.Cells[row, intDept3].Value?.ToString() ?? "";
                    var OfficialMobile3 = intMobileNo3 == 0 ? "" :  worksheet.Cells[row, intMobileNo3].Value?.ToString() ?? "";
                    var OfficialEmail3 = intOfficialEmail3 == 0 ? "" : worksheet.Cells[row, intOfficialEmail3].Value?.ToString() ?? "";


                    if (!DateTime.TryParse(CreatedOn, out DateTime createdon))
                    {
                        createdon = new DateTime();
                    }

                    if(!string.IsNullOrEmpty(OfficialName1)) {
                        var Official = new CustomerOfficial {
                            AppUserId=Convert.ToInt32(AppUserId1), Gender= Gender1, Title=OfficialTitle1,
                            OfficialName=OfficialName1, KnownAs=OfficialKnownAs1, UserName = Username1,
                            Designation = Designation1, Divn = Dept1, Mobile = OfficialMobile1, 
                            Email = OfficialEmail1
                        };
                        customerOfficials.Add(Official);
                    }

                    if(!string.IsNullOrEmpty(OfficialName2)) {
                        var Official = new CustomerOfficial {
                            AppUserId=Convert.ToInt32(AppUserId2), Gender= Gender2, Title=OfficialTitle2,
                            OfficialName=OfficialName2, KnownAs=OfficialKnownAs2, UserName = Username2,
                            Designation = Designation2, Divn = Dept2, Mobile = OfficialMobile2, 
                            Email = OfficialEmail2
                        };
                        customerOfficials.Add(Official);
                    }

                    if(!string.IsNullOrEmpty(OfficialName3)) {
                        var Official = new CustomerOfficial {
                            AppUserId=Convert.ToInt32(AppUserId3), Gender= Gender3, Title=OfficialTitle3,
                            OfficialName=OfficialName3, KnownAs=OfficialKnownAs3, UserName = Username3,
                            Designation = Designation3, Divn = Dept3, Mobile = OfficialMobile3, 
                            Email = OfficialEmail3
                        };
                        customerOfficials.Add(Official);
                    }
                    
                    if(!string.IsNullOrEmpty(IndustryType1)) {
                        var industry = new CustomerIndustry {Industry = new Industry {IndustryName = IndustryType1}};
                        customerIndustries.Add(industry);
                    }

                    if(!string.IsNullOrEmpty(IndustryType2)) {
                        var industry = new CustomerIndustry {Industry = new Industry {IndustryName = IndustryType2}};
                        customerIndustries.Add(industry);
                    }
                    if(!string.IsNullOrEmpty(IndustryType3)) {
                        var industry = new CustomerIndustry {Industry = new Industry {IndustryName = IndustryType3}};
                        customerIndustries.Add(industry);
                    }

                    var newCustomer = new Customer
                    {
                        CustomerType = CustomerType, CustomerName = CustomerName, KnownAs = KnownAs,
                        Add = Add, Add2 = Add2, City = City, District = District, State = State,
                        Country = Country, Email = Email, Website = Website, Phone = Phone, Phone2=Phone2,
                        CreatedOn = createdon, Introduction = Introduction, CustomerStatus = CustomerStatus,
                        
                        IsBlackListed = Convert.ToBoolean(IsBlacklisted), CustomerIndustries = customerIndustries,
                        CustomerOfficials = customerOfficials
                    };      

                    context.Entry(newCustomer).State = EntityState.Added;
                   
                }
            }
             
            int EntriesFailed=0;
            //bool isSaved = false;
            //do {
                try {
                    await context.SaveChangesAsync();
                    //isSaved = true;
                } catch (DbUpdateException ex) {
                    /*foreach (var entry in ex.Entries) {
                        entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                        Error += ex.Message;
                        EntriesFailed ++;
                    }*/
                    Error = ex.Message;
                }
            //} while (!isSaved);
            
            if(!string.IsNullOrEmpty(Error)) Error += "Total Entries Failed:" + EntriesFailed + ". ";
            return Error;
        }
        
        public static async Task<ReturnStringsDto> ReadProfessionDataExcelFile(this DataContext context, string filePath, string Username)
        {
            //column titles in row 4, data starts from row 5
            var dtoErr = new ReturnStringsDto();

            int rowTitle=1;   //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                int rows=0, columns=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["Sheet1"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    dtoErr.ErrorString = ex.Message;
                    return dtoErr;
                }
                
                //DataTable dataTable = new();
                
                int intProfessionName=0, intProfessionGroup=0;
    
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "professionname" :case "profession name": intProfessionName=col; break;
                        case "professiongroup": case "profession group": intProfessionGroup=col;break;
                        default:break;
                    }
                }
                
                if (intProfessionGroup == 0 || intProfessionGroup == 0) {
                    dtoErr.ErrorString = "Failed to identify Field Names in the file";
                    return dtoErr;
                }

                string ProfessionName = "", ProfessionGroup = "";
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    ProfessionName = intProfessionName == 0 ? "" : worksheet.Cells[row, intProfessionName].Value?.ToString() ?? "";
                    ProfessionGroup = intProfessionGroup == 0 ? "" : worksheet.Cells[row, intProfessionGroup].Value?.ToString() ?? "";

                    var newProfession = new Profession
                    {
                        ProfessionName = ProfessionName,
                        ProfessionGroup = ProfessionGroup
                    };

                    context.Entry(newProfession).State = EntityState.Added;
                }
            }

            bool isSaved = false;
            int recAffected = 0;
            do
                {
                    try
                    {
                        recAffected += await context.SaveChangesAsync();
                        isSaved = true;
                        dtoErr.SuccessString = recAffected + " records copied";
                    }
                    catch (DbUpdateException ex)
                    {
                        foreach (var entry in ex.Entries) {
                            Console.Write("Profession Exception - " + ex.InnerException.Message);

                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoErr.ErrorString += ex.Message;
                        }
                    }
                    catch (DbException ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }

                    catch (Exception ex)
                    {
                        dtoErr.ErrorString += ex.Message;
                    }
                }
            while (!isSaved);

            return dtoErr;
        }


    }
}