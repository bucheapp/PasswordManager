using PasswordManager.Models;
using PasswordManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace PasswordManager.Services
{
    internal class CacheService : ICacheServie
    {
        ICacheRepository _cacheRepository;
        private const string FaviconDirectoryPath = "favicon";
        public CacheService(ICacheRepository cacheRepository) {
            _cacheRepository = cacheRepository;
        }
        public void Add(WebsiteData websiteData)
        {
            string url = websiteData.Url;
            Uri uri = new Uri(url);
            string result = uri.GetLeftPart(UriPartial.Authority);

            if (result != null)
            {
                string imageUrl = OutputImage(websiteData.Image);
                if (imageUrl == null)
                {
                    throw new InvalidOperationException("Failed to generate image URL.");
                }

                Cache cache = new Cache(result, imageUrl, websiteData.Titile);
                _cacheRepository.Create(cache);
            }
        }

        private string OutputImage(BitmapImage image)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            string? path = null;
            int cnt = 0;

            while(true)
            {
                Guid uuid = Guid.NewGuid();
                path = uuid.ToString();

                if(!File.Exists(FaviconDirectoryPath + "/" + path))
                {
                    break;
                }

                cnt++;
                if(cnt >= 1000)
                {
                    break;
                }
            }

            using var stream = new FileStream(FaviconDirectoryPath + "/" + path, FileMode.Create);
            encoder.Save(stream);

            return path;
        }

        public Cache Load(string url)
        {
            return _cacheRepository.GetByUrl(url);
        }
        public void Clear()
        {
            _cacheRepository.DeleteAll();
        }
    }
}
