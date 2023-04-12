namespace UniformancePhdConnectSystem.Models.Phd
{
    using System.Collections.Generic;

    public class FetchUniformancePhdData
    {
        public FetchUniformancePhdData(
            PHDHistorianModel phdHistorianParam, 
            ICollection<string> tagsParam, 
            bool dstCompensationParam = false)
        {
            this.PhdHistorian = phdHistorianParam;
            this.Tags = tagsParam;
            this.DSTCompensation = dstCompensationParam;
        }

        public PHDHistorianModel PhdHistorian { get; set; }

        public ICollection<string> Tags { get; set; }

        public bool DSTCompensation { get; set; }
    }
}