using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    internal class Cache
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public Cache(string url,string imageUrl, string title)
        {
            Url = url;
            ImageUrl = imageUrl;
            Title = title;
        }
    }
}
