using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PasswordManager.Services
{
    internal interface IWebSiteFetchService
    {
        WebsiteData Fetch(string url);
    }

    public class WebsiteData
    {
        public string Url { get; set; }
        public BitmapImage Image { get; set; }
        public string Titile { get; set; }

        public WebsiteData(string url,BitmapImage image,string title)
        {
            Url = url;
            Image = image;
            Titile = title;
        }
    }
}
