using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NETMP.Module10.Http.HttpManager.Interfaces;

namespace NETMP.Module10.Http.HttpManager
{
    public class HttpResponseProvider : IHttpResponseProvider
    {
        private readonly HttpClient _httpClient;

        public HttpResponseProvider()
        {
            _httpClient = new HttpClient();
        }

        public string RequestHttpLayout(string uri)
        {
            return RequestHttpLayoutAsync(uri).Result;
        }

        public async Task<string> RequestHttpLayoutAsync(string uri)
        {
            var response = RequestHttpContent(uri);

            return await ConvertByteArrayToString(response.ReadAsByteArrayAsync());
        }

        public byte[] RequestLinkBytes(string uri)
        {
            return RequestLinkBytesAsync(uri).Result;
        }

        public async Task<byte[]> RequestLinkBytesAsync(string uri)
        {
            var response = RequestHttpContent(uri);

            return await response.ReadAsByteArrayAsync();
        }

        #region Private methods

        private HttpContent RequestHttpContent(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Null or empty uri.");
            }

            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out var uriAddress))
            {
                var response = _httpClient.GetAsync(uriAddress).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Failed to get data form {uri}");
                }

                return response.Content;
            }

            throw new ArgumentException("Passed value is not a valid URI address.");
        }

        private async Task<string> ConvertByteArrayToString(Task<byte[]> byteArray)
        {
            var htmlString = Encoding.Default.GetString(byteArray.Result);

            return await Task.FromResult(htmlString);
        }

        #endregion
    }
}
