using System.Data.Common;
using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Masters;
using api.Interfaces.Orders;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class CandidatesRepository : ICandidateRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IComposeMessagesHRRepository _hrMsgRepo;
        private readonly DateTime _today = DateTime.UtcNow;
        private readonly IProfessionRepository _profRepo;
        public CandidatesRepository(DataContext context, UserManager<AppUser> userManager, IProfessionRepository profRepo,
            IMapper mapper, IComposeMessagesHRRepository hrMsgRepo)
        {
            _profRepo = profRepo;
            _hrMsgRepo = hrMsgRepo;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> AadharNoExists(string aadharNo)
        {
            var obj = await _context.Candidates
                .Where(x => x.AadharNo == aadharNo)
                .Select(x => x.AadharNo)
                .FirstOrDefaultAsync();
            
            return obj != null;
        }

        public async Task<bool> CheckPPExists(string PPNo)
        {
             var obj = await _context.Candidates
                .Where(x => x.PpNo == PPNo)
                .Select(x => x.PpNo)
                .FirstOrDefaultAsync();
            
            return obj != null;
        }

        public async Task<bool> DeleteCandidate(int Id)
        {
            var candidate = await _context.Candidates.FindAsync(Id);
            if (candidate == null) return false;

            _context.Candidates.Remove(candidate);
            _context.Entry(candidate).State = EntityState.Deleted;

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        
        }

        public async Task<Candidate> GetCandidate(CandidateParams candidateParams)
        {
            var query = _context.Candidates.Include(x => x.UserProfessions).AsQueryable();
            if(candidateParams.Id > 0) {
                query = query.Where(x => x.Id == candidateParams.Id);}
                else {
                    if(!string.IsNullOrEmpty(candidateParams.CandidateName)) 
                        query = query.Where(x => x.FullName.ToLower().Contains(candidateParams.CandidateName.ToLower()));
                    if(!string.IsNullOrEmpty(candidateParams.PassportNo))
                        query = query.Where(x => x.PpNo == candidateParams.PassportNo);
                    /*if(candidateParams.ProfessionId > 0)
                        query = query.Where(x => x.UserProfessions.Select(x => x.ProfessionId).ToList()
                        ).Include(candidateParams.ProfessionId);
                    */
                }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<CandidateBriefDto> GetCandidateBriefFromParams(CandidateParams cParams)
        {
            
            var query = _context.Candidates.AsQueryable();
   
            if(cParams.Id != 0) {
                query = query.Where(x => x.Id == cParams.Id);
            } else if(cParams.ApplicationNoFrom !=0 && cParams.ApplicationNoUpto != 0) {
                query = query.Where(x => x.ApplicationNo >= cParams.ApplicationNoFrom &&
                    x.ApplicationNo <= cParams.ApplicationNoUpto);
            } else {
                return null;
            }

            var obj = await query.ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

            return obj;
        }

        public async Task<PagedList<cvsAvailableDto>> GetAvailableCandidates(CandidateParams candidateParams)
        {
                var query2 = (from asses in _context.CandidateAssessments where asses.AssessResult != "Not Assessed" && asses.CVRefId==0
                    join cv in _context.Candidates on asses.CandidateId equals cv.Id
                    join item in _context.OrderItems on asses.OrderItemId equals item.Id
                    join cat in _context.Professions on item.ProfessionId equals cat.Id
                    join order in _context.Orders on item.OrderId equals order.Id
                    orderby cv.ApplicationNo
                    select new cvsAvailableDto {
                        CandAssessmentId = asses.Id, ApplicationNo = cv.ApplicationNo, City = cv.City, FullName = cv.FullName, 
                        CandidateId = cv.Id, AssessedOn = asses.AssessedOn, 
                        Gender=  (cv.Gender == "female" ? "F": "M").ToUpper(),
                        GradeAssessed=asses.AssessResult,  Checked = false, OrderItemId = item.Id,
                        OrderCategoryRef=cat.ProfessionName + " (" + order.OrderNo + "-" + item.SrNo + ") - " + order.Customer.KnownAs,
                    }).AsQueryable();
                
                var obj = await PagedList<cvsAvailableDto>.CreateAsync(query2.AsNoTracking()
                    //.ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider)
                    , candidateParams.PageNumber, candidateParams.PageSize);
                
                foreach(var item in obj) {
                    item.userProfessions = await _context.UserProfessions.Where(x => x.CandidateId == item.CandidateId).ToListAsync();
                    foreach(var prof in item.userProfessions) {
                        if(string.IsNullOrEmpty(prof.ProfessionName)) {
                            prof.ProfessionName = await _context.GetProfessionNameFromId(prof.ProfessionId);
                        }
                        _context.Entry(prof).State = EntityState.Modified;
                    }
                }
                
                await _context.SaveChangesAsync();
                return obj;
        }

        public async Task<PagedList<CandidateBriefDto>> GetCandidates(CandidateParams cParams)
        {
                //check if userProfession.ProfessionName is not blanks
                var blankProfs = await _context.UserProfessions
                    .Where(x => x.ProfessionName==null || x.ProfessionName == "").ToListAsync();
                if(blankProfs.Count > 0) {
                    foreach(var up in blankProfs) {
                        var profName = await _context.Professions
                            .Where(x => x.Id == up.ProfessionId).Select(x => x.ProfessionName).FirstOrDefaultAsync();
                        up.ProfessionName = profName;
                        _context.Entry(up).State=EntityState.Modified;
                    }
                    if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
                }                    
                
                var qry = (from cand in _context.Candidates
                    join cust in _context.Customers on cand.CustomerId equals cust.Id into Customer
                    from cst in Customer.DefaultIfEmpty()
                    select new CandidateBriefDto  {
                        ApplicationNo = cand.ApplicationNo, City = cand.City, Id=cand.Id,
                        CustomerId = Convert.ToInt32(cand.CustomerId), Email = cand.Email,
                        FullName = cand.FullName, KnownAs = cand.KnownAs, PpNo = cand.PpNo,
                        Status = cand.Status, UserProfessions = cand.UserProfessions.Select(x => x.ProfessionName.ToLower()).ToList(),
                        ReferredByName = cst.CustomerName
                    })
                    .OrderByDescending(x => x.ApplicationNo)
                    .AsQueryable();
                
                if(cParams.ApplicationNoFrom > 0) {
                    if(cParams.ApplicationNoUpto > 0) {
                        qry = qry.Where(x => x.ApplicationNo >= cParams.ApplicationNoFrom 
                        && x.ApplicationNo <= cParams.ApplicationNoUpto);
                    } else {
                        qry = qry.Where(x => x.ApplicationNo == cParams.ApplicationNoFrom);
                    }
                }
                if(!string.IsNullOrEmpty(cParams.CandidateName)) qry = qry.Where(x => x.FullName.ToLower().Contains(cParams.CandidateName.ToLower()));
                if(!string.IsNullOrEmpty(cParams.CategoryName)) qry = qry.Where(x => 
                    x.UserProfessions.Contains(cParams.CategoryName.ToLower()));

                var pagedList = await PagedList<CandidateBriefDto>.CreateAsync(qry.AsNoTracking()
                        , cParams.PageNumber, cParams.PageSize);
                
                //update pagedList.userProfessions
                var ups = await _context.UserProfessions.Where(x =>pagedList.Select(x => x.Id).ToList().Contains(x.CandidateId)).ToListAsync();
                
                foreach(var page in pagedList) {
                    var userProfs = ups.Where(x => x.CandidateId==page.Id).Select(x => x.ProfessionName).ToList();
                    page.UserProfessions = userProfs;
                }
                return pagedList;

        }

        public async Task<bool> InsertCandidate(Candidate candidate)
        {
            candidate.AppUserId = await UpdateAppUserIdExtensions.UpdateCandidateAppUserId(candidate, 
                    _userManager, _context);
          
            _context.Candidates.Add(candidate);
            
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<bool> UpdateCandidate(Candidate newObject)
        {
            var existingObject = _context.Candidates
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.UserProfessions)
                .Include(x => x.UserExperiences)
                .Include(x => x.UserPhones)
                .Include(x => x.UserAttachments)
                //.Include(x => x.UserQualifications)
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.UserProfessions?.ToList())
            {
                if(!newObject.UserProfessions.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserProfessions.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserProfessions)
            {
                var existingItem = existingObject.UserProfessions?
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserProfession
                    {
                        CandidateId = existingObject.Id,
                        ProfessionId = newItem.ProfessionId,
                        IsMain = newItem.IsMain,
                        IndustryId = newItem.IndustryId
                    };

                    existingObject.UserProfessions.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            //delete userphones that currently exists, but are not present in new Object
            foreach (var existingItem in existingObject.UserPhones?.ToList())
            {
                if(!newObject.UserPhones.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserPhones.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserPhones)
            {
                var existingItem = existingObject.UserPhones?
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserPhone
                    {
                        CandidateId = existingObject.Id,
                        MobileNo = newItem.MobileNo,
                        IsMain = newItem.IsMain,
                        IsValid = true,
                        Remarks = newItem.Remarks
                    };

                    existingObject.UserPhones.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            //delete experience that currently exist in DB but not in newObject
            foreach (var existingItem in existingObject.UserExperiences.ToList())
            {
                if(!newObject.UserExperiences.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserExps.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserExperiences)
            {
                var existingItem = existingObject.UserExperiences
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserExp
                    {
                        CandidateId = existingObject.Id,
                        SrNo = newItem.SrNo,
                        Employer = newItem.Employer,
                        Position = newItem.Position,
                        CurrentJob = newItem.CurrentJob,
                        WorkedFrom = newItem.WorkedFrom,
                        WorkedUpto = newItem.WorkedUpto,
                        SalaryCurrency = newItem.SalaryCurrency,
                        MonthlySalaryDrawn = newItem.MonthlySalaryDrawn
                    };

                    existingObject.UserExperiences.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            /*
            
            //edit UserQualifications
            foreach (var existingItem in existingObject.UserQualifications.ToList())
            {
                if(!newObject.UserQualifications.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserQualifications.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }
        
            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserQualifications)
            {
                var existingItem = existingObject.UserQualifications
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserQualification
                    {
                        CandidateId = existingObject.Id,
                        QualificationId = newItem.QualificationId,
                        IsMain = newItem.IsMain
                    };

                    existingObject.UserQualifications.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }
            */
            
            //userAttachments
            foreach (var existingItem in existingObject.UserAttachments.ToList())
            {
                if(!newObject.UserAttachments.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    //delete  any files previously uploaded
                    var location = existingItem.UploadedLocation;
                    if(string.IsNullOrEmpty(location)) location = "D:\\IdealR_\\idealR\\api\\Assets\\Images";
                    var FileToDelete = location + "\\" + existingItem.Name;
                    if(File.Exists(FileToDelete)) File.Delete(FileToDelete);

                    _context.UserAttachments.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            foreach(var newItem in newObject.UserAttachments)
            {
                var existingItem = existingObject.UserAttachments?
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserAttachment
                    {
                        CandidateId = existingObject.Id,
                        Name = newObject.ApplicationNo + "-" + newItem.Name,
                        UploadedbyUserName = newItem.UploadedbyUserName,
                        UploadedLocation = newItem.UploadedLocation,
                        UploadedOn = newItem.UploadedOn,
                        AttachmentType = newItem.AttachmentType,
                        AppUserId = newItem.AppUserId
                    };

                    existingObject.UserAttachments.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existingObject).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    
        public async Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, string Username)
          {
                var cand = await CreateCandidateObject(registerDto, Username);

                _context.Candidates.Add(cand);

                if (registerDto.NotificationDesired) {
                        await _hrMsgRepo.ComposeHTMLToAckToCandidateByEmail(cand);
                }
                
                try {
                    await _context.SaveChangesAsync();
                } catch {
                    return null;
                }
                
            //create COA for the candidate    
                var coa = new COA{
                     AccountClass = "R", AccountType = "B", 
                     AccountName = cand.ApplicationNo + "-" + cand.FullName,
                     Divn = "B"
                };

                _context.COAs.Add(coa);
                
               await _context.SaveChangesAsync();

               return cand;
          }

        public async Task<Candidate> CreateCandidateObject(RegisterDto registerDto, string Username)
        {
            var NextAppNo = await _context.Candidates.MaxAsync(x => x.ApplicationNo);
            NextAppNo = NextAppNo == 0 ? 10001 : NextAppNo+1;

        var cand = new Candidate {
            Gender = registerDto.Gender, AppUserId = registerDto.AppUserId,
            ApplicationNo= NextAppNo, FirstName = registerDto.FirstName,SecondName = registerDto.SecondName ?? "", 
            FamilyName = registerDto.FamilyName ?? "",  KnownAs = registerDto.KnownAs ?? "",
            DOB =registerDto.DOB, AadharNo = registerDto.AadharNo ?? "", Email =registerDto.Email,
            NotificationDesired = registerDto.NotificationDesired, Nationality = registerDto.Nationality, 
            CustomerId = registerDto.CompanyId, PpNo = registerDto.PpNo ?? "", City = registerDto.City, 
            Pin = registerDto.Pin, UserPhones = registerDto.UserPhones, UserProfessions = registerDto.UserProfessions,
        };

        if (registerDto.UserAttachments != null && registerDto.UserAttachments.Count > 0) {
                foreach(var att in registerDto.UserAttachments) {
                        att.Name = NextAppNo + "-" + att.Name;
                        att.UploadedLocation = Directory.GetCurrentDirectory();
                        att.UploadedbyUserName = Username;
                }
                cand.UserAttachments = registerDto.UserAttachments;
            }
            return cand;
        }

        public async Task<ICollection<UserAttachment>> AddAndSaveUserAttachments(ICollection<UserAttachment> newObject, string Username)
        {
            var attachmentsPosted = new List<UserAttachment>();
            var candId = newObject.Select(x => x.CandidateId).FirstOrDefault();

            var existing = await  (_context.UserAttachments
                    .Where(x => x.CandidateId == candId)
                    .AsNoTracking()
                ).ToListAsync();
                 
            var filesDeleted = new List<string>();
            foreach(var existingItem in existing)
            {
                if(!newObject.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserAttachments.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                    filesDeleted.Add(existingItem.UploadedLocation + "//" + existingItem.Name);
                }
            }
                
            foreach(var newItem in newObject)
            {
                var existingItem = existing?.Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null) {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {
                    var appuserObj= await _userManager.FindByNameAsync(Username);
                    var appuserid = appuserObj == null ? 0 : appuserObj.Id;

                    var itemToInsert = new UserAttachment{
                        CandidateId = newItem.CandidateId, AppUserId = appuserid, 
                        AttachmentType = newItem.AttachmentType, Name = newItem.Name, 
                        UploadedbyUserName = Username, UploadedLocation = newItem.UploadedLocation,
                        UploadedOn = _today,  Length = newItem.Length
                    };
                    
                    _context.Entry(itemToInsert).State = EntityState.Added;
                    attachmentsPosted.Add(newItem);
                }
               
            }

            if (await _context.SaveChangesAsync() > 0 ) {
                foreach(var filename in filesDeleted) {
                    if(File.Exists(filename)) File.Delete(filename);
                }
                return attachmentsPosted;
            } else {
                return null;
            }
            
        }

        public async Task<bool> DeleteUserAttachment(int attachmentId)
        {
            var exists = await _context.UserAttachments.FindAsync(attachmentId);
            if(exists == null) return false;

            _context.UserAttachments.Remove(exists);
            _context.Entry(exists).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserAttachmentsWithErrDto> UpdateCandidateAttachments(ICollection<UserAttachment> userAttachments)
        {
            var attachmentsToReturn = new List<UserAttachment>();
            var dtoReturn = new UserAttachmentsWithErrDto();

            foreach(var attach in userAttachments)
            {
                var existing = await _context.UserAttachments
                    .Where(x => x.Name == attach.Name && x.CandidateId == attach.CandidateId)
                    .FirstOrDefaultAsync();
                if(existing != null)  {
                    _context.Entry(existing).CurrentValues.SetValues(attach);
                    _context.Entry(existing).State = EntityState.Modified;
                    //attachmentsToReturn.Add(existing);
                } else {
                    _context.UserAttachments.Add(attach);
                }
                dtoReturn.UserAttachments.Add(existing);
            }

            try{
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                dtoReturn.ErrorString = ex.Message;
            } catch (Exception ex) {
                dtoReturn.ErrorString = ex.Message;
            } 
            
            return dtoReturn;
        }

        public async Task<UserAttachment> GetUserAttachmentById(int attachmentId)
        {
            var obj = await _context.UserAttachments.FindAsync(attachmentId);
            return obj;
        }

        public async Task<int> GetApplicationNoFromCandidateId(int candidateId)
        {
            var appno = await _context.GetApplicationNoFromCandidateId(candidateId);

            return appno;
        } 

        public async Task<int> GetAppUserIdOfCandidate(int candidateId)
        {
            var id = await _context.GetAppUserIdOfCandidate(candidateId);

            return id;
        }

        public async Task<ICollection<UserAttachment>> GetUserAttachmentByCandidateId (int candidateid)
        {
            var objs = await _context.UserAttachments.Where(x => x.CandidateId == candidateid).ToListAsync();

            return objs;
        }

        public async Task<Candidate> GetCandidateById(int candidateid)
        {
            var obj = await _context.Candidates
                .Include(x => x.UserPhones)
                .Include(x => x.UserAttachments)
                .Include(x => x.UserProfessions)
                .Include(x => x.UserExperiences)
                .Include(x => x.UserQualifications)
                .Where(x => x.Id == candidateid)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            return obj;
        }

        public async Task<ReturnStringsDto> WriteProspectiveExcelToDB(string fileNameWithPath, string Username)
        {
            var strError = await _context.ReadProspectiveCandidateDataExcelFile(fileNameWithPath, Username);
            return strError;
        }
        
        public async Task<ReturnStringsDto> WriteProspectiveNaukriExcelToDB(string fileNameWithPath, string Username)
        {
            var strError = await _context.ReadNaukriProspectiveCandidateDataExcelFile(fileNameWithPath, Username);
            return strError;
        }
        
        public async Task<string> WriteCandidateExcelToDB(string fileNameWithPath, string Username)
        {
            return  await _context.ReadCandidateDataExcelFile(fileNameWithPath, Username);
        }


        public async Task<int> GetNextApplicationNo() {
            var no = await _context.Candidates.MaxAsync(x => x.ApplicationNo);
            return no==0 ? 1000 : no+1;
        }
    }
}