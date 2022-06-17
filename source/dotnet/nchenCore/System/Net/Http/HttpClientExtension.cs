using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtension
    {
        public static void SetBearerAuthorization(this HttpClient client, string token)
            => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        public static void AddHeaders(this HttpClient client, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }


        public async static Task<HttpResponseMessage> SendJsonAsync(this HttpClient client, HttpMethod method, string requestUri, string json)
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(method, requestUri) { Content = content };
            return await client.SendAsync(request);
        }
        public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string requestUri, string json)
            => client.SendJsonAsync(HttpMethod.Post, requestUri, json);
        public static Task<HttpResponseMessage> PutJsonAsync(this HttpClient client, string requestUri, string json)
            => client.SendJsonAsync(HttpMethod.Put, requestUri, json);


        public async static Task<HttpResponseMessage> SendFormAsync(this HttpClient client, HttpMethod method, string requestUri, Dictionary<string, string> values)
        {
            using var content = new FormUrlEncodedContent(values);
            using var request = new HttpRequestMessage(method, requestUri) { Content = content };
            return await client.SendAsync(request);
        }
        public static Task<HttpResponseMessage> PostFormAsync(this HttpClient client, string requestUri, Dictionary<string, string> values)
            => client.SendFormAsync(HttpMethod.Post, requestUri, values);
        public static Task<HttpResponseMessage> PutFormAsync(this HttpClient client, string requestUri, Dictionary<string, string> values)
            => client.SendFormAsync(HttpMethod.Put, requestUri, values);
    }
}
