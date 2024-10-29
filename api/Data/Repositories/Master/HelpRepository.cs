using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.Master;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class HelpRepository : IHelpRepository
    {
        private readonly DataContext _context;
        private readonly IComposeMsgForIntrviews _composeMsg;
        private readonly IProspectiveCandidatesRepository _prosRepo;

        public HelpRepository(DataContext context, IComposeMsgForIntrviews composeMsg, 
            IProspectiveCandidatesRepository prosRepo)
        {
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
            var helpItem = new HelpItem{HelpId = helpId, Sequence = seq, HelpText = helpText};

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

        public async Task<MessagesWithErrDto> GenerateInterviewInvitationMessages(ICollection<int> IntervwItemCandidateIds, string loggedInUsername)
        {
            var msgs = await _composeMsg.InviteCandidatesForInterviews(IntervwItemCandidateIds, loggedInUsername);

            //if prospective candidate, convert to candidates
            var existingInterviewCandidates = await _context.IntervwItemCandidates.Where(x => IntervwItemCandidateIds.Contains(x.Id)).ToListAsync();
            foreach(var cand in existingInterviewCandidates) {
                if(cand.CandidateId == 0) {
                    await _prosRepo.ConvertProspectiveToCandidate(cand.ProspectiveCandidateId, loggedInUsername);
                }
            }
             
            if(msgs.Messages.Count > 0 ) {
                foreach(var msg in msgs.Messages) {
                    _context.Entry(msg).State = EntityState.Added;
                }

                await _context.SaveChangesAsync();

                return msgs;
            }

            return null;

        }

    }
}