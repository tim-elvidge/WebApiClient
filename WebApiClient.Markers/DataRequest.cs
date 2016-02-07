using System;
using System.Linq;

namespace WebApiClient
{
   public class DataRequest
   {
      public System.Collections.Generic.List<FilterDescriptor> Filters { get; set; }
      public int Page { get; set; }
      public int PageSize { get; set; }
      public System.Collections.Generic.List<SortDescriptor> Sorts { get; set; }
      public string Serialize()
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder("?");
         sb.Append("sort=");
         if (this.Sorts.Count > 0)
         {
            foreach (var sort in this.Sorts)
            {
               sb.Append(sort.Member);
               sb.Append("-");
               sb.Append(sort.SortDirection == SortDirection.Ascending ? "asc" : "desc");
            }
         }
         sb.Append("&page=");
         sb.Append(this.Page.ToString());
         sb.Append("&pageSize=");
         sb.Append(this.PageSize.ToString());
         sb.Append("&group=");
         sb.Append("&filter=");
         if (this.Filters != null && this.Filters.Count > 0)
         {
            foreach (var filter in this.Filters)
            {
               sb.Append(filter.Member);
               sb.Append("~");
               sb.Append(filter.Operator.ToString());
               sb.Append("~");
               Type valType = filter.Value.GetType();
               if (valType == typeof(bool) || valType == typeof(System.Nullable<bool>))
               {
                  sb.Append(filter.Value.ToString().ToLower());
               }
               else if (valType == typeof(string))
               {
                  sb.Append("'");
                  sb.Append(filter.Value);
                  sb.Append("'");
               }
               else
                  sb.Append(filter.Value.ToString());
            }
         }
         return sb.ToString();
      }
   }
}
