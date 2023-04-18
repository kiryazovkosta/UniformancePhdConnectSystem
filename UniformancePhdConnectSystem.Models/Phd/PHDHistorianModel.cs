namespace UniformancePhdConnectSystem.Models.Phd
{
    using System.Text;
    using UniformancePhdConnectSystem.Models.Enums;

    public class PHDHistorianModel
    {
        public PHDHistorianModel(PHDServerModel phdServerParam, 
            string startTimeParam = "Now - 1H", 
            string endTimeParam = "Now")
        {
            PHDServer = phdServerParam;
            StartTime = startTimeParam;
            EndTime = endTimeParam;
        }

        public PHDServerModel PHDServer { get; set; }
        public int ConnectionTimeout { get; set; }
        public string EndTime { get; set; }
        public int MaximumRows { get; set; }
        public int MinimumConfidence { get; set; }
        public int Offset { get; set; }
        public PhdSampleType Sampletype { get; set; }
        public string StartTime { get; set; }
        public PhdReductionType ReductionType { get; set; }
        public uint SampleFrequency { get; set; }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"\t\tPHD Server: {this.PHDServer.ToString()}");
            output.AppendLine($"\t\tConnectionTimeout: {this.ConnectionTimeout}");
            output.AppendLine($"\t\tStartTime: {this.StartTime}");
            output.AppendLine($"\t\tEndTime: {this.EndTime}");
            output.AppendLine($"\t\tMaximumRows: {this.MaximumRows}");
            output.AppendLine($"\t\tMinimumConfidence: {this.MinimumConfidence}");
            output.AppendLine($"\t\tOffset: {this.Offset}");
            output.AppendLine($"\t\tSampletype: {this.Sampletype}");
            output.AppendLine($"\t\tReductionType: {this.ReductionType}");
            output.Append($"\t\tSampleFrequency: {this.SampleFrequency}");
            return output.ToString();
        }
    }
}
