using System.Data.Common;
using api.Data.Migrations;
using api.DTOs.Admin;
using api.Entities.Master;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Data.Repositories.Master
{
    public class ProfessionRepository : IProfessionRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ProfessionRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task<Profession> AddProfession(Profession profession)
        {
            var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == profession.ProfessionName.ToLower() 
                    && x.ProfessionGroup.ToLower() == profession.ProfessionGroup.ToLower())
                .FirstOrDefaultAsync();

            if(q != null) return q;

            var obj = new Profession{ProfessionGroup=profession.ProfessionGroup, ProfessionName = profession.ProfessionName};

            _context.Professions.Add(obj);

            return await _context.SaveChangesAsync() > 0 ? obj : null;
        }

        public async Task<string> DeleteProfession(string ProfessionName)
        {
             var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == ProfessionName.ToLower())
                .FirstOrDefaultAsync();

            if(q == null) return "That Profession does not exist";

            _context.Professions.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the Profession";
        }

        public async Task<string> DeleteProfessionById(int professionid)
        {
            var q = await _context.Professions.FindAsync(professionid);
                

            if(q == null) return "That Profession does not exist";

            _context.Professions.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the Profession";
        }


        public async Task<string> EditProfession(Profession profession)
        {
            var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == profession.ProfessionName.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(q == null) return "No such Profession exists in the database";

            _context.Entry(q).CurrentValues.SetValues(profession);

            _context.Entry(q).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to update the Profession";
        }

        public async Task<Profession> GetProfessionById(int professionid)
        {
            return await _context.Professions.FindAsync(professionid);
        }


        public async Task<ICollection<Profession>> GetProfessionList()
        {
            return await _context.Professions.OrderBy(x => x.ProfessionName).ToListAsync();
        }


        public async Task<PagedList<Profession>> GetProfessions(ProfessionParams pParams)
        {
            var obj = _context.Professions.OrderBy(x => x.ProfessionName).AsQueryable();

            if(!string.IsNullOrEmpty(pParams.ProfessionName)) 
                obj = obj.Where(x => x.ProfessionName.ToLower() == pParams.ProfessionName.ToLower());

            if(!string.IsNullOrEmpty(pParams.Search)) obj = obj.Where(x => x.ProfessionName.ToLower().Contains(pParams.Search.ToLower()));
            
            if(pParams.Id != 0) obj = obj.Where(x => x.Id == pParams.Id);

            var paged = await PagedList<Profession>.CreateAsync(obj.AsNoTracking()
                //.ProjectTo<Profession>(_mapper.ConfigurationProvider),
                ,pParams.PageNumber, pParams.PageSize);
            
            return paged;
        }

        public async Task<string> ReadProfessionExcelAndWriteToDB(string FileNameWithPath, string Username)
        {
              //var strError = await _context.ReadProspectiveCandidateDataExcelFile(fileNameWithPath, Username);
            //var ProfessionName = label in row2, col 5, data in row2, col6     //this is redendent
            //column titles in row 4, data starts from row 5
            var dtoErr = "";

            int rowTitle=4;     //data starts from this row
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(FileNameWithPath)))
            {

                //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows=0, columns=0;
                int FieldTitleRow=4, intProfName=0, intProfGroup=0;
                ExcelWorksheet worksheet;
                try{
                    worksheet = package.Workbook.Worksheets["Sheet1"];
                    rows = worksheet.Dimension.Rows;
                    columns = worksheet.Dimension.Columns;
                } catch (Exception ex) {
                    dtoErr = ex.Message;
                    return dtoErr;
                }
                
                string ProfessionName="", ProfessionGroup = ""; 
                for(int col=1; col <= 3; col++){
                    try {
                        var colTitle = worksheet.Cells[FieldTitleRow, col].Value?.ToString();   //field name
                        switch (colTitle.ToLower()) {
                            case "professionname": case "profession name": 
                                intProfName=col;
                                break;
                            case "professiongroup": case "profession group":
                                intProfGroup = col;
                                break;
                            default:break;
                        }
                    } catch (Exception ex) {
                        dtoErr = ex.Message;
                        return dtoErr;
                    }
                }
                
                //DataTable dataTable = new();
    
                for(int col=1; col <= columns; col++) {
                    var colTitle = worksheet.Cells[rowTitle, col].Value?.ToString();
                    switch (colTitle?.ToLower()) {
                        case "professionname" :case "profession name": intProfName=col; break;
                        case "professiongroup": case "profession group": intProfGroup=col;break;
                        default:break;
                    }
                }
                    
                for (int row = rowTitle+1; row <= rows; row++)
                {
                    ProfessionName = intProfName == 0 ? "" : worksheet.Cells[row, intProfName].Value?.ToString() ?? "";
                    ProfessionGroup = intProfGroup == 0 ? "" : worksheet.Cells[row, intProfGroup].Value?.ToString() ?? "";

                    var prof = new Profession {ProfessionName = ProfessionName, ProfessionGroup = ProfessionGroup};

                    _context.Entry(prof).State = EntityState.Added;
                }
            }

            bool isSaved = false;
            int recAffected = 0;
            do
                {
                    try
                    {
                        recAffected += await _context.SaveChangesAsync();
                        isSaved = true;
                        dtoErr= recAffected + " records copied";
                    }
                    catch (DbUpdateException ex)
                    {
                        foreach (var entry in ex.Entries) {
                            Console.Write("Prospective candidates Exception - " + ex.InnerException.Message);

                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoErr += ex.Message;
                        }
                    }
                    catch (DbException ex)
                    {
                        dtoErr += ex.Message;
                    }

                    catch (Exception ex)
                    {
                        dtoErr += ex.Message;
                    }
                }
            while (!isSaved);

            return dtoErr;
        }
        
        public async Task<ReturnStringsDto> WriteProfessisonExcelToDB(string fileNameWithPath, string Username)
        {
            var strError = await _context.ReadProfessionDataExcelFile(fileNameWithPath, Username);
            return strError;
        }
        
    }
}