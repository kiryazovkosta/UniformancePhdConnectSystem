namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    using Newtonsoft.Json;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Http;
    using Uniformance.PHD;
    using UniformancePhdConnectSystem.Models.Enums;
    using UniformancePhdConnectSystem.Models.Phd;
    using UniformancePhdConnectSystem.WebApi.Infrastructure;
    using UniformancePhdConnectSystem.WebApi.Infrastructure.Extensions;

    [Authorize(Roles ="Admin,PhdUser")]
    [RoutePrefix("uniformance")]
    public class UniformanceController : BaseApiController
    {
        private readonly ILogger logger = Log.ForContext<UniformanceController>();

        [AllowAnonymous]
        [HttpGet]
        [Route("get-put-data")]
        public IHttpActionResult GetPutData()
        {
            var record = new PutUniformancePhdData(
                new PHDHistorianModel(new PHDServerModel("10.94.0.241;10.94.0.242")),
                new List<PutUniformancePhdDataRecord>()
                {
                    new PutUniformancePhdDataRecord("KT53531_REQN.SP", 9999, DateTime.UtcNow, 100),
                    new PutUniformancePhdDataRecord("KT53531_CARN.SP", 444444444444, DateTime.UtcNow, 100),
                    new PutUniformancePhdDataRecord("KT53531_DOSE.SP", 55555, DateTime.UtcNow, 100),
                });

            var json = JsonConvert.SerializeObject(record);
            return Ok(json);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("get-fetch-data")]
        public IHttpActionResult GetFetchData()
        {
            var record = new FetchUniformancePhdData(
                new PHDHistorianModel(
                    new PHDServerModel("10.94.0.241;10.94.0.242"))
                {
                    Sampletype = PhdSampleType.Raw
                },
                new List<string>() { "02FQI113_M_2.TOT_SM.OLDAV", "900FIQ205_V.TOT_S1.PV" },
                false);

            var json = JsonConvert.SerializeObject(record);
            return Ok(json);
        }

        [HttpPost]
        [Route("put")]
        public IHttpActionResult PutPhdData([FromBody] PutUniformancePhdData data)
        {
            try
            {
                var records = 0;

                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(data.PhdHistorian.PHDServer.HostName, (SERVERVERSION)data.PhdHistorian.PHDServer.APIVersion)
                    {
                        Port = data.PhdHistorian.PHDServer.Port
                    };
                    phd.Sampletype = (SAMPLETYPE)data.PhdHistorian.Sampletype;
                    phd.ReductionType = (REDUCTIONTYPE)data.PhdHistorian.ReductionType;
                    phd.SampleFrequency = data.PhdHistorian.SampleFrequency;
                    foreach (var record in data.Records)
                    {
                        var tag = new Tag(record.TagName);
                        var tagDfn = phd.TagDfn(tag.TagName);
                        var tagData = Utility.GetPhdTagData(tagDfn.Tables[0].Rows[0]);
                        try
                        {
                            phd.PutData(tag, record.Value, record.TimeStamp, record.Confidence, tagData.Units);
                            records++;
                        }
                        catch (PHDErrorException)
                        {
                        }
                    }
                }

                return Ok(records == data.Records.Count);
            }
            catch (PHDErrorException exception)
            {
                this.logger.Error(exception, exception.Message);
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        [Route("fetch")]
        public IHttpActionResult FetchPhdData([FromBody] FetchUniformancePhdData data)
        {
            this.logger.Debug($"fetch: {data.ToString()}");
            try
            {
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(data.PhdHistorian.PHDServer.HostName,
                        (SERVERVERSION)data.PhdHistorian.PHDServer.APIVersion)
                    {
                        Port = data.PhdHistorian.PHDServer.Port
                    };
                    phd.Sampletype = (SAMPLETYPE)data.PhdHistorian.Sampletype;
                    phd.ReductionType = (REDUCTIONTYPE)data.PhdHistorian.ReductionType;
                    phd.StartTime = data.PhdHistorian.StartTime;
                    phd.EndTime = data.PhdHistorian.EndTime;
                    phd.ConnectionTimeout = (uint)data.PhdHistorian.ConnectionTimeout;
                    phd.MaximumRows = (uint)data.PhdHistorian.MaximumRows;
                    if (data.PhdHistorian.SampleFrequency > uint.MinValue)
                    {
                        phd.SampleFrequency = data.PhdHistorian.SampleFrequency;
                    }

                    var tags = new Tags();
                    foreach (var tagName in data.Tags)
                    {
                        tags.Add(new Tag(tagName));
                    }

                    var fetchedData = phd.FetchRowData(tags, data.DSTCompensation, false);
                    this.logger.Debug(fetchedData.Tables[0].Print());

                    var records = new List<FetchUniformancePhdDataRecord>();
                    foreach (DataRow dataRow in fetchedData.Tables[0].Rows)
                    {
                        var record = new FetchUniformancePhdDataRecord()
                        {
                            Confidence = dataRow.IsNull("Confidence") ? 0 : Convert.ToInt32(dataRow["Confidence"]),
                            TagName = dataRow.IsNull("TagName") ? null : Convert.ToString(dataRow["TagName"]),
                            HostName = dataRow.IsNull("HostName") ? null : Convert.ToString(dataRow["HostName"]),
                            TimeStamp = dataRow.IsNull("Timestamp")
                                ? DateTime.MinValue
                                : Convert.ToDateTime(dataRow["Timestamp"]),
                            Value = dataRow.IsNull("Value") ? null : dataRow["Value"]
                        };

                        this.logger.Debug($"Record: {record.ToString()}");

                        records.Add(record);
                    }

                    return Ok(records);
                }
            }
            catch (Exception ex) when (ex is PHDErrorException || ex is Exception)
            {
                this.logger.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("fetch-single")]
        public IHttpActionResult FetchSinglePhdData([FromBody] FetchUniformancePhdData data)
        {
            if (data.Tags.Count != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(data.Tags));
            }

            try
            {
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(data.PhdHistorian.PHDServer.HostName, (SERVERVERSION)data.PhdHistorian.PHDServer.APIVersion)
                    {
                        Port = data.PhdHistorian.PHDServer.Port
                    };
                    phd.Sampletype = (SAMPLETYPE)data.PhdHistorian.Sampletype;
                    phd.ReductionType = (REDUCTIONTYPE)data.PhdHistorian.ReductionType;
                    phd.StartTime = data.PhdHistorian.StartTime;
                    phd.EndTime = data.PhdHistorian.EndTime;
                    phd.ConnectionTimeout = (uint)data.PhdHistorian.ConnectionTimeout;

                    var tags = new Tags();
                    foreach (var tagName in data.Tags)
                    {
                        tags.Add(new Tag(tagName));
                    }

                    var fetchedData = phd.FetchRowData(tags, data.DSTCompensation, false);
                    var record = fetchedData.Tables[0].Rows[0]["Value"];
                    return Ok(record);
                }
            }
            catch (Exception ex) when (ex is PHDErrorException || ex is Exception)
            {
                this.logger.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        private IEnumerable<FetchUniformancePhdDataRecord> DataTableToFetchDataRecords(DataTable dataTable)
        {
            var records = new List<FetchUniformancePhdDataRecord>();
            try
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var record = new FetchUniformancePhdDataRecord()
                    {
                        Confidence = dataRow.IsNull("Confidence") ? 0 : Convert.ToInt32(dataRow["Confidence"]),
                        TagName = dataRow.IsNull("TagName") ? null : Convert.ToString(dataRow["TagName"]),
                        HostName = dataRow.IsNull("HostName") ? null : Convert.ToString(dataRow["HostName"]),
                        TimeStamp = dataRow.IsNull("Timestamp") ? DateTime.MinValue : Convert.ToDateTime(dataRow["Timestamp"]),
                        Value = dataRow.IsNull("Value") ? null : dataRow["Value"]
                    };

                    records.Add(record);
                }
            }
            catch (Exception)
            {
            }

            return records;
        }
    }
}
