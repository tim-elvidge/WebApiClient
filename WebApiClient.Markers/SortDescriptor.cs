using System;
using System.Linq;

namespace WebApiClient
{
   public class SortDescriptor
   {
      public string Member { get; set; }
      public SortDirection SortDirection { get; set; }
   }
}
