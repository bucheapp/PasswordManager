using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PasswordManager.Services
{
    public interface IWebSiteFetchService
    {
        Task<WebsiteData> Fetch(string url);
    }

    public class WebsiteData
    {
        public string Url { get; set; }
        public byte[] ImageBytes { get; set; }
        public WebsiteData(string url,byte[] imageBytes)
        {
            Url = url;
            ImageBytes = imageBytes;
        }
    }
}
