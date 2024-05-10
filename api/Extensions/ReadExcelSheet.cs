using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using api.Data;
using api.Entities.HR;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Extensions
{
    public static class ReadExcelSheet
    {
        static void ReadExcelFile()
        {
            try
            {
                using SpreadsheetDocument doc = SpreadsheetDocument.Open("testdata.xlsx", false);
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                StringBuilder excelResult = new StringBuilder();

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

        static async Task<int> ReadCandidateDataExcelFile(this DataContext context)
        {
            string filePath = "D:\\IdealR_/Ideal/api/CandidateExcelData.xlsx";

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new();
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                for (int i = 1; i <= columns; i++)
                {
                    dataTable.Columns.Add("Column" + i);
                }

                for (int row = 2; row <= rows; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    var Gendr = worksheet.Cells[row, 1].Value.ToString();
                    var FirstName = worksheet.Cells[row, 2].Value.ToString();
                    var SecondName = worksheet.Cells[row, 3].Value.ToString();
                    var FamilyName = worksheet.Cells[row, 4].Value.ToString();
                    var KnownAs = worksheet.Cells[row, 5].Value.ToString();
                    var Username = worksheet.Cells[row, 6].Value.ToString();
                    var Source = worksheet.Cells[row, 7].Value.ToString();
                    var PassportNo = worksheet.Cells[row, 8].Value.ToString();
                    var ECNR = worksheet.Cells[row, 9].Value.ToString();
                    var Address = worksheet.Cells[row,10].Value.ToString();
                    var City = worksheet.Cells[row, 11].Value.ToString();
                    var Pin = worksheet.Cells[row,12].Value.ToString();
                    var Country = worksheet.Cells[row, 13].Value.ToString();
                    var Nationality = worksheet.Cells[row,14].Value.ToString();
                    var PhoneNo = worksheet.Cells[row, 15].Value.ToString();
                    var MobileNo = worksheet.Cells[row, 16].Value.ToString();
                    var Email = worksheet.Cells[row, 17].Value.ToString();
                    var AssociateId = worksheet.Cells[row, 18].Value.ToString();
                    var AssociateName = worksheet.Cells[row, 19].Value.ToString();
                    var ProfessionId = worksheet.Cells[row, 20].Value.ToString();
                    var ProfessionName = worksheet.Cells[row, 21].Value.ToString();
                    var DOB = worksheet.Cells[row, 22].Value.ToString();

                    if (!DateOnly.TryParse(DOB, out DateOnly dob))
                    {
                        dob = new DateOnly(0001, 1, 1);
                    }

                    var userPhones = new List<UserPhone>();
                    var userphones = new List<UserPhone>{new() { MobileNo=MobileNo, IsMain=true}};
                    var userProfessions = new List<UserProfession>();
                    if(!string.IsNullOrEmpty(ProfessionId)) 
                    {
                        var profid = Convert.ToInt32(ProfessionId);
                        if(await context.Professions.FindAsync(profid) != null) 
                        {
                            userProfessions.Add(new UserProfession{ ProfessionId = Convert.ToInt32(ProfessionId)});
                        }
                    }

                    var newCandidate = new Candidate
                    {
                        ApplicationNo = await context.NextCandidateApplicationNo(),
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        FirstName = FirstName,
                        SecondName = SecondName,
                        FamilyName = FamilyName,
                        KnownAs = KnownAs,
                        UserName = Username,
                        CustomerId = string.IsNullOrEmpty(AssociateId) ? 0 : Convert.ToInt32(AssociateId),
                        Source = Source,
                        DOB = dob,
                        PpNo = PassportNo,
                        Ecnr = Convert.ToBoolean(ECNR),
                        Address = Address,
                        City = City,
                        Pin = Pin,
                        Country = Country,
                        Nationality = Nationality,
                        Email = Email,
                        Created = DateTime.UtcNow,
                        UserPhones = userphones,
                        UserProfessions = userProfessions
                    };

                    context.Entry(newCandidate).State = EntityState.Added;
                   
                }
            }

            var recAffected=await context.SaveChangesAsync();
            return recAffected;
        }

        static async Task<int> ReadCustomerDataExcelFile(this DataContext context)
        {
            string filePath = "D:\\IdealR_/Ideal/api/CandidateExcelData.xlsx";

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new();
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                for (int i = 1; i <= columns; i++)
                {
                    dataTable.Columns.Add("Column" + i);
                }

                for (int row = 2; row <= rows; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    var Gendr = worksheet.Cells[row, 1].Value.ToString();
                    var FirstName = worksheet.Cells[row, 2].Value.ToString();
                    var SecondName = worksheet.Cells[row, 3].Value.ToString();
                    var FamilyName = worksheet.Cells[row, 4].Value.ToString();
                    var KnownAs = worksheet.Cells[row, 5].Value.ToString();
                    var Username = worksheet.Cells[row, 6].Value.ToString();
                    var Source = worksheet.Cells[row, 7].Value.ToString();
                    var PassportNo = worksheet.Cells[row, 8].Value.ToString();
                    var ECNR = worksheet.Cells[row, 9].Value.ToString();
                    var Address = worksheet.Cells[row,10].Value.ToString();
                    var City = worksheet.Cells[row, 11].Value.ToString();
                    var Pin = worksheet.Cells[row,12].Value.ToString();
                    var Country = worksheet.Cells[row, 13].Value.ToString();
                    var Nationality = worksheet.Cells[row,14].Value.ToString();
                    var PhoneNo = worksheet.Cells[row, 15].Value.ToString();
                    var MobileNo = worksheet.Cells[row, 16].Value.ToString();
                    var Email = worksheet.Cells[row, 17].Value.ToString();
                    var AssociateId = worksheet.Cells[row, 18].Value.ToString();
                    var AssociateName = worksheet.Cells[row, 19].Value.ToString();
                    var ProfessionId = worksheet.Cells[row, 20].Value.ToString();
                    var ProfessionName = worksheet.Cells[row, 21].Value.ToString();
                    var DOB = worksheet.Cells[row, 22].Value.ToString();

                    if (!DateOnly.TryParse(DOB, out DateOnly dob))
                    {
                        dob = new DateOnly(0001, 1, 1);
                    }

                    var userPhones = new List<UserPhone>();
                    var userphones = new List<UserPhone>{new() { MobileNo=MobileNo, IsMain=true}};
                    var userProfessions = new List<UserProfession>();
                    if(!string.IsNullOrEmpty(ProfessionId)) 
                    {
                        var profid = Convert.ToInt32(ProfessionId);
                        if(await context.Professions.FindAsync(profid) != null) 
                        {
                            userProfessions.Add(new UserProfession{ ProfessionId = Convert.ToInt32(ProfessionId)});
                        }
                    }

                    var newCandidate = new Candidate
                    {
                        ApplicationNo = await context.NextCandidateApplicationNo(),
                        Gender = Gendr.ToLower() == "male" ? "m" : "f",
                        FirstName = FirstName,
                        SecondName = SecondName,
                        FamilyName = FamilyName,
                        KnownAs = KnownAs,
                        UserName = Username,
                        CustomerId = string.IsNullOrEmpty(AssociateId) ? 0 : Convert.ToInt32(AssociateId),
                        Source = Source,
                        DOB = dob,
                        PpNo = PassportNo,
                        Ecnr = Convert.ToBoolean(ECNR),
                        Address = Address,
                        City = City,
                        Pin = Pin,
                        Country = Country,
                        Nationality = Nationality,
                        Email = Email,
                        Created = DateTime.UtcNow,
                        UserPhones = userphones,
                        UserProfessions = userProfessions
                    };

                    context.Entry(newCandidate).State = EntityState.Added;
                   
                }
            }

            var recAffected=await context.SaveChangesAsync();
            return recAffected;
        }
    }
}