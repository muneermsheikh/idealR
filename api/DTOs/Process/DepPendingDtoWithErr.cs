namespace api.DTOs.Process
{
    public class DepPendingDtoWithErr
    {
        public ICollection<DeploymentPendingDto> deploymentPendingDtos { get; set; }
        public string ErrorString { get; set; }
        public ICollection<DepItemAndDepIdDto> DepItemIdsInserted { get; set; }
    }
}