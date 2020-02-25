namespace Vsts.Domain.Contract.Dto
{
    public class VstsConfigurationData
    {
        public string Organization { get; set; }
        public string TeamProject { get; set; }
        public string Token { get; set; }
        public string ApiVersion { get; set; } = "5.1";
    }
}