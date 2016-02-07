using System;
using System.Linq;

namespace WebApiClient
{
   public static class FilterTokenExtensions
   {
      public static FilterOperator ToFilterOperator(this FilterToken token)
      {
         return FilterConversions.TokenToOperator[token.Value];
      }

      public static string ToToken(this FilterOperator filterOperator)
      {
         return FilterConversions.OperatorToToken[filterOperator];
      }
   }
}
