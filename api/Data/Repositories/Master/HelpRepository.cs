using api.DTOs.Admin;
using api.Entities.Identity;
using api.Entities.Master;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class HelpRepository : IHelpRepository
    {
        private readonly DataContext _context;
        private readonly IComposeMsgForIntrviews _composeMsg;
        private readonly IProspectiveCandidatesRepository _prosRepo;
        private readonly UserManager<AppUser> _userManager;

        public HelpRepository(DataContext context, IComposeMsgForIntrviews composeMsg, UserManager<AppUser> userManager,
            IProspectiveCandidatesRepository prosRepo)
        {
            _userManager = userManager;
            _composeMsg = composeMsg;
            _prosRepo = prosRepo;

            _context = context;
        }


        public async Task<Help> AddANewHelpTopic (string topic) {
            var helptopic = new Help{Topic=topic};
            _context.Helps.Add(helptopic);

            return await _context.SaveChangesAsync() > 0 ? helptopic : null;
        }

        public async Task<HelpItem> AddANewHelpSubTopic(int helpId, int seq, string helpText)
        {
            var helpItem = new HelpItem{HelpId = helpId, Sequence = seq, HelpSubTopic = helpText};

            _context.HelpItems.Add(helpItem);

            return await _context.SaveChangesAsync() > 0 ? helpItem : null;
        }

        public async Task<Help> GetHelpOnATopic(string topic)
        {
            return await _context.Helps.Where(x => x.Topic.ToLower() == topic.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<Help> GetHelpWithITems(int helpId)
        {
            return await _context.Helps.Include(x => x.HelpItems).Where(x => x.Id == helpId).FirstOrDefaultAsync();
        }

        public async Task<MessagesWithErrDto> GenerateInterviewInvitationMessages(ICollection<int> IntervwItemCandidateIds, 
            string loggedInUsername)
        {
            var dtoErr = new MessagesWithErrDto();
            
            //if scheduledTime msg already composed earlier, then issue edited Invitation, else simple Invitation

            var msgIds = await _context.Messages.Where(x => x.MessageType=="InterviewInvitation" 
                && IntervwItemCandidateIds.Contains(x.RecipientId)).Select(x => new {x.RecipientId, x.Id})
                .ToListAsync();
            var ids=new List<int>();
            var idsToEdit=new List<int>();
            foreach(var id in IntervwItemCandidateIds) {
                var msgId = msgIds.FirstOrDefault(x => x.Id == id);
                if(msgId != null) {
                    ids.Add(id) ;
                } else {
                    idsToEdit.Add(id);
                }
            }
            
            if(ids.Count > 0) {
                dtoErr = await _composeMsg.InviteCandidatesForInterviews(ids, loggedInUsername);
                //if prospective candidate, convert to candidates
                var existingInterviewCandidates = await _context.IntervwItemCandidates.Where(x => IntervwItemCandidateIds.Contains(x.Id)).ToListAsync();
                foreach(var cand in existingInterviewCandidates) {
                    if(cand.CandidateId == 0) {
                        await _prosRepo.ConvertProspectiveToCandidate(cand.ProspectiveCandidateId,"","", loggedInUsername);
                    }
                }
            }

            if(idsToEdit.Count > 0) {
                var dto = await _composeMsg.EditInviteForInterviews(idsToEdit, loggedInUsername);
                if(dto.Messages.Count > 0) {
                    foreach(var msg in dto.Messages) {dtoErr.Messages.Add(msg);}
                } else if(!string.IsNullOrEmpty(dto.ErrorString)) dtoErr.ErrorString += dto.ErrorString;
            }
            
       
            if(dtoErr.Messages.Count > 0 ) {
                foreach(var msg in dtoErr.Messages) {
                    _context.Entry(msg).State = EntityState.Added;
                }

                await _context.SaveChangesAsync();
            }

            return dtoErr;
        }

    }
}