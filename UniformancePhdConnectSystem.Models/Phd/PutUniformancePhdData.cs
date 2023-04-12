namespace UniformancePhdConnectSystem.Models.Phd
{
    using System.Collections.Generic;

    public class PutUniformancePhdData
    {
        public PHDHistorianModel PhdHistorian { get; set; }

        public ICollection<PutUniformancePhdDataRecord> Records { get; set; }

        public PutUniformancePhdData(PHDHistorianModel phdHistorianParam, ICollection<PutUniformancePhdDataRecord> recordsParam)
        {
            this.PhdHistorian = phdHistorianParam;
            this.Records = recordsParam;
        }
    }
}