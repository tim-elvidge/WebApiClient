using System;
using System.Threading.Tasks;

namespace WebApiClient
{
   public interface IWebMethods
   {
      HttpDataResponse Authenticate(Uri url, string userName, string password);
      Task<HttpDataResponse> AuthenticateAsync(Uri url, string userName, string password);
      Uri GetUri(Uri baseUri, string route, HttpMethodEnum method);
      HttpDataResponse<Data> Get<Data>(Uri url, int id);
      HttpDataResponse<Data> Get<Data>(Uri url, System.Collections.Generic.Dictionary<string, object> getParams);
      Task<HttpDataResponse<Data>> GetAsync<Data>(Uri url, int id);
      Task<HttpDataResponse<Data>> GetAsync<Data>(Uri url, System.Collections.Generic.Dictionary<string, object> getParams);
      HttpDataResponse<Data> Read<Data>(Uri url, string request);
      Task<HttpDataResponse<Data>> ReadAsync<Data>(Uri url, string request);
      HttpDataResponse<Data> Create<Data>(Uri url, Data data);
      Task<HttpDataResponse<Data>> CreateAsync<Data>(Uri url, Data data);
      HttpDataResponse<Data> Update<Data>(Uri url, Data data);
      Task<HttpDataResponse<Data>> UpdateAsync<Data>(Uri url, Data data);
      HttpDataResponse<Data> Delete<Data>(Uri url);
      Task<HttpDataResponse<Data>> DeleteAsync<Data>(Uri url);
      HttpDataResponse<Data> DeleteByPut<Data>(Uri url, Data data);
      Task<HttpDataResponse<Data>> DeleteByPutAsync<Data>(Uri url, Data data);
      void Dispose();
   }
}