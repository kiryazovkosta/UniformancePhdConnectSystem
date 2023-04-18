namespace UniformancePhdConnectSystem.Models.Phd
{
    using System;
    using UniformancePhdConnectSystem.Models.Enums;

    public class PHDServerModel
    {
        public PHDServerModel(
            string hostNameParam, 
            int portParam = 3100, 
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

        public override string ToString()
        {
            return $"Host: {this.HostName}, Port:{this.Port}, APIVersion: {this.APIVersion}, RT:{this.RequestTimeout}";
        }
    }
}