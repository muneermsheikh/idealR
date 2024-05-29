using api.Entities.Master;

namespace api.Interfaces
{
    public interface IHelpRepository
    {
         Task<Help> GetHelpOnATopic (string topic);
         Task<Help> AddANewHelpTopic ( string helpText);
         Task<HelpItem> AddANewHelpSubTopic(int helpId, int seq, string subTopic);
         Task<Help> GetHelpWithITems(int helpId);

    }
}