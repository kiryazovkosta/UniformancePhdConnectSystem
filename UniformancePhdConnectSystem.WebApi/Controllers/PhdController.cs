namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    using Serilog;
    using System;
    using System.Data;
    using System.Text;
    using System.Web.Http;
    using Uniformance.PHD;
    using UniformancePhdConnectSystem.Models.Phd;
    using UniformancePhdConnectSystem.WebApi.Infrastructure;
    using UniformancePhdConnectSystem.WebApi.Providers;

    [Authorize]
    [RoutePrefix("api/phd")]
    public class PhdController : ApiController
    {
        private readonly ILogger logger = Log.ForContext<PhdController>();

        [HttpGet]
        [Route("tag-def")]
        public IHttpActionResult Get([FromUri] string host, string tag)
        {
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

                    phd.DefaultServer.HostName = host;
                    TagData tagData ;
                    using (var def = phd.TagDfn(tag))
                    {
                        tagData = Utility.GetPhdTagData(def.Tables[0].Rows[0]);
                    }

                    this.logger.Debug($"tag-def: {tagData}");
                    return Ok(tagData);
                }
                catch (PHDErrorException phdEx)
                {
                    this.logger.Error(phdEx, "PHD SDK Error [tag-def]: {Tag}", tag);
                    return BadRequest($"PHD Error: {phdEx.Message}");
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "General exception in [tag-def] for tags: {Tag}", tag);
                    return InternalServerError(ex);
                }
            }
        }

        [HttpGet]
        [Route("get-tag-data")]
        public IHttpActionResult GetTagData([FromUri] string host, string tag)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    TagData tagData;
                    using (var tagDfn = phd.TagDfn(tag))
                    {
                        tagData = Utility.GetPhdTagData(tagDfn.Tables[0].Rows[0]);
                    }

                    this.logger.Debug($"get-tag-data: {tagData}");
                    return Ok(tagData);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("host")]
        public IHttpActionResult ConvertToPHDTime([FromUri] string host, DateTime date)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var dateAsString = phd.ConvertToPHDTime(date);
                    this.logger.Debug($"to-phd-time: {dateAsString}");
                    return Ok(dateAsString);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-rdi-info")]
        public IHttpActionResult GetRDIInfo([FromUri] string host, string rdi)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = phd.GetRDIInfo(rdi);
                    this.logger.Debug($"get-rdi-info: {result}");
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("rel-to-abs")]
        public IHttpActionResult GetRelativeToAbsolute([FromUri] string host, string datetime)
        {
            var dtAbsolute = new DateTime();
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    phd.RelativeToAbsolute(datetime, ref dtAbsolute, true);
                }

                this.logger.Debug($"rel-to-abs: {dtAbsolute}");
                return Ok(dtAbsolute);
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-db-info")]
        public IHttpActionResult GetDBInfo([FromUri] string host)
        {
            var response = new DbInfoData();
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var dBMProvider = string.Empty;
                    var dBMDataSource = string.Empty;
                    var useInts = int.MinValue;
                    phd.GetDBInfo(ref dBMProvider, ref dBMDataSource, ref useInts);
                    response.DBMProvider = dBMProvider;
                    response.DBMDataSource = dBMDataSource;
                    response.UsingINTS = useInts;
                }

                this.logger.Debug($"get-db-info: {response}");
                return Ok(response);
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("browse-tags-list")]
        public IHttpActionResult BrowsingTagsList([FromUri] string host, uint maxTagCount)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = phd.BrowsingTagsList(maxTagCount, new TagFilter() { });
                    this.logger.Debug($"browse-tags-list: {result}");
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-parent-tag-list")]
        public IHttpActionResult GetParentTagList([FromUri] string host)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = phd.GetParentTagList();
                    this.logger.Debug($"get-parent-tag-list: {result}");
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-link-list")]
        public IHttpActionResult GetLinkList([FromUri] string host)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = phd.GetLinkList();
                    this.logger.Debug($"get-link-list: {result}");
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-rdi-list")]
        public IHttpActionResult GetRDIList([FromUri] string host)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = phd.GetRDIList();
                    this.logger.Debug($"get-link-list: {result}");
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("put-list-data")]
        public IHttpActionResult GetPutListDataTable([FromUri] string host)
        {
            try
            {
                using (var server = new PHDServer(host, SERVERVERSION.API200) { Port = 3100 })
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = server;

                    var result = new StringBuilder();
                    using (var table = phd.DefaultServer.PutListDataTable)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            result.AppendLine("--- Row ---");
                            foreach (var item in row.ItemArray)
                            {
                                result.AppendLine($"Item: {item}");
                            }
                        }
                    }

                    this.logger.Debug($"put-list-data: {result.ToString()}");
                    return Ok(result.ToString());
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
