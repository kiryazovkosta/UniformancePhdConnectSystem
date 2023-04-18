using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace UniformancePhdConnectSystem.WebApi.Infrastructure.Extensions
{
    public static class DataTableExtensions
    {
        public static string Print(this DataTable table)
        {
            var output = new StringBuilder();
            output.AppendLine();
            if (null != table && null != table.Rows)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        output.Append(item);
                        output.Append(',');
                    }
                    output.AppendLine();
                }
            }

            return output.ToString(); 
        }
    }
}