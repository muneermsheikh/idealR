using System.Data.Common;
using api.Data;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Entities.HR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Extensions
{
    public static class ConvertEmployeesAndOrders
    {   
        public static async Task<string> ReadEmployeeExcelFile(this DataContext context, string filePath, string Username)
        {
            //column titles in row 4, data starts from row 5
            var strError="";
            int rowTitle=2;     //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                int intGender=0, intFirstName=0, intSecondName=0, intFamilyName=0, intKnownAs=0, intUsername=0;
                int intPosition=0, intDOB=0, intPlaceOfBirth=0, intAadharNo=0;
                int intNationality=0, intOfficialEmail=0, intOfficialPhoneNo=0, intOfficialMobileNo=0;
                int intDOJ=0, intDepartment=0;
                int intQualification=0, intAddress=0, intAddress2=0, intCity=0, intCountry=0;
                int intHRSkill1=0, intHRSkill2=0, intHRSkill3=0, intRole1=0, intRole2=0, intRole3=0;

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
                        case "phoneno": case "phone no": intOfficialPhoneNo=col;break;
                        case "mobileno": case "mobile no":  intOfficialMobileNo=col;break;
                        case "dateofjoining": case "date of joining": case "doj": case "joined on":  intDOJ=col;break;

                        case "department": case "dept": case "divn": case "division": intDepartment=col;break;
                        case "address": intAddress=col;break;
                        case "address2": case "address 2": intAddress2=col;break;
                        case "city": intCity=col;break;
                        case "country": intCountry=col; break;
                        case "hrskill1": case "hr skill 1": intHRSkill1=col;break;
                        case "hrskill2": case "hr skill 2": intHRSkill2=col;break;
                        case "hrskill3": case "hr skill 3": intHRSkill3=col;break;
                        case "role 1": case "role1": intRole1=col;break;
                        case "role 2": case "role2": intRole2=col;break;
                        case "role 3": case "role3": intRole3=col;break;
                        default:break;
                    }
                }
                
                string Gender = "", FirstName = "", SecondName = "", FamilyName = "", KnownAs = "";
                string Position = "", Qualification = "", DateOfBirth = "", PlaceOfBirth = "";
                string AadharNo = "", OfficialEmail="";
                string PhoneNo="", Phone2="", DateOfJoining="";
                string Department="", Address="", Address2="", City="", Country=""; 
                string HRSkill1="", HRSkill2="", HRSkill3="";
                string Role1="", Role2="", Role3="";
                //string OtherSkill1="", OtherSkill2="", OtherSkill3="";

                int TotalCount=0;

                for (int row = rowTitle+1; row <= rows; row++)
                {
                    //Required
                    FirstName = intFirstName == 0 ? "" : worksheet.Cells[row, intFirstName].Value?.ToString() ?? "";
                    if(string.IsNullOrEmpty(FirstName)) continue;
                    
                    SecondName = intSecondName == 0 ? "" : worksheet.Cells[row, intSecondName].Value?.ToString() ?? "";
                    FamilyName = intFamilyName == 0 ? "" : worksheet.Cells[row, intFamilyName].Value?.ToString() ?? "";
                    //Required
                    KnownAs = intKnownAs == 0 ? "" : worksheet.Cells[row, intKnownAs].Value?.ToString() ?? "";
                    if(string.IsNullOrEmpty(KnownAs)) continue;
                    Username = intUsername == 0 ? "" : worksheet.Cells[row, intUsername].Value?.ToString() ?? "";
                    //Required
                    Position = intPosition == 0 ? "": worksheet.Cells[row, intPosition].Value?.ToString()  ?? "";
                    if(string.IsNullOrEmpty(Position)) continue;
                    Qualification = intQualification == 0 ? "" : worksheet.Cells[row, intQualification].Value?.ToString() ?? "";
                    DateOfBirth = intDOB == 0 ? "" : worksheet.Cells[row, intDOB].Value?.ToString() ?? "";
                    PlaceOfBirth = intPlaceOfBirth == 0 ? "" : worksheet.Cells[row, intPlaceOfBirth].Value?.ToString() ?? "";
                    AadharNo = intAadharNo == 0 ? "" : worksheet.Cells[row, intAadharNo].Value?.ToString() ?? "";
                    Gender = Gender == "m" ? "Male" : "Female";
                    Address = intAddress == 0 ? "" : worksheet.Cells[row, intAddress].Value?.ToString() ?? "";
                    Address2 = intAddress2 == 0 ? "" : worksheet.Cells[row, intAddress2].Value?.ToString() ?? "";
                    City = intCity == 0 ? "" : worksheet.Cells[row, intCity].Value?.ToString() ?? "";
                    Country = intCountry == 0 ? "India" : worksheet.Cells[row, intCountry].Value?.ToString() ?? "India";
                    OfficialEmail = intOfficialEmail == 0 ? "" : worksheet.Cells[row, intOfficialEmail].Value?.ToString() ?? "";
                    PhoneNo = intOfficialPhoneNo == 0 ? "" : worksheet.Cells[row, intOfficialPhoneNo].Value?.ToString() ?? "";
                    Phone2 = intOfficialMobileNo == 0 ? "" : worksheet.Cells[row, intOfficialMobileNo].Value?.ToString() ?? "";
                    if(string.IsNullOrEmpty(PhoneNo) && string.IsNullOrEmpty(Phone2)) continue;

                    DateOfJoining = intDOJ == 0 ? "" : worksheet.Cells[row, intDOJ].Value?.ToString() ?? "";
                    Department = intDepartment == 0 ? "" : worksheet.Cells[row, intDepartment].Value?.ToString() ?? "";
                    HRSkill1 = intHRSkill1 == 0 ? "" : worksheet.Cells[row, intHRSkill1].Value?.ToString() ?? "";
                    HRSkill2 = intHRSkill2 == 0 ? "" : worksheet.Cells[row, intHRSkill2].Value?.ToString() ?? "";
                    HRSkill3 = intHRSkill3 == 0 ? "" : worksheet.Cells[row, intHRSkill3].Value?.ToString() ?? "";
                    Role1 = intRole1 == 0 ? "" : worksheet.Cells[row, intRole1].Value?.ToString() ?? "";
                    Role2 = intRole2 == 0 ? "" : worksheet.Cells[row, intRole2].Value?.ToString() ?? "";
                    Role3 = intRole3 == 0 ? "" : worksheet.Cells[row, intRole3].Value?.ToString() ?? "";
                    
                    var newEmployee = new Employee
                    {
                        Gender = Gender, FirstName = FirstName, SecondName = SecondName,
                        FamilyName = FamilyName, KnownAs = KnownAs, UserName = Username,
                        Position = Position, Qualification = Qualification, PlaceOfBirth=PlaceOfBirth,
                        AadharNo = AadharNo,  Email=OfficialEmail,
                        PhoneNo = PhoneNo, Phone2 = Phone2,
                        Department = Department, Address = Address, Address2 = Address2, 
                        City = City
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
                    TotalCount ++;
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

    
        public static async Task<string> ReadOrdersExcelFile(this DataContext context, string filePath, string Username)
        {
            //column titles in row 4, data starts from row 5
            var strError="";
            int TotalCount=0;
            int rowTitle=2;     //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var errString="";
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                //column numbers
                int intOrderNo=0, intCustomerId=0, intProjectManagerId=0, intSalesmanId=0, intStatus=0;
                int intOrderDate=0, intOrderRef=0, intOrderRefDate=0, intCompleteBy=0, intCountry=0, intCityOfWorking=0;
                //orderitems
                int intSrNo1=0, intProfessionId1=0, intQuantity1=0, intSourceFrom1=0, intEcnr1=0,intCompleteBefore1=0;
                int intSrNo2=0, intProfessionId2=0, intQuantity2=0, intSourceFrom2=0, intEcnr2=0,intCompleteBefore2=0;
                int intSrNo3=0, intProfessionId3=0, intQuantity3=0, intSourceFrom3=0, intEcnr3=0,intCompleteBefore3=0;
                int intSrNo4=0, intProfessionId4=0, intQuantity4=0, intSourceFrom4=0, intEcnr4=0,intCompleteBefore4=0;
                
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "orderno": case "order no": intOrderNo=col; break;
                        case "orderdate": case "order date": case "order dated": intOrderDate=col;break;
                        case "customerid": case "customer id": intCustomerId=col;break;
                        case "projmanagerid": case "projectmanagerid": case "proj manager id": case "projmanager id": intProjectManagerId=col;break;
                        case "salesmanid": case "salesman id": intSalesmanId=col;break;
                        case "country": intCountry=col;break;
                        case "status": intStatus=col;break;
                        case "orderref": case "order ref": intOrderRef=col;break;
                        case "orderrefdate": case "order ref date": case "order ref dt": intOrderRefDate=col;break;
                        case "completeby": case "complete by": intCompleteBy=col;break;
                        case "cityofworking": case "city of working": case "working city": case "workingcity": intCityOfWorking=col;break;
                        //orderitems
                        case "srno1": case "sr no 1": case "srno 1": intSrNo1=col;break;
                        case "professionid1": case "profession id1": case "profession id 1": intProfessionId1=col;break;
                        case "quantity1": case "quantity 1": case "qnty1": intQuantity1=col;break;
                        case "sourcefrom1": case "source from1": case "source from 1": intSourceFrom1=col;break;
                        case "ecnr1": case "ecnr 1": intEcnr1=col;break;
                        case "completebefore1": case "complete before 1": case "completebefore 1": intCompleteBefore1=col;break;
                        
                        case "srno4": case "sr no 4": case "srno 4": intSrNo4=col;break;
                        case "professionid4": case "profession id4": case "profession id 4": intProfessionId4=col;break;
                        case "quantity4": case "quantity 4": case "qnty4": intQuantity4=col;break;
                        case "sourcefrom4": case "source from4": case "source from 4": intSourceFrom4=col;break;
                        case "ecnr4": case "ecnr 4": intEcnr4=col;break;
                        case "completebefore4": case "complete before 4": case "completebefore 4": intCompleteBefore4=col;break;
                        
                        case "srno2": case "sr no 2": case "srno 2": intSrNo2=col;break;
                        case "professionid2": case "profession id2": case "profession id 2": intProfessionId2=col;break;
                        case "quantity2": case "quantity 2": case "qnty2": intQuantity2=col;break;
                        case "sourcefrom2": case "source from2": case "source from 2": intSourceFrom2=col;break;
                        case "ecnr2": case "ecnr 2": intEcnr2=col;break;
                        case "completebefore2": case "complete before 2": case "completebefore 2": intCompleteBefore2=col;break;
                        
                        case "srno3": case "sr no 3": case "srno 3": intSrNo3=col;break;
                        case "professionid3": case "profession id3": case "profession id 3": intProfessionId3=col;break;
                        case "quantity3": case "quantity 3": case "qnty3": intQuantity3=col;break;
                        case "sourcefrom3": case "source from3": case "source from 3": intSourceFrom3=col;break;
                        case "ecnr3": case "ecnr 3": intEcnr3=col;break;
                        case "completebefore3": case "complete before 3": case "completebefore 3": intCompleteBefore3=col;break;
                        
                        default:break;
                    }
                }
                //initialize table variables
                int OrderNo=0, CustomerId=0, ProjectManagerId=0, SalesmanId=0;
                string Status="", CityOfWorking="", Country="", OrderRef="";
                DateTime OrderRefDate, OrderDate, CompleteBy;

                //orderitems
                int SrNo1=0, ProfessionId1=0, Quantity1=0;
                int SrNo2=0, ProfessionId2=0, Quantity2=0;
                int SrNo3=0, ProfessionId3=0, Quantity3=0;
                int SrNo4=0, ProfessionId4=0, Quantity4=0;
                bool Ecnr1=false, Ecnr2=false, Ecnr3=false, Ecnr4=false;
                string SourceFrom1="", SourceFrom2="", SourceFrom3="", SourceFrom4="";
                string CompleteBefore1="", CompleteBefore2="", CompleteBefore3="", CompleteBefore4="";
                
            
                int NextOrderNo = await context.Orders.MaxAsync(x => x.OrderNo) + 1;
                var OrderItems = new List<OrderItem>();

                for (int row = rowTitle+1; row <= rows; row++)
                {
                    Quantity1 = intQuantity1 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intQuantity1].Value?.ToString() ?? "0");
                    if(Quantity1 == 0) {
                        errString = "Quantity cannot be 0 or null, in row " + row;
                        break;
                    }

                    ProfessionId1 = intProfessionId1 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intProfessionId1].Value?.ToString() ?? "0");
                    if(ProfessionId1 == 0) {
                        errString="Profession Id not defined in row " + row;
                        break;
                    }

                    if(await ValidateProfessionId(context, ProfessionId1)==false) continue;

                    var DtOrder= intOrderDate == 0 ? "" : worksheet.Cells[row, intOrderDate].Value?.ToString() ?? "";

                    if(DateTime.TryParse(DtOrder, out DateTime OrderDt)) {
                        OrderDate = OrderDt;
                    } else {
                        errString="Order Date invalid or not defined on row " + row;
                        break;
                    }

                    CustomerId = intCustomerId == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intCustomerId].Value?.ToString() ?? "0");
                    if(CustomerId==0) {
                        errString = "Customer Id not defined in row " + row;
                        break;
                    }
                    if(await ValidateCustomerId(context, CustomerId)==false) {
                        errString = "Invalid Customer Id - " + CustomerId + " in row " + row;
                        break;
                    }
                    ProjectManagerId = intProjectManagerId == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intProjectManagerId].Value?.ToString() ?? "0");
                    if(await ValidateEmployeeId(context, ProjectManagerId)==false) ProjectManagerId=0;
                    SalesmanId = intSalesmanId == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intSalesmanId].Value?.ToString() ?? "0");
                    if(await ValidateEmployeeId(context, SalesmanId)==false) SalesmanId=0;

                    var dtCompleteBy = intCompleteBy == 0 ? "": worksheet.Cells[row, intCompleteBy].Value?.ToString()  ?? "";
                    if(DateTime.TryParse(dtCompleteBy, out DateTime dt)) CompleteBy = dt;

                    //Required prperties
                    OrderNo = intOrderNo == 0 ? NextOrderNo : Convert.ToInt32(worksheet.Cells[row, intOrderNo].Value?.ToString());
                    if(OrderNo==0) OrderNo=NextOrderNo;
                    Status = intStatus == 0 ? "Active": worksheet.Cells[row, intStatus].Value?.ToString()  ?? "Active";
                    CityOfWorking = intCityOfWorking == 0 ? "" : worksheet.Cells[row, intCityOfWorking].Value?.ToString() ?? "";
                    OrderRef = intOrderRef == 0 ? "" : worksheet.Cells[row, intOrderRef].Value?.ToString() ?? "";
                    var orderrefdt = intOrderRefDate == 0 ? "" : worksheet.Cells[row, intOrderRefDate].Value?.ToString() ?? "";
                    if(DateTime.TryParse(orderrefdt, out DateTime OrderRefDt)) OrderRefDate = OrderRefDt;

                    Country = intCountry == 0 ? "India" : worksheet.Cells[row, intCountry].Value?.ToString() ?? "India";

                    SrNo1 = intSrNo1 == 0 ? 1 : Convert.ToInt32(worksheet.Cells[row, intSrNo1].Value?.ToString() ?? "1");
                    SourceFrom1 = intSourceFrom1 == 0 ? "India" : worksheet.Cells[row, intSourceFrom1].Value?.ToString() ?? "India";
                    Ecnr1 = intEcnr1 != 0 && Convert.ToBoolean(worksheet.Cells[row, intEcnr1].Value?.ToString() ?? "0");
                    CompleteBefore1 = intCompleteBefore1 == 0 ? "" : worksheet.Cells[row, intCompleteBefore1].Value?.ToString() ?? "";
                    Quantity1 = intQuantity1 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intQuantity1].Value?.ToString() ?? "0");
                    
                    var newOrderItem = new OrderItem{SrNo=SrNo1, SourceFrom=SourceFrom1, Ecnr=Ecnr1, Quantity=Quantity1, 
                        ProfessionId=ProfessionId1, MinCVs=Quantity1*3, MaxCVs=Quantity1*3};
                    if(DateTime.TryParse(CompleteBefore1, out DateTime CompleteBeforeDt1)) 
                        newOrderItem.CompleteBefore=Convert.ToDateTime(CompleteBefore1);
                    OrderItems.Add(newOrderItem);
                    
                    ProfessionId2 = intProfessionId2 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intProfessionId2].Value?.ToString() ?? "0");
                    Quantity2 = intQuantity2 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intQuantity2].Value?.ToString() ?? "0");
                    if(ProfessionId2 > 0 && Quantity2 > 0) {
                        if(await ValidateProfessionId(context, ProfessionId2)==false) continue;
                        SrNo2 = intSrNo2 == 0 ? 2 : Convert.ToInt32(worksheet.Cells[row, intSrNo2].Value?.ToString() ?? "2");
                        SourceFrom2 = intSourceFrom2 == 0 ? "India" : worksheet.Cells[row, intSourceFrom2].Value?.ToString() ?? "India";
                        Ecnr2 = intEcnr2 != 0 && Convert.ToBoolean(worksheet.Cells[row, intEcnr2].Value?.ToString() ?? "0");
                        CompleteBefore2 = intQuantity2 == 0 ? "" : worksheet.Cells[row, intQuantity2].Value?.ToString() ?? "";
                        
                        newOrderItem = new OrderItem{SrNo=SrNo2, SourceFrom=SourceFrom2, Ecnr=Ecnr2, Quantity=Quantity2, 
                            ProfessionId=ProfessionId2, MinCVs=Quantity2*3, MaxCVs=Quantity2*3};
                        if(DateTime.TryParse(CompleteBefore2, out DateTime CompleteBeforeDt2)) 
                            newOrderItem.CompleteBefore=Convert.ToDateTime(CompleteBefore2);
                        OrderItems.Add(newOrderItem);
                    }
                    
                    ProfessionId3 = intProfessionId3 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intProfessionId3].Value?.ToString() ?? "0");
                    Quantity3 = intQuantity3 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intQuantity3].Value?.ToString() ?? "0");
                    if(ProfessionId3 > 0 && Quantity3 > 0) {
                        if(await ValidateProfessionId(context, ProfessionId3)==false) continue;
                        SrNo3 = intSrNo3 == 0 ? 3 : Convert.ToInt32(worksheet.Cells[row, intSrNo3].Value?.ToString() ?? "3");
                        SourceFrom3 = intSourceFrom2 == 0 ? "India" : worksheet.Cells[row, intSourceFrom3].Value?.ToString() ?? "India";
                        Ecnr3 = intEcnr3 != 0 && Convert.ToBoolean(worksheet.Cells[row, intEcnr3].Value?.ToString() ?? "0");
                        CompleteBefore3 = intCompleteBefore3 == 0 ? "" : worksheet.Cells[row, intCompleteBefore3].Value?.ToString() ?? "";
                        
                        newOrderItem = new OrderItem{SrNo=SrNo3, SourceFrom=SourceFrom3, Ecnr=Ecnr3, Quantity=Quantity3, 
                            ProfessionId=ProfessionId3, MinCVs=Quantity3*3, MaxCVs=Quantity3*3};
                        if(DateTime.TryParse(CompleteBefore3, out DateTime CompleteBeforeDt3)) 
                            newOrderItem.CompleteBefore=Convert.ToDateTime(CompleteBefore3);
                        OrderItems.Add(newOrderItem);
                    }

                    ProfessionId4 = intProfessionId4 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intProfessionId4].Value?.ToString() ?? "0");
                    Quantity4 = intQuantity4 == 0 ? 0 : Convert.ToInt32(worksheet.Cells[row, intQuantity4].Value?.ToString() ?? "0");
                    if(ProfessionId4 > 0 && Quantity4 > 0) {
                        if(await ValidateProfessionId(context, ProfessionId4)==false) continue;
                        SrNo4 = intSrNo4 == 0 ? 4 : Convert.ToInt32(worksheet.Cells[row, intSrNo4].Value?.ToString() ?? "4");
                        SourceFrom4 = intSourceFrom4 == 0 ? "India" : worksheet.Cells[row, intSourceFrom4].Value?.ToString() ?? "India";
                        Ecnr4 = intEcnr4 != 0 && Convert.ToBoolean(worksheet.Cells[row, intEcnr4].Value?.ToString() ?? "0");
                        CompleteBefore4 = intCompleteBefore4 == 0 ? "" : worksheet.Cells[row, intCompleteBefore4].Value?.ToString() ?? "";
                        
                        newOrderItem = new OrderItem{SrNo=SrNo4, SourceFrom=SourceFrom4, Ecnr=Ecnr4, Quantity=Quantity4, 
                            ProfessionId=ProfessionId4, MinCVs=Quantity1*4, MaxCVs=Quantity4*3};
                        if(DateTime.TryParse(CompleteBefore4, out DateTime CompleteBeforeDt4)) 
                            newOrderItem.CompleteBefore=Convert.ToDateTime(CompleteBefore4);
                        OrderItems.Add(newOrderItem);
                    }


                    var newOrder = new Order
                    {
                        OrderNo=OrderNo, OrderDate=OrderDate, CityOfWorking = CityOfWorking, CompleteBy = dt, Country=Country,
                        CustomerId = CustomerId, OrderRef = OrderRef, OrderRefDate=OrderRefDt, ProjectManagerId=ProjectManagerId,
                        SalesmanId = SalesmanId, Status = "Awaiting Review", OrderItems = OrderItems
                    };
                    
                    TotalCount ++;
                    NextOrderNo ++;
        
                    context.Entry(newOrder).State = EntityState.Added;
                }
            }

            int recAffected = 0;
            try {
                recAffected=await context.SaveChangesAsync();
                return TotalCount.ToString();
            } catch (DbException ex) {
                strError = ex.Message;
            } catch (Exception ex) {
                strError = ex.Message;
            }

            return string.IsNullOrEmpty(strError) ? "" : strError;
        }

        private static async Task<bool> ValidateProfessionId(DataContext contxt, int profId) {
            var exists = await contxt.Professions.FindAsync(profId);
            return exists != null;
        }

        private static async Task<bool> ValidateCustomerId(DataContext contxt, int custId) {
            var exists = await contxt.Customers.FindAsync(custId);
            return exists != null;
        }

        private static async Task<bool> ValidateEmployeeId(DataContext contxt, int empId) {
            var exists = await contxt.Employees.FindAsync(empId);
            return exists != null;
        }
    }
}