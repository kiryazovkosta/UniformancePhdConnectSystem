namespace UniformancePhdConnectSystem.Models.Phd
{
    using UniformancePhdConnectSystem.Models.Enums;

    public class PHDServerModel
    {
        public PHDServerModel(
            string hostNameParam, 
            int portParam = 3150, 
            PhdServerVersionType apiVersionParam = PhdServerVersionType.API200)
        {
            this.HostName = hostNameParam;
            this.Port = portParam;
            this.APIVersion = apiVersionParam;
        }

        public string HostName { get; set; }
        public int Port { get; set; }
        public PhdServerVersionType APIVersion { get; set; }
        public int RequestTimeout { get; set; }
    }
}