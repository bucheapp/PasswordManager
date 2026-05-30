using System;
using System.Collections.Generic;
using System.Linq;
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
        public BitmapImage Image { get; set; }
        public string Titile { get; set; }

        public WebsiteData(BitmapImage image,string title)
        {
            Image = image;
            Titile = title;
        }
    }
}
