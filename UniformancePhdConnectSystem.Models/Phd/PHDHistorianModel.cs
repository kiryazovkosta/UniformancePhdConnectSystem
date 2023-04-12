namespace UniformancePhdConnectSystem.Models.Phd
{
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
    }
}
