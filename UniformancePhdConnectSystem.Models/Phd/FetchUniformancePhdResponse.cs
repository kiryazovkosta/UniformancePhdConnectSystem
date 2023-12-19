// ------------------------------------------------------------------------------------------------
//  <copyright file="FetchUniformancePhdResponse.cs" company="Business Management System Ltd.">
//      Copyright "2023" (c), Business Management System Ltd.
//      All rights reserved.
//  </copyright>
//  <author>Kosta.Kiryazov</author>
// ------------------------------------------------------------------------------------------------

namespace UniformancePhdConnectSystem.Models.Phd
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class FetchUniformancePhdResponse
    {
        private IList<FetchUniformancePhdDataRecord> records;
        
        public FetchUniformancePhdResponse()
        {
            this.records = new Collection<FetchUniformancePhdDataRecord>();
        }
        
        public bool IsSuccess { get; set; } = false;

        public ICollection<FetchUniformancePhdDataRecord> Records => 
            new ReadOnlyCollection<FetchUniformancePhdDataRecord>(this.records);
        
        public string JwtToken { get; set; }
        
        public string Error { get; set; }

        public void AddRecords(IList<FetchUniformancePhdDataRecord> recordsParam)
        {
            this.records = recordsParam;
        }
    }
}