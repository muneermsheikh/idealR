using System.Data.Common;
using api.Data.Repositories.Admin;
using api.DTOs;
using api.DTOs.HR;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
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
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        public CandidatesRepository(DataContext context, UserManager<AppUser> userManager, 
            IMapper mapper, IComposeMessagesHRRepository hrMsgRepo)
        {
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
                var query2 = (from cv in _context.Candidates 
                    join asses in _context.CandidateAssessments on cv.Id equals asses.CandidateId where asses.CVRefId==0
                    join item in _context.OrderItems on asses.OrderItemId equals item.Id
                    join cat in _context.Professions on item.ProfessionId equals cat.Id
                    join order in _context.Orders on item.OrderId equals order.Id
                    select new cvsAvailableDto {
                        CandAssessmentId = asses.Id, ApplicationNo = cv.ApplicationNo, City = cv.City, FullName = cv.FullName, 
                        CandidateId = cv.Id, AssessedOn = asses.AssessedOn, Gender=  (cv.Gender == "female" ? "F": "M").ToUpper(),
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

        public async Task<PagedList<CandidateBriefDto>> GetCandidates(CandidateParams candidateParams)
        {
                var query = _context.Candidates.Include(x => x.UserProfessions).AsQueryable();

                if(candidateParams.Id > 0) {
                    query = query.Where(x => x.Id == candidateParams.Id);}
                else {
                        if(!string.IsNullOrEmpty(candidateParams.CandidateName)) 
                            query = query.Where(x => x.FullName.ToLower().Contains(candidateParams.CandidateName.ToLower()));
                        
                        if(!string.IsNullOrEmpty(candidateParams.PassportNo))
                            query = query.Where(x => x.PpNo == candidateParams.PassportNo);
                        
                        if(candidateParams.ProfessionId != 0) {
                            var candidateids = await _context.UserProfessions.Where(x => x.ProfessionId == candidateParams.ProfessionId)
                                .Select(x => x.CandidateId).ToListAsync();
                            if(candidateids.Count > 0) {
                                query = query.Where(x => candidateids.Contains(x.Id));
                            }
                        }

                        if(candidateParams.AgentId > 0) query = query.Where(x => x.CustomerId == candidateParams.AgentId);
                    }
                
                var paged = await PagedList<CandidateBriefDto>.CreateAsync(query.AsNoTracking()
                        .ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider),
                        candidateParams.PageNumber, candidateParams.PageSize);
                
                return paged;

            
            
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
                .Include(x => x.UserQualifications)
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

            //edit UserPhones
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

            //edit UserExp
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
                     Divn = "Candidate"
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

                        if(att.AttachmentType.ToLower()=="photograph") cand.PhotoUrl=att.UploadedLocation + "/" + att.Name;
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

        public async Task<ICollection<UserAttachment>> UpdateCandidateAttachments(ICollection<UserAttachment> userAttachments)
        {
            var attachmentsToReturn = new List<UserAttachment>();

            foreach(var attach in userAttachments)
            {
                var existing = await _context.UserAttachments.Where(x => x.Name == attach.Name).FirstOrDefaultAsync();
                if(existing != null)  {
                    _context.Entry(existing).CurrentValues.SetValues(attach);
                    _context.Entry(existing).State = EntityState.Modified;

                    attachmentsToReturn.Add(existing);
                }
            }

            return await _context.SaveChangesAsync() > 0 ? attachmentsToReturn : null;
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
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            return obj;
        }

        public async Task<string> WriteProspectiveExcelToDB(string fileNameWithPath, string Username)
        {
            var count = await _context.ReadProspectiveCandidateDataExcelFile(fileNameWithPath, Username);
            return count;
        }
    }
}