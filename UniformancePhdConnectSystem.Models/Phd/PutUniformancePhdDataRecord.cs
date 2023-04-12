namespace UniformancePhdConnectSystem.Models.Phd
{
    using System;

    public class PutUniformancePhdDataRecord
    {
        public PutUniformancePhdDataRecord(
            string tagNameParam, 
            object valueParam, 
            DateTime timestampParam, 
            sbyte confidenceParam = 100)
        {
            this.TagName = tagNameParam;
            this.Value = valueParam;
            this.TimeStamp = timestampParam;
            this.Confidence = confidenceParam;
        }

        public string TagName { get; set; }

        public object Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public sbyte Confidence { get; set; }
    }
}