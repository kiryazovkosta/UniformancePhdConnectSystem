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

    [Authorize]
    [RoutePrefix("phd")]
    public class PhdController : ApiController
    {
        private readonly ILogger logger = Log.ForContext<PhdController>();

        [HttpGet]
        [Route("tag-def")]
        public IHttpActionResult Get([FromUri] string host, string tag)
        {
            try
            {
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

                    var def = phd.TagDfn(tag);
                    this.logger.Debug($"tag-def: {def}");
                    return Ok(def);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("get-tag-data")]
        public IHttpActionResult GetTagData([FromUri] string host, string tag)
        {
            try
            {
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

                    var tagDfn = phd.TagDfn(tag);
                    var tagData = Utility.GetPhdTagData(tagDfn.Tables[0].Rows[0]);
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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

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
                using (var phd = new PHDHistorian())
                {
                    phd.DefaultServer = new PHDServer(host, SERVERVERSION.API200)
                    {
                        Port = 3100
                    };

                    var table = phd.DefaultServer.PutListDataTable;
                    var result = new StringBuilder();

                    foreach (DataRow row in table.Rows)
                    {
                        result.AppendLine("--- Row ---");
                        foreach (var item in row.ItemArray)
                        {
                            result.AppendLine($"Item: {item}");
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
