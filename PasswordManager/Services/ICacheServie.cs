using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    internal interface ICacheServie
    {
        public void Add(WebsiteData websiteData);
        public Cache? Load(string url);
        public void Clear();
    }
}
