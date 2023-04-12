namespace UniformancePhdConnectSystem.WebApi.Infrastructure
{
    using System;
    using System.Data;
    using UniformancePhdConnectSystem.Models.Phd;

    public class Utility
    {
        internal static TagData GetPhdTagData(DataRow tagDfn)
        {
            var record = new TagData()
            {
                Id = tagDfn.IsNull("Tagno") ? 0 : Convert.ToInt32(tagDfn["Tagno"]),
                Name = tagDfn.IsNull("TagName") ? string.Empty : Convert.ToString(tagDfn["TagName"]),
                Description = tagDfn.IsNull("Description") ? string.Empty : Convert.ToString(tagDfn["Description"]),
                Units = tagDfn.IsNull("Units") ? string.Empty : Convert.ToString(tagDfn["Units"]),
            };

            return record;
        }
    }
}