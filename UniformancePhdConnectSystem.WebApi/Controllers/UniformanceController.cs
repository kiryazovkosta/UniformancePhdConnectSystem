namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    using Newtonsoft.Json;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Models;
    using Providers;
    using Uniformance.PHD;
    using UniformancePhdConnectSystem.Models.Enums;
    using UniformancePhdConnectSystem.Models.Phd;
    using UniformancePhdConnectSystem.WebApi.Infrastructure;
    using UniformancePhdConnectSystem.WebApi.Infrastructure.Extensions;
    using System.Linq;

    [Authorize(Roles ="Admin,PhdUser")]
    [RoutePrefix("api/uniformance")]
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
            if (data == null || data.Records == null || data.Records.Count < 1)
            {
                this.logger.Warning("PutPhdData called with invalid tag count.");
                return BadRequest("Exactly one tag is required.");
            }

            var phdProvider = UniformancePhdProvider.Instance;
            lock (phdProvider.SyncLock)
            {
                var phd = phdProvider.GlobalHistorian;
                try
                {
                    if (phd == null)
                    {
                        this.logger.Error("PHD Global Historian is null. Initialization in Startup.cs failed.");
                        return InternalServerError(new Exception("PHD Connection is not available."));
                    }

                    var records = 0;
                    phd.Sampletype = (SAMPLETYPE)data.PhdHistorian.Sampletype;
                    phd.ReductionType = (REDUCTIONTYPE)data.PhdHistorian.ReductionType;
                    phd.SampleFrequency = data.PhdHistorian.SampleFrequency;
                    foreach (var record in data.Records)
                    {
                        var tag = new Tag(record.TagName);
                        TagData tagData;
                        using (var tagDfn = phd.TagDfn(tag.TagName))
                        {
                            tagData = Utility.GetPhdTagData(tagDfn.Tables[0].Rows[0]);
                        }
                        
                        phd.PutData(tag, record.Value, record.TimeStamp, record.Confidence, tagData.Units);
                        records++;
                    }

                    return Ok(records == data.Records.Count);
                }
                catch (PHDErrorException phdEx)
                {
                    this.logger.Error(phdEx, "PHD SDK Error [PutPhdData]: {Tags}", string.Join(";", data.Records.Select(r => r.TagName)));
                    return BadRequest($"PHD Error: {phdEx.Message}");
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "General exception in [PutPhdData] for tags: {Tags}", string.Join(";", string.Join(";", data.Records.Select(r => r.TagName))));
                    return InternalServerError(ex);
                }
                finally
                {
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("fetch")]
        public IHttpActionResult FetchPhdData([FromBody] FetchUniformancePhdData data)
        {
            if (data == null || data.Tags == null || data.Tags.Count < 1)
            {
                this.logger.Warning("FetchSinglePhdData called with invalid tag count.");
                return BadRequest("One or more tags are required.");
            }

            var phdProvider = UniformancePhdProvider.Instance;
            DataSet fetchedData = null;
            Tags tags = null;

            lock (phdProvider.SyncLock)
            {
                var phd = phdProvider.GlobalHistorian;
                try
                {
                    if (phd == null)
                    {
                        this.logger.Error("PHD Global Historian is null. Initialization in Startup.cs failed.");
                        return InternalServerError(new Exception("PHD Connection is not available."));
                    }

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

                    tags = new Tags();
                    foreach (var tagName in data.Tags)
                    {
                        tags.Add(new Tag(tagName));
                    }
                    fetchedData = phd.FetchRowData(tags, data.DSTCompensation, false);

                    var records = new List<FetchUniformancePhdDataRecord>();
                    foreach (DataRow dataRow in fetchedData.Tables[0].Rows)
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

                    this.logger.Debug($"End processing of request: {records.Count}");
                    var response = new FetchUniformancePhdResponse() { IsSuccess = true };
                    response.AddRecords(records);
                    return Ok(response);
                }
                catch (PHDErrorException phdEx)
                {
                    this.logger.Error(phdEx, "PHD SDK Error [FetchPhdData]: {Tags}", string.Join(";", data.Tags));
                    return BadRequest($"PHD Error: {phdEx.Message}");
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "General exception in [FetchPhdData] for tags: {Tags}", string.Join(";", data.Tags));
                    return InternalServerError(ex);
                }
                finally
                {
                    if (tags != null)
                    {
                        tags.RemoveAll();
                        tags = null;
                    }

                    if (fetchedData != null)
                    {
                        try
                        {
                            fetchedData.Tables.Clear();
                            fetchedData.Dispose();
                        }
                        catch (Exception disposeEx)
                        {
                            this.logger.Error(disposeEx, "Error disposing PHD DataSet.");
                        }
                        finally
                        {
                            fetchedData = null;
                        }
                    }
                }
            }
        }

        [HttpPost]
        [Route("fetch-single")]
        public IHttpActionResult FetchSinglePhdData([FromBody] FetchUniformancePhdData data)
        {
            if (data == null || data.Tags == null || data.Tags.Count != 1)
            {
                this.logger.Warning("FetchSinglePhdData called with invalid tag count.");
                return BadRequest("Exactly one tag is required.");
            }

            var phdProvider = UniformancePhdProvider.Instance;
            DataSet fetchedData = null;
            Tags tags = null;

            lock (phdProvider.SyncLock)
            {
                var phd = phdProvider.GlobalHistorian;
                try
                {
                    if (phd == null)
                    {
                        this.logger.Error("PHD Global Historian is null. Initialization in Startup.cs failed.");
                        return InternalServerError(new Exception("PHD Connection is not available."));
                    }

                    phd.Sampletype = (SAMPLETYPE)data.PhdHistorian.Sampletype;
                    phd.ReductionType = (REDUCTIONTYPE)data.PhdHistorian.ReductionType;
                    phd.StartTime = data.PhdHistorian.StartTime;
                    phd.EndTime = data.PhdHistorian.EndTime;
                    if (data.PhdHistorian.ConnectionTimeout > 0)
                    {
                        phd.ConnectionTimeout = (uint)data.PhdHistorian.ConnectionTimeout;
                    }

                    tags = new Tags();
                    tags.Add(new Tag(data.Tags.First().ToString()));
                    fetchedData = phd.FetchRowData(tags, data.DSTCompensation, false);

                    if (fetchedData != null && fetchedData.Tables.Count > 0 && fetchedData.Tables[0].Rows.Count > 0)
                    {
                        var row = fetchedData.Tables[0].Rows[0];
                        var value = row.IsNull("Value") ? null : row["Value"];
                        return Ok(value);
                    }

                    this.logger.Error("No data found for tag: {Tag}", data.Tags.First());
                    return NotFound();
                }
                catch (PHDErrorException phdEx)
                {
                    this.logger.Error(phdEx, "PHD SDK Error [FetchSinglePhdData]: {Tag}", data.Tags.First());
                    return BadRequest($"PHD Error: {phdEx.Message}");
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "General exception in [FetchSinglePhdData] for tag: {Tag}", data.Tags.First());
                    return InternalServerError(ex);
                }
                finally
                {
                    if (tags != null)
                    {
                        tags.RemoveAll();
                        tags = null;
                    }

                    if (fetchedData != null)
                    {
                        try
                        {
                            fetchedData.Tables.Clear();
                            fetchedData.Dispose();
                        }
                        catch (Exception disposeEx)
                        {
                            this.logger.Error(disposeEx, "Error disposing PHD DataSet.");
                        }
                        finally
                        {
                            fetchedData = null;
                        }
                    }
                }
            }
        }
    }
}
