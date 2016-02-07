using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApiClient
{
   public class WebMethods : IDisposable, IWebMethods
   {
      private string currentToken = string.Empty;
      public HttpClient Client;

      public WebMethods()
      {
         this.Client = new HttpClient();
      }

      public WebMethods(string authenticated)
      {
         var handler = new HttpClientHandler
         {
            AllowAutoRedirect = false,
            UseCookies = false
         };
         this.currentToken = authenticated;
         this.Client = new HttpClient(handler);
      }

      private void AddAuthHeader()
      {
         Client.DefaultRequestHeaders.Accept.Clear();
         Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);
      }

      private void AddAcceptJson()
      {
         Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      private void AddApiHeaders()
      {
         AddAuthHeader();
         AddAcceptJson();
      }

      private Dictionary<string, string> GetTokenDictionary(string responseContent)
      {
         Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
         return tokenDictionary;
      }

      public HttpDataResponse Authenticate(Uri url, string userName, string password)
      {
         var pairs = new List<KeyValuePair<string, string>>
         {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", userName),
            new KeyValuePair<string, string>("password", password)
         };

         var content = new FormUrlEncodedContent(pairs);
         var tokenEndpoint = new Uri(url, "Token");
         HttpResponseMessage response = Client.PostAsync(tokenEndpoint, content).Result;
         //string responseContent = response.Content.ReadAsStringAsync().Result;
         //if (!response.IsSuccessStatusCode)
         //{
         //   throw new Exception(string.Format("Error: {0}", responseContent));
         //}
         //Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
         Dictionary<string, string> tokenDictionary = response.Content.ReadAsAsync<Dictionary<string, string>>().Result;
         currentToken = tokenDictionary["access_token"];
         return new HttpDataResponse { StatusCode = HttpStatusCode.OK };
      }

      public async Task<HttpDataResponse> AuthenticateAsync(Uri url, string userName, string password)
      {
         var pairs = new List<KeyValuePair<string, string>>
         {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", userName),
            new KeyValuePair<string, string>("password", password)
         };

         var content = new FormUrlEncodedContent(pairs);
         var tokenEndpoint = new Uri(url, "Token");
         HttpResponseMessage response = await Client.PostAsync(tokenEndpoint, content);
         //string responseContent = await response.Content.ReadAsStringAsync();
         //if (!response.IsSuccessStatusCode)
         //{
         //   throw new Exception(string.Format("Error: {0}", responseContent));
         //}
         //Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
         Dictionary<string, string> tokenDictionary = await response.Content.ReadAsAsync<Dictionary<string, string>>();
         currentToken = tokenDictionary["access_token"];
         return new HttpDataResponse { StatusCode = HttpStatusCode.OK };
      }

      public Uri GetUri(Uri baseUrl, string route, HttpMethodEnum method)
      {
         if (!route.EndsWith("/"))
            route = route + "/";
         switch (method)
         {
            case HttpMethodEnum.Create:
               return new Uri(baseUrl, route + "post");
            case HttpMethodEnum.Get:
               return new Uri(baseUrl, route + "get/");
            case HttpMethodEnum.DeleteByPut:
               return new Uri(baseUrl, route + "put");
            case HttpMethodEnum.Update:
               return new Uri(baseUrl, route + "put");
            default:
               return new Uri(baseUrl, route + method.ToString().ToLower());
         }
      }

      //Get
      public HttpDataResponse<Data> Get<Data>(Uri url, int id)
      {
         try
         {
            AddApiHeaders();
            var getUrl = new Uri(url, id.ToString());
            HttpResponseMessage result = Client.GetAsync(getUrl).Result;
            return GetResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      public HttpDataResponse<Data> Get<Data>(Uri url, Dictionary<string, object> getParams)
      {
         try
         {
            AddApiHeaders();
            var sb = new System.Text.StringBuilder();
            foreach (var entry in getParams)
            {
               if (sb.Length == 0)
                  sb.Append("?");
               else
                  sb.Append("&");

               sb.Append(entry.Key);
               sb.Append("=");
               sb.Append(entry.Value.ToString());
            }
            var getUrl = new Uri(url, sb.ToString());
            HttpResponseMessage result = Client.GetAsync(getUrl).Result;
            return GetResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      //Read
      public HttpDataResponse<Data> Read<Data>(Uri url, string request)
      {
         try
         {
            AddApiHeaders();
            var getUrl = new Uri(url, request);
            HttpResponseMessage result = Client.GetAsync(getUrl).Result;
            return GetResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      //Create
      public HttpDataResponse<Data> Create<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = Client.PostAsJsonAsync<Data>(url, data).Result;
            return ApiResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      //Update
      public HttpDataResponse<Data> Update<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = Client.PutAsJsonAsync<Data>(url, data).Result;
            return ApiResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }


      //Delete
      public HttpDataResponse<Data> Delete<Data>(Uri url)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = Client.DeleteAsync(url).Result;
            return ApiResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      //Delete by Put
      public HttpDataResponse<Data> DeleteByPut<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = Client.PutAsJsonAsync(url, data).Result;
            return ApiResponse<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      /* Async Methods */

      //Read
      public async Task<HttpDataResponse<Data>> ReadAsync<Data>(Uri url, string request)
      {
         try
         {
            AddApiHeaders();
            var getUrl = new Uri(url, request);
            HttpResponseMessage result = await Client.GetAsync(getUrl);
            return await GetResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }
      public async Task<HttpDataResponse<Data>> GetAsync<Data>(Uri url, int id)
      {
         try
         {
            AddApiHeaders();
            var getUrl = new Uri(url, id.ToString());
            HttpResponseMessage result = await Client.GetAsync(getUrl);
            return await GetResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }
      public async Task<HttpDataResponse<Data>> GetAsync<Data>(Uri url, Dictionary<string, object> getParams)
      {
         try
         {
            AddApiHeaders();
            var sb = new System.Text.StringBuilder();
            foreach (var entry in getParams)
            {
               if (sb.Length == 0)
                  sb.Append("?");
               else
                  sb.Append("&");

               sb.Append(entry.Key);
               sb.Append("=");
               sb.Append(entry.Value.ToString());
            }
            var getUrl = new Uri(url, sb.ToString());
            HttpResponseMessage result = await Client.GetAsync(getUrl);
            return await GetResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      //Create
      public async Task<HttpDataResponse<Data>> CreateAsync<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = await Client.PostAsJsonAsync<Data>(url, data);
            return await ApiResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }
      public async Task<HttpDataResponse<Data>> UpdateAsync<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = await Client.PutAsJsonAsync<Data>(url, data);
            return await ApiResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }
      //Delete
      public async Task<HttpDataResponse<Data>> DeleteAsync<Data>(Uri url)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = await Client.DeleteAsync(url);
            return await ApiResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      public async Task<HttpDataResponse<Data>> DeleteByPutAsync<Data>(Uri url, Data data)
      {
         try
         {
            AddApiHeaders();
            HttpResponseMessage result = await Client.PutAsJsonAsync(url, data);
            return await ApiResponseAsync<Data>(result);
         }
         catch (WebException exception)
         {
            return Error<Data>(exception.Source, HttpStatusCode.BadRequest);
         }
      }

      private HttpDataResponse<Data> ApiResponse<Data>(HttpResponseMessage result)
      {
         if (result.IsSuccessStatusCode)
         {
            return Response<Data>(result);
         }
         else
         {
            string httpErrorObject = result.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrWhiteSpace(httpErrorObject))
            {
               string errorResult = CreateApiException(httpErrorObject);
               if (!string.IsNullOrWhiteSpace(errorResult))
                  result.ReasonPhrase += errorResult;
            }
            return Error<Data>(result.ReasonPhrase, result.StatusCode);
         }
      }
      private async Task<HttpDataResponse<Data>> ApiResponseAsync<Data>(HttpResponseMessage result)
      {
         if (result.IsSuccessStatusCode)
         {
            //using (Stream responseStream = await result.Content.ReadAsStreamAsync())
            //{
            //   string jsonMessage = new StreamReader(responseStream).ReadToEnd();
            //   var response = new HttpDataResponse<Data>();
            //   response.StatusCode = result.StatusCode;
            //   response.Result = JsonConvert.DeserializeObject<Data>(jsonMessage);
            //   return response;
            //}
            return await ResponseAsync<Data>(result);
         }
         else
         {
            string httpErrorObject = await result.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(httpErrorObject))
            {
               string errorResult = CreateApiException(httpErrorObject);
               if (!string.IsNullOrWhiteSpace(errorResult))
                  result.ReasonPhrase += errorResult;
            }
            return Error<Data>(result.ReasonPhrase, result.StatusCode);
         }
      }
      private HttpDataResponse<Data> GetResponse<Data>(HttpResponseMessage result)
      {
         if (result.IsSuccessStatusCode)
         {
            return Response<Data>(result);
         }
         else
         {
            return Error<Data>(result.ReasonPhrase, result.StatusCode);
         }
      }

      private async Task<HttpDataResponse<Data>> GetResponseAsync<Data>(HttpResponseMessage result)
      {
         if (result.IsSuccessStatusCode)
         {
            return await ResponseAsync<Data>(result);
         }
         else
         {
            return Error<Data>(result.ReasonPhrase, result.StatusCode);
         }
      }

      private HttpDataResponse<Data> Error<Data>(string statusDescription, HttpStatusCode errorCode)
      {
         var response = new HttpDataResponse<Data>();
         response.StatusDescription = statusDescription;
         response.StatusCode = errorCode;
         return response;
      }

      private HttpDataResponse<Data> Response<Data>(HttpResponseMessage result)
      {
         var response = new HttpDataResponse<Data>();
         response.StatusCode = result.StatusCode;
         response.Result = result.Content.ReadAsAsync<Data>().Result;
         return response;
      }

      private async Task<HttpDataResponse<Data>> ResponseAsync<Data>(HttpResponseMessage result)
      {
         var response = new HttpDataResponse<Data>();
         response.StatusCode = result.StatusCode;
         response.Result = await result.Content.ReadAsAsync<Data>();
         return response;
      }

      public string CreateApiException(string httpErrorObject)
      {
         var sb = new System.Text.StringBuilder(":-");
         // Create an anonymous object to use as the template for deserialization:
         var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };

         // Deserialize:
         var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

         // Sometimes, there may be Model Errors:
         if (deserializedErrorObject != null && deserializedErrorObject.ModelState != null)
         {
            IEnumerable<string> errors = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));
            for (int i = 0; i < errors.Count(); i++)
            {
               // Wrap the errors up into the base Exception.Data Dictionary:
               sb.Append(errors.ElementAt(i));
            }
         }
         // Othertimes, there may not be Model Errors:
         else 
         {
            Dictionary<string, string> error = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
            foreach (var kvp in error)
            {
               // Wrap the errors up into the base Exception.Data Dictionary:
               sb.Append(kvp.Key);
               sb.Append(":");
               sb.Append(kvp.Value); 
            }
         }
         return ReplaceCarriageReturns(sb.ToString()); // when CarriageReturns present no error is returned
      }

      private string ReplaceCarriageReturns(string toProcess)
      {
         if ((toProcess == string.Empty) || (toProcess == null))
            return string.Empty;

         var regex = new System.Text.RegularExpressions.Regex(pattern: @"(\r\n|\r|\n)");
         return regex.Replace(toProcess, replacement: " ");
      }

      //private HttpDataResponse<Data> ExceptionHandler<Data>(WebException exception, HttpDataResponse<Data> response)
      //{
      //   response.StatusCode = HttpStatusCode.InternalServerError;
      //   if (exception.Response is HttpWebResponse)
      //   {
      //      var httpResponse = (HttpWebResponse)exception.Response;
      //      response.StatusCode = httpResponse.StatusCode;
      //   }

      //   if (exception.Response != null)
      //   {
      //      Stream responseStream = exception.Response.GetResponseStream();

      //      if (responseStream != null)
      //      {
      //         using (var reader = new StreamReader(responseStream))
      //         {
      //            response.StatusDescription = reader.ReadToEnd();
      //         }
      //      }
      //   }
      //   return response;
      //}

      //private HttpDataResponse ExceptionHandler(WebException exception, HttpDataResponse response)
      //{
      //   response.StatusCode = HttpStatusCode.InternalServerError;
      //   if (exception.Response is HttpWebResponse)
      //   {
      //      var httpResponse = (HttpWebResponse)exception.Response;
      //      response.StatusCode = httpResponse.StatusCode;
      //   }

      //   if (exception.Response != null)
      //   {
      //      Stream responseStream = exception.Response.GetResponseStream();

      //      if (responseStream != null)
      //      {
      //         using (var reader = new StreamReader(responseStream))
      //         {
      //            response.StatusDescription = reader.ReadToEnd();
      //         }
      //      }
      //   }
      //   return response;
      //}


      public void Dispose()
      {
         if (Client != null)
         {
            Client.Dispose();
            Client = null;
         }
      }
   }
}
