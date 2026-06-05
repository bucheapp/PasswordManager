using PasswordManager.Models;
using PasswordManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public class ServiceInfoService : IServiceInfoService
    {
        private readonly IServiceInfoRepositoryFactory _factory;
        private readonly Dictionary<long, IServiceInfoRepository> _cache = [];

        public long UserId { get; set; }
        public string MasterKey { get; set; } = "";
        public ServiceInfoService(IServiceInfoRepositoryFactory factory)
        {
            _factory = factory;
        }

        private IServiceInfoRepository Repo
        {
            get
            {
                if (!_cache.TryGetValue(UserId, out var repo))
                {
                    repo = _factory.Create(UserId, MasterKey);
                    _cache[UserId] = repo;
                }
                return repo;
            }
        }

        public void SetDB(long userId, string masterKey)
        {
            UserId = userId;
            MasterKey = masterKey;
            var repo = _factory.Create(UserId, MasterKey);
            _cache[UserId] = repo;
        }
        public void Create(ServiceInfo serviceInfo)
        {
            CheckValidation(serviceInfo);
            long maxDisplayIndex = Repo
                .GetAll()
                .Select(u => u.DisplayIndex)
                .DefaultIfEmpty(0)
                .Max();
            serviceInfo.DisplayIndex = maxDisplayIndex + 1;
            Repo.Create(serviceInfo);
        }
        public void Delete(long id)
        {
            Repo.DeleteById(id);
        }
        public void Update(ServiceInfo serviceInfo)
        {
            CheckValidation(serviceInfo);

            var getServiceInfo = Repo.GetByTitle(serviceInfo.Title);

            if (getServiceInfo != null && getServiceInfo.Id != serviceInfo.Id)
            {
                throw new InvalidOperationException("A service with the same title already exists.");
            }
            Repo.Update(serviceInfo);
        }
        public List<ServiceInfo> GetAll()
        {
            return [.. Repo.GetAll()];
        }
        public ServiceInfo? Get(long id)
        {
            return Repo.GetById(id);
        }

        public ServiceInfo? Get(string title)
        {
            return Repo.GetByTitle(title);
        }

        private void CheckValidation(ServiceInfo serviceInfo)
        {
            ArgumentNullException.ThrowIfNull(serviceInfo);

            if (serviceInfo.Title.Length < 1 || serviceInfo.Title.Length > 30)
            {
                throw new ArgumentException("Title must be between 1 and 30 characters.");
            }

            if (serviceInfo.Url != null && !Uri.IsWellFormedUriString(serviceInfo.Url, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL format.");
            }
        }
    }
}
