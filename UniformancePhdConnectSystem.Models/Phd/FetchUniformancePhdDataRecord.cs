namespace UniformancePhdConnectSystem.Models.Phd
{
    using System;

    public class FetchUniformancePhdDataRecord
    {
        public int Confidence { get; set; }
        public string TagName { get; set; }
        public string HostName { get; set; }
        public DateTime TimeStamp { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{TagName},{HostName},{Confidence},{TimeStamp},{Value}";
        }
    }
}