using System.Threading.Tasks;

namespace NETMP.Module10.Http.HttpManager.Interfaces
{
    public interface IHttpResponseProvider
    {
        string RequestHttpLayout(string uri);

        Task<string> RequestHttpLayoutAsync(string uri);

        byte[] RequestLinkBytes(string uri);

        Task<byte[]> RequestLinkBytesAsync(string uri);
    }
}
