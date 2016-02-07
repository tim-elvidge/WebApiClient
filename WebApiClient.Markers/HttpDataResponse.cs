namespace WebApiClient
{
   public sealed class HttpDataResponse : HttpDataResponse<string> { }
   public class HttpDataResponse<Data>
   {
      public System.Net.HttpStatusCode StatusCode { get; set; }
      public string StatusDescription { get; set; }
      public Data Result { get; set; }
   }
}
