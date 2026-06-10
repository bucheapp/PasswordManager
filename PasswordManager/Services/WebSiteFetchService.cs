using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public class WebSiteFetchService : IWebSiteFetchService
    {
        private const string _googleFaviconBaseUrl = "https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=";
        private static readonly HttpClient _client = new HttpClient();
        public async Task<WebsiteData> Fetch(string url)
        {
            string faviconUrl = $"{_googleFaviconBaseUrl}{url}&size=32";
            byte[] imageBytes = await _client.GetByteArrayAsync(faviconUrl);

            return new WebsiteData(url,imageBytes);
        }
    }
}
