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

        public Cache(string url,string imageUrl)
        {
            this.Url = url;
            this.ImageUrl = imageUrl;
        }
    }
}
