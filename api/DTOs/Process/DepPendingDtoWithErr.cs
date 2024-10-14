namespace api.DTOs.Process
{
    public class DepPendingDtoWithErr
    {
        public string ErrorString { get; set; }
        public ICollection<DeploymentPendingBriefDto> DeploymentPendingBriefDtos { get; set; }
    }
}