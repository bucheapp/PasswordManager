using PasswordManager.Models;
using PasswordManager.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordManager.Services
{
    public class CacheService : ICacheService
    {
        ICacheRepository _cacheRepository;
        IWebSiteFetchService _webSiteFetchService;
        private const string FaviconDirectoryPath = "favicon";
        public CacheService(ICacheRepository cacheRepository,IWebSiteFetchService webSiteFetchService) {
            _cacheRepository = cacheRepository;
            _webSiteFetchService = webSiteFetchService;
        }
        public Cache? Add(WebsiteData websiteData)
        {
            string url = websiteData.Url;

            if (_cacheRepository.GetByUrl(url) != null)
            {
                throw new InvalidOperationException("This URL already exists.");
            }

            Uri uri = new(url);
            string result = uri.GetLeftPart(UriPartial.Authority);

            if (result != null)
            {
                string imageUrl = OutputImage(websiteData.ImageBytes);
                Cache cache = new()
                {
                    Url = result,
                    ImageUrl = imageUrl ?? throw new InvalidOperationException("Failed to generate image URL."),
                    NextUpdateAt = DateTime.Now.AddDays(10)
                };
                _cacheRepository.Create(cache);

                return cache;
            }

            return null;
        }
        private string OutputImage(byte[] imageBytes)
        {
            string? path = null;
            int cnt = 0;

            if(!File.Exists(FaviconDirectoryPath)) {
                Directory.CreateDirectory(FaviconDirectoryPath);
            }

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

            File.WriteAllBytes(FaviconDirectoryPath + "/" + path,imageBytes);

            return path;
        }

        public Cache? Load(string url)
        {
            Uri uri = new(url);
            string result = uri.GetLeftPart(UriPartial.Authority);
            Cache? cache = _cacheRepository.GetByUrl(result);
            if (cache != null)
            {
                if (cache.NextUpdateAt < DateTime.Now)
                {
                    _webSiteFetchService.Fetch(result)
                        .ContinueWith(task =>
                        {
                            if (task.IsCompletedSuccessfully)
                            {
                                WebsiteData websiteData = task.Result;
                                File.WriteAllBytes(FaviconDirectoryPath + "/" + cache.ImageUrl, websiteData.ImageBytes);
                            }
                        });

                }
            }

            return cache;
        }
        public void Clear()
        {
            _cacheRepository.DeleteAll();
        }
    }
}
