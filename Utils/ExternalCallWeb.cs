using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionJujutsu.Utils
{
    internal static class ExternalCallWeb
    {
        public static async Task<string> CallUrl(string url)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            return response;
        }
    }
}
