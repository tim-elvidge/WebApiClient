using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClient
{
   public enum FilterTokenType
   {
      Property,
      ComparisonOperator,
      Or,
      And,
      Not,
      Function,
      Number,
      String,
      Boolean,
      DateTime,
      LeftParenthesis,
      RightParenthesis,
      Comma
   }
   
   public class FilterToken
   {
      public FilterTokenType TokenType
      {
         get;
         set;
      }

      public string Value
      {
         get;
         set;
      }
   }

   public static class FilterConversions
   {
      public static readonly IDictionary<string, FilterOperator> TokenToOperator = new Dictionary<string, FilterOperator>
        {
            { "eq", FilterOperator.IsEqualTo },
            { "neq", FilterOperator.IsNotEqualTo },
            { "lt", FilterOperator.IsLessThan },
            { "lte", FilterOperator.IsLessThanOrEqualTo },
            { "gt", FilterOperator.IsGreaterThan },
            { "gte", FilterOperator.IsGreaterThanOrEqualTo },
            { "startswith", FilterOperator.StartsWith },
            { "contains", FilterOperator.Contains },
            { "notsubstringof", FilterOperator.DoesNotContain },
            { "endswith", FilterOperator.EndsWith },
            { "doesnotcontain", FilterOperator.DoesNotContain }
        };

      public static readonly IDictionary<FilterOperator, string> OperatorToToken = new Dictionary<FilterOperator, string>
        {
            { FilterOperator.IsEqualTo, "eq" },
            { FilterOperator.IsNotEqualTo, "neq" },
            { FilterOperator.IsLessThan, "lt" },
            { FilterOperator.IsLessThanOrEqualTo, "lte" },
            { FilterOperator.IsGreaterThan, "gt" },
            { FilterOperator.IsGreaterThanOrEqualTo, "gte" },
            { FilterOperator.StartsWith, "startswith" },
            { FilterOperator.Contains, "contains" },
            { FilterOperator.DoesNotContain,"notsubstringof" },
            { FilterOperator.EndsWith, "endswith" }
        };

   }
}
