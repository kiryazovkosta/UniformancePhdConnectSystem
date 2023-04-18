using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Uniformance.PHD;
using UniformancePhdConnectSystem.Models.Phd;
using UniformancePhdConnectSystem.WebApi.Infrastructure;

namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    [RoutePrefix("phd")]
    public class PhdController : ApiController
    {
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
                    return Ok(def);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("tag-data")]
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
                    return Ok(tagData);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("to-phd-time")]
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
                    return Ok(dateAsString);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("rdi-info")]
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

                return Ok(dtAbsolute);
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("c")]
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
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("parent-tag-list")]
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
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("link-list")]
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
                    return Ok(result);
                }
            }
            catch (PHDErrorException exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("rdi-list")]
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
