namespace UniformancePhdConnectSystem.Models.Phd
{
    using System.Collections.Generic;
    using System.Text;

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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("FetchUniformancePhdData");
            sb.AppendLine($"\tPhdHistorian:");
            sb.AppendLine($"{this.PhdHistorian.ToString()}");
            sb.AppendLine($"\tTags: {string.Join("; ", this.Tags)}");
            sb.AppendLine($"\tDSTCompensation: {this.DSTCompensation}");
            return sb.ToString();
        }
    }
}