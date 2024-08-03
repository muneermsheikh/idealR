using System.Data.Common;
using api.Data;
using api.Entities.Admin;
using api.Entities.HR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Extensions
{
    public static class ReadEmployeexcelSheet
    {   
        public static async Task<string> ReadEmployeeExcelFile(this DataContext context, string filePath, string Username)
        {
            //column titles in row 4, data starts from row 5
            var strError="";
            int rowTitle=4;     //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows=0, columns=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["Employees"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    strError = ex.Message;
                    return strError;
                }
                                
                //DataTable dataTable = new();
                int intGender=0, intFirstName=0, intSecondName=0, intFamilyName=0, intKnownAs=0, intUsername=0;
                int intPosition=0, intDOB=0, intPlaceOfBirth=0, intAadharNo=0;
                int intNationality=0, intOfficialEmail=0, intOfficialPhoneNo=0, intOfficialMobileNo=0;
                int intDOJ=0, intDepartment=0;
                int intQualification=0, intAddress=0, intAddress2=0, intCity=0, intCountry=0;
                int intHRSkill1=0, intHRSkill2=0, intHRSkill3=0, intRole1=0, intRole2=0, intRole3=0;
                //int intOtherSkill1=0, intOtherSkill2=0, intOtherSkill3=0;

                var DOJ = DateTime.Now;
                
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "gender": intGender=col; break;
                        case "firstname": case "first name": case "name": intFirstName=col;break;
                        case "secondname": case "second name": case "father's name": case "father name":  intSecondName=col;break;
                        case "familyname": case "family name": case "surname": case "sur name": intFamilyName=col;break;
                        case "knownas": case "known as": intKnownAs=col;break;
                        case "username": case "user name": intUsername=col;break;
                        case "position": case "designation": intPosition=col;break;
                        case "qualification": intQualification=col;break;
                        case "dateofbirth": case "date of birth": case "dob": case "d.o.b.": intDOB=col;break;
                        case "placeofbirth": case "place of birth": case "birth place": intPlaceOfBirth=col;break;
                        case "aadharno": case "aadhar no": case "aadhar": intAadharNo=col;break;
                        case "nationality": intNationality=col;break;
                        case "officialemail" : case "official email": case "email": intOfficialEmail=col;break;
                        case "officialphoneno": case "official phone no": case "phone no" : intOfficialPhoneNo=col;break;
                        case "officialmobileno": case "official mobile no": case "mobile no": case "mobileno":  intOfficialMobileNo=col;break;
                        case "dateofjoining": case "date of joining": case "doj": case "joined on":  intDOJ=col;break;

                        case "department": case "dept": case "divn": case "division": intDepartment=col;break;
                        case "address": intAddress=col;break;
                        case "address2": case "address 2": intAddress2=col;break;
                        case "city": intCity=col;break;
                        case "country": intCountry=col; break;
                        case "hrskill1": case "hr skill 1": intHRSkill1=col;break;
                        case "hrskill2": case "hr skill 2": intHRSkill2=col;break;
                        case "hrskill3": case "hr skill 3": intHRSkill3=col;break;
                        case "role 1": intRole1=col;break;
                        case "role 2": intRole2=col;break;
                        case "role 3": intRole3=col;break;
                        default:break;
                    }
                }
                
                string Gender = "", FirstName = "", SecondName = "", FamilyName = "", KnownAs = "";
                string Position = "", Qualification = "", DateOfBirth = "", PlaceOfBirth = "";
                string AadharNo = "", Nationality="", OfficialEmail="";
                string OfficialPhoneNo="", OfficialMobileNo="", DateOfJoining="";
                string Department="", Address="", Address2="", City="", Country=""; 
                string HRSkill1="", HRSkill2="", HRSkill3="";
                string Role1="", Role2="", Role3="";
                //string OtherSkill1="", OtherSkill2="", OtherSkill3="";

                for (int row = rowTitle+1; row <= rows; row++)
                {
                    FirstName = intFirstName == 0 ? "" : worksheet.Cells[row, intFirstName].Value.ToString() ?? "";
                    SecondName = intSecondName == 0 ? "" : worksheet.Cells[row, intSecondName].Value.ToString() ?? "";
                    FamilyName = intFamilyName == 0 ? "" : worksheet.Cells[row, intFamilyName].Value.ToString() ?? "";
                    KnownAs = intKnownAs == 0 ? "" : worksheet.Cells[row, intKnownAs].Value.ToString() ?? "";
                    Username = intUsername == 0 ? "" : worksheet.Cells[row, intUsername].Value.ToString() ?? "";
                    Position = intPosition == 0 ? "": worksheet.Cells[row, intPosition].Value.ToString()  ?? "";
                    Qualification = intQualification == 0 ? "" : worksheet.Cells[row, intQualification].Value.ToString() ?? "";
                    DateOfBirth = intDOB == 0 ? "" : worksheet.Cells[row, intDOB].Value.ToString() ?? "";
                    PlaceOfBirth = intPlaceOfBirth == 0 ? "" : worksheet.Cells[row, intPlaceOfBirth].Value.ToString() ?? "";
                    AadharNo = intAadharNo == 0 ? "" : worksheet.Cells[row, intAadharNo].Value.ToString() ?? "";
                    Gender = Gender == "m" ? "Male" : "Female";
                    Address = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value.ToString() ?? "";
                    Address2 = intAddress2 == 0 ? "" : worksheet.Cells[row, intAddress2].Value.ToString() ?? "";
                    City = intCity == 0 ? "" : worksheet.Cells[row, intCity].Value.ToString() ?? "";
                    Country = intCountry == 0 ? "India" : worksheet.Cells[row, intCountry].Value.ToString() ?? "India";
                    OfficialEmail = intOfficialEmail == 0 ? "" : worksheet.Cells[row, intOfficialEmail].Value.ToString() ?? "";
                    OfficialPhoneNo = intOfficialPhoneNo == 0 ? "" : worksheet.Cells[row, intOfficialPhoneNo].Value.ToString() ?? "";
                    OfficialMobileNo = intOfficialMobileNo == 0 ? "" : worksheet.Cells[row, intOfficialMobileNo].Value.ToString() ?? "";
                    DateOfJoining = intDOJ == 0 ? "" : worksheet.Cells[row, intDOJ].Value.ToString() ?? "";
                    Department = intDepartment == 0 ? "" : worksheet.Cells[row, intDepartment].Value.ToString() ?? "";
                    HRSkill1 = intHRSkill1 == 0 ? "" : worksheet.Cells[row, intHRSkill1].Value.ToString() ?? "";
                    HRSkill2 = intHRSkill2 == 0 ? "" : worksheet.Cells[row, intHRSkill2].Value.ToString() ?? "";
                    HRSkill3 = intHRSkill3 == 0 ? "" : worksheet.Cells[row, intHRSkill3].Value.ToString() ?? "";
                    Role1 = intRole1 == 0 ? "" : worksheet.Cells[row, intRole1].Value.ToString() ?? "";
                    Role2 = intRole2 == 0 ? "" : worksheet.Cells[row, intRole2].Value.ToString() ?? "";
                    Role3 = intRole3 == 0 ? "" : worksheet.Cells[row, intRole3].Value.ToString() ?? "";
                    
                    var newEmployee = new Employee
                    {
                        Gender = Gender, FirstName = FirstName, SecondName = SecondName,
                        FamilyName = FamilyName, KnownAs = KnownAs, UserName = Username,
                        Position = Position, Qualifications = Qualification, PlaceOfBirth=PlaceOfBirth,
                        AadharNo = AadharNo, Nationality = Nationality, OfficialEmail=OfficialEmail,
                        OfficialPhoneNo = OfficialPhoneNo, OfficialMobileNo = OfficialMobileNo,
                        Department = Department, Address = Address, Address2 = Address2, 
                        City = City, Country = Country
                    };
                    
                    
                    if (!DateTime.TryParse(DateOfBirth, out DateTime dob)) {
                        dob = new DateTime();
                        //if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                    }
                    newEmployee.DateOfBirth=dob;

                    
                    if (!DateTime.TryParse(DateOfJoining, out DateTime doj))  {
                        doj = new DateTime();
                        //if(dob.Year < 1900) dob = DateTime.Now.AddYears(-Convert.ToInt32(Age[..2]));
                    }
                    newEmployee.DateOfJoining=doj;

                    var hrskills = new List<HRSkill>();
                    if(!string.IsNullOrEmpty(HRSkill1)) hrskills.Add(new HRSkill{ProfessionName=HRSkill1});
                    if(!string.IsNullOrEmpty(HRSkill2)) hrskills.Add(new HRSkill{ProfessionName=HRSkill2});
                    if(!string.IsNullOrEmpty(HRSkill3)) hrskills.Add(new HRSkill{ProfessionName=HRSkill3});

                    newEmployee.HRSkills = hrskills;

                    //OtherSkills to update manually.

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

    }
}