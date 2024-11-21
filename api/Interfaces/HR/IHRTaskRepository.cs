namespace api.Interfaces.HR
{
    public interface IHRTaskRepository
    {
        Task<string> AssignTasksToHRExecs(ICollection<int> orderItemIds, string Username);
        
    }
}