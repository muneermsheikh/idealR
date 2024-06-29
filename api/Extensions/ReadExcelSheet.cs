using System.Data;
using System.Data.Common;
using System.Text;
using api.Data;
using api.Entities.Admin.Client;
using api.Entities.HR;
using api.Entities.Master;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SQLitePCL;

namespace api.Extensions
{
    public static class ReadExcelSheet
    {
        static void ReadExcelFile()
        {
            try
            {
                using SpreadsheetDocument doc = SpreadsheetDocument.Open("../Assets/1049_1_Fire_and_Safety_Engineer-02June2022-0-14.xlsx", false);
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                StringBuilder excelResult = new();

                // Iterate through each sheet
                foreach (Sheet thesheet in thesheetcollection.Cast<Sheet>())
                {
                    excelResult.AppendLine("Excel Sheet Name: " + thesheet.Name);
                    excelResult.AppendLine("-----------------------------------------------");

                    // Get the worksheet object by using the sheet ID
                    Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;
                    SheetData thesheetdata = theWorksheet.GetFirstChild<SheetData>();

                    // Iterate through rows and cells
                    foreach (Row thecurrentrow in thesheetdata.Cast<Row>())
                    {
                        foreach (Cell thecurrentcell in thecurrentrow.Cast<Cell>())
                        {                            
                            // Take the cell value
                            string currentcellvalue = string.Empty;
                            if (thecurrentcell.DataType != null)
                            {
                                if (thecurrentcell.DataType == CellValues.SharedString)
                                {
                                    if (Int32.TryParse(thecurrentcell.InnerText, out int id))
                                    {
                                        SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                        if (item.Text != null)
                                        {
                                            // Code to take the string value
                                            excelResult.Append(item.Text.Text + " ");
                                        }
                                        else if (item.InnerText != null)
                                        {
                                            currentcellvalue = item.InnerText;
                                        }
                                        else if (item.InnerXml != null)
                                        {
                                            currentcellvalue = item.InnerXml;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                excelResult.Append(Convert.ToInt16(thecurrentcell.InnerText) + " ");
                            }
                        }
                        excelResult.AppendLine();
                    }
                    excelResult.Append("");
                    Console.WriteLine(excelResult.ToString());
                    Console.ReadLine();

                }
            }
            catch (Exception)
            {
                // Handle exceptions
            }
        }

        public static async Task<string> ReadProspectiveCandidateDataExcelFile(this DataContext context, string filePath, string Username)
        {
            // var CategoryRef - label in row2, column1, data in row2, column 2
            // var OrderItemId = label in row3, col1, data in row3, col2 ;
            // var professionId = label in row2, col3; data in row2, col4
            //var ProfessionName = label in row2, col 5, data in row2, col6
            //column titles in row 4, data starts from row 5
            var strError="";
            int rowTitle=5;
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
                
                string CategoryRef="", ProfessionName="";
                int OrderItemId = 0, ProfessionId = 0;

                for(int col=1; col <= 2; col++){
                    try {
                        var colTitle = worksheet.Cells[3, col].Value.ToString();
                        switch (colTitle.ToLower()) {
                            case "orderitemid": case "order item id": case "orderitem id": OrderItemId=Convert.ToInt32((worksheet.Cells[3, col+1].Value ?? "0").ToString()); break;
                            default:break;
                        }
                    } catch (Exception ex) {
                        return ex.Message;
                    }
                }

                if(OrderItemId == 0) return "OrderItemId not defined";

                var query = await (from item in context.OrderItems where item.Id == OrderItemId
                        join order in context.Orders on item.OrderId equals order.Id
                        join cat in context.Professions on item.ProfessionId equals cat.Id
                        select new {ProfessionId=item.ProfessionId, 
                        CategoryRef=order.OrderNo + "-" + item.SrNo + cat.ProfessionName,
                        ProfessionName=cat.ProfessionName}
                ).FirstOrDefaultAsync();

                if(query==null) return "Invalid Order Item Id";

                ProfessionName = query.ProfessionName;
                CategoryRef = query.CategoryRef;
                ProfessionId = query.ProfessionId;
                
                //DataTable dataTable = new();
                
                int intCandidateName=0, intEmail=0, intAlternateEmail=0, intAlternatePhone=0, intDob=0;
                int intMobileNo=0, intAlternateNo=0, intResumeTitle=0, intKeySkills=0, intWorkExp=0, intCurrentLocation=0;
                int intEducation=0, intGender=0, intAge=0, intAddress=0, intResumeId=0,intDesignation=0;

                /*for (int i = 1; i <= columns; i++)
                {
                    dataTable.Columns.Add("Column" + i);
                }
                */
                
                var Source = "TimesJobs";
                var DateRegistered = DateTime.Now;
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value.ToString();
                    switch (colTitle.ToLower()) {
                        case "candidatename" :case "name": intCandidateName=col; break;
                        case "emailid": case "email id": intEmail=col;break;
                        case "alternateemailid": case "alternate email": case "alternte emailid": intAlternateEmail=col;break;
                        case "alternatephone": intAlternatePhone=col;break;
                        case "dob": intDob=col;break;
                        case "mobileno": case "mobile no": case "mobile no.": intMobileNo=col;break;
                        case "alternatenumber": intAlternateNo=col;break;
                        case "resumetitle": intResumeTitle=col;break;
                        case "keyskills": intKeySkills=col;break;
                        case "workexp": case "work experience": intWorkExp=col;break;
                        case "currentlocation" : case "current location": intCurrentLocation=col;break;
                        case "education": case "course": case "course1" : intEducation=col;break;
                        case "gender": intGender=col;break;
                        case "age": intAge=col;break;
                        case "address": intAddress=col;break;
                        case "resumeid": case "resume id": intResumeId=col;break;
                        case "designation": intDesignation=col;break;
                        default:break;
                    }
                }
                
                string CandidateName = "", EmailId = "", AlternateEmailId = "", AlternatePhone = "", DOB = "";
                string MobileNo = "", AlternateNumber = "", ResumeTitle = "", KeySkills = "", WorkExp = "", CurrentLocation = "";
                string Education = "", Gendr = "", Age = "", Address = "", ResumeId = "", Designation = "";
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    //DataRow dataRow = dataTable.NewRow();
                    
                    CandidateName = intCandidateName == 0 ? "" : worksheet.Cells[row, intCandidateName].Value.ToString() ?? "";
                    EmailId = intEmail == 0 ? "" : worksheet.Cells[row, intEmail].Value.ToString() ?? "";
                    AlternateEmailId = intAlternateEmail == 0 ? "" : worksheet.Cells[row, intAlternateEmail].Value.ToString() ?? "";
                    AlternatePhone = intAlternatePhone == 0 ? "" : worksheet.Cells[row, intAlternatePhone].Value.ToString() ?? "";
                    DOB = intDob == 0 ? "" : worksheet.Cells[row, intDob].Value.ToString() ?? "";
                    MobileNo = intMobileNo == 0 ? "": worksheet.Cells[row, intMobileNo].Value.ToString()  ?? "";
                    AlternateNumber = intAlternatePhone == 0 ? "" : worksheet.Cells[row, intAlternateNo].Value.ToString() ?? "";
                    ResumeTitle = intResumeTitle == 0 ? "" : worksheet.Cells[row, intResumeTitle].Value.ToString() ?? "";
                    KeySkills = intKeySkills == 0 ? "" : worksheet.Cells[row, intKeySkills].Value.ToString() ?? "";
                    WorkExp = intWorkExp == 0 ? "" : worksheet.Cells[row, intWorkExp].Value.ToString() ?? "";
                    Address = intAddress == 0 ? "": worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    CurrentLocation = intCurrentLocation == 0 ? "" : worksheet.Cells[row, intCurrentLocation].Value.ToString() ?? "";
                    Education = intEducation == 0 ? "" : worksheet.Cells[row, intEducation].Value.ToString() ?? "";
                    Gendr = intGender == 0 ? "Male" : worksheet.Cells[row, intGender].Value.ToString() ?? "Male";
                    Gendr = Gendr == "m" ? "Male" : "Female";
                    Age =  intAge == 0 ? "" : worksheet.Cells[row, intAge].Value.ToString() ?? "";
                    Address = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    ResumeId = intResumeId == 0 ? "" : worksheet.Cells[row, intResumeId].Value.ToString() ?? "";
                    Designation = intDesignation == 0 ? "" : worksheet.Cells[row, intDesignation].Value.ToString() ?? "";
                    
                    if (!DateTime.TryParse(DOB, out DateTime dob))
                    {
                        dob = new DateTime();
                        if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                    }
                
                    var newProspectiveCandidate = new ProspectiveCandidate
                    {
                        CategoryRef = CategoryRef,
                        OrderItemId = OrderItemId,
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        CandidateName = CandidateName ,
                        PersonId = ResumeId,
                        Source = Source,
                        DateOfBirth = dob,
                        Age = Age,
                        Address = Address,
                        PhoneNo = MobileNo,
                        AlternateNumber = AlternateNumber,
                        CurrentLocation = CurrentLocation,
                        Email = EmailId,
                        AlternateEmail = AlternateEmailId,
                        ProfessionId = ProfessionId,
                        ProfessionName = ProfessionName,
                        DateRegistered = DateRegistered,
                        ResumeTitle = ResumeTitle,
                        Education = Education,
                        WorkExperience = WorkExp,
                        Username=Username
                    };

                    context.Entry(newProspectiveCandidate).State = EntityState.Added;
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

        static async Task<int> ReadCustomerDataExcelFile(this DataContext context, string filePath)
        {
            //column title in row 2, data starts from row 3
            //string filePath = "D:\\IdealR_/Ideal/api/CandidateExcelData.xlsx";

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new();
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
                for (int i = 1; i <= columns; i++)
                {
                    dataTable.Columns.Add("Column" + i);
                }
                
                for(int col=3; col <= columns; col++) {
                    var colTitle = worksheet.Cells[5, col].Value.ToString();
                    switch (colTitle.ToLower()) {
                        case "customertype": intCustomerType=col;break;
                        case "customername": intCustomerName=col; break;
                        case "knownas": intKnownAs=col;break;
                        case "add": intAddress=col;break;
                        case "add2": intAddress2=col;break;
                        case "city": intCity=col;break;
                        case "pin": intPin=col;break;
                        case "district": intDistrict=col;break;
                        case "state": intState=col;break;
                        case "country": intCountry=col;break;
                        case "email": intEmail=col;break;
                        case "website": intWebsite=col;break;
                        case "phone": intPhone1=col;break;
                        case "phone2": intPhone2=col;break;
                        case "createdon": intCreatedOn=col;break;
                        case "introduction": intIntroduction=col;break;
                        case "customerstatus": intStatus=col;break;
                        case "isblacklisted": intIsBlacklisted=col;break;

                        case "industrytype1": intIndustryType1=col;break;
                        case "industrytype2": intIndustryType2=col;break;
                        case "industrytype3": intIndustryType3=col;break;

                        case "gender1": intGender1=col;break;

                        case "appuserid1": intAppUserId1=col;break;
                        case "username1": intUsername1=col;break;
                        case "officialname1": intOfficialName1=col;break;
                        case "officialtitle1": intOfficialName1=col;break;
                        case "designation1": intOfficialDesignation1=col;break;
                        case "dept1": intDept1=col;break;
                        case "officialmobile1": intMobileNo1=col;break;
                        case "officialemail1": intOfficialEmail3=col;break;

                        case "appuserid2": intAppUserId2=col;break;
                        case "username2": intUsername2=col;break;
                        case "officialname2": intOfficialName2=col;break;
                        case "officialtitle2": intOfficialName2=col;break;
                        case "designation2": intOfficialDesignation2=col;break;
                        case "dept2": intDept2=col;break;
                        case "officialmobile2": intMobileNo2=col;break;
                        case "officialemail2": intOfficialEmail2=col;break;

                        case "appuserid3": intAppUserId3=col;break;
                        case "username3": intUsername3=col;break;
                        case "officialname3": intOfficialName3=col;break;
                        case "officialtitle3": intOfficialTitle3=col;break;
                        case "designation3": intOfficialDesignation3=col;break;
                        case "dept3": intDept3=col;break;
                        case "officialmobile3": intMobileNo3=col;break;
                        case "officialemail3": intOfficialEmail3=col;break;

                        default:break;
                    }
                }

                var customerOfficials = new List<CustomerOfficial>();
                var customerIndustries = new List<CustomerIndustry>();
                var agencySpecialties = new List<AgencySpecialty>();

                for (int row = 2; row <= rows; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    var CustomerType = worksheet.Cells[row, intCustomerType].Value.ToString() ?? "Customer";
                    var CustomerName = worksheet.Cells[row, intCustomerName].Value.ToString() ?? "";
                    var KnownAs = worksheet.Cells[row, intKnownAs].Value.ToString() ?? "";
                    var Add = worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    var Add2 = worksheet.Cells[row, intAddress2].Value.ToString() ?? "";
                    var City = worksheet.Cells[row, intCity].Value.ToString() ?? "";
                    var District = worksheet.Cells[row, intDistrict].Value.ToString() ?? "";
                    var State = worksheet.Cells[row, intState].Value.ToString() ?? "";
                    var Country = worksheet.Cells[row, intCountry].Value.ToString() ?? "";
                    var Email = worksheet.Cells[row,intEmail].Value.ToString() ?? "";
                    var Website = worksheet.Cells[row, intWebsite].Value.ToString() ?? "";
                    var Phone = worksheet.Cells[row,intPhone1].Value.ToString() ?? "";
                    var Phone2 = worksheet.Cells[row, intPhone2].Value.ToString() ?? "";
                    var CreatedOn = worksheet.Cells[row,intCreatedOn].Value.ToString() ?? "";
                    var Introduction = worksheet.Cells[row, intIntroduction].Value.ToString() ?? "";
                    var CustomerStatus = worksheet.Cells[row, intCustomerStatus].Value.ToString();
                    var IsBlacklisted = worksheet.Cells[row, intIsBlacklisted].Value.ToString();

                    var IndustryType1 = worksheet.Cells[row, intIndustryType1].Value.ToString() ?? "";
                    var IndustryType2 = worksheet.Cells[row, intIndustryType2].Value.ToString() ?? "";
                    var IndustryType3 = worksheet.Cells[row, intIndustryType3].Value.ToString() ?? "";
                    
                    var AppUserId1 = worksheet.Cells[row, intAppUserId1].Value.ToString() ?? "0";
                    var Username1 = worksheet.Cells[row, intUsername1].Value.ToString() ?? "";
                    var Gender1 = worksheet.Cells[row, intGender1].Value.ToString() ?? "Male";
                    var OfficialTitle1 = worksheet.Cells[row, intOfficialTitle1].Value.ToString() ?? "";
                    var OfficialName1  = worksheet.Cells[row, intOfficialName1].Value.ToString() ?? "";
                    var OfficialKnownAs1  = worksheet.Cells[row, intKnownAs1].Value.ToString() ?? "";
                    var Designation1 = worksheet.Cells[row, intOfficialDesignation1].Value.ToString() ?? "";
                    var Dept1 = worksheet.Cells[row, intDept1].Value.ToString() ?? "";
                    var OfficialMobile1 = worksheet.Cells[row, intMobileNo1].Value.ToString() ?? "";
                    var OfficialEmail1 = worksheet.Cells[row, intOfficialEmail1].Value.ToString() ?? "";
                    
                    var AppUserId2 = worksheet.Cells[row, intAppUserId2].Value.ToString() ?? "0";
                    var Username2 = worksheet.Cells[row, intUsername2].Value.ToString() ?? "";
                    var Gender2 = worksheet.Cells[row, intGender2].Value.ToString() ?? "Male";
                    var OfficialTitle2 = worksheet.Cells[row, intOfficialTitle2].Value.ToString() ?? "";
                    var OfficialName2  = worksheet.Cells[row, intOfficialName2].Value.ToString() ?? "";
                    var OfficialKnownAs2  = worksheet.Cells[row, intKnownAs2].Value.ToString() ?? "";
                    var Designation2 = worksheet.Cells[row, intOfficialDesignation2].Value.ToString() ?? "";
                    var Dept2 = worksheet.Cells[row, intDept2].Value.ToString() ?? "";
                    var OfficialMobile2 = worksheet.Cells[row, intMobileNo2].Value.ToString() ?? "";
                    var OfficialEmail2 = worksheet.Cells[row, intOfficialEmail2].Value.ToString() ?? "";

                    var AppUserId3 = worksheet.Cells[row, intAppUserId3].Value.ToString() ?? "0";
                    var Username3 = worksheet.Cells[row, intUsername3].Value.ToString() ?? "";
                    var Gender3 = worksheet.Cells[row, intGender3].Value.ToString() ?? "Male";
                    var OfficialTitle3 = worksheet.Cells[row, intOfficialTitle3].Value.ToString() ?? "";
                    var OfficialName3  = worksheet.Cells[row, intOfficialName3].Value.ToString() ?? "";
                    var OfficialKnownAs3  = worksheet.Cells[row, intKnownAs3].Value.ToString() ?? "";
                    var Designation3 = worksheet.Cells[row, intOfficialDesignation3].Value.ToString() ?? "";
                    var Dept3 = worksheet.Cells[row, intDept3].Value.ToString() ?? "";
                    var OfficialMobile3 = worksheet.Cells[row, intMobileNo3].Value.ToString() ?? "";
                    var OfficialEmail3 = worksheet.Cells[row, intOfficialEmail3].Value.ToString() ?? "";


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

            var recAffected=await context.SaveChangesAsync();
            return recAffected;
        }
    }
}