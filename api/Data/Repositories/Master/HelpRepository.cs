using api.Entities.Master;
using api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class HelpRepository : IHelpRepository
    {
        private readonly DataContext _context;
        public HelpRepository(DataContext context)
        {
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
    }
}