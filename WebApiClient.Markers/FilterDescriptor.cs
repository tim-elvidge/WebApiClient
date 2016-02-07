using System;
using System.Linq;

namespace WebApiClient
{

   public class FilterDescriptor
   {
      public string Member { get; set; }
      public FilterOperator Operator { get; set; }
      public object Value { get; set; }
   }
}
