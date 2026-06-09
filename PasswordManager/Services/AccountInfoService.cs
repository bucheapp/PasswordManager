using PasswordManager.Models;
using PasswordManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public class AccountInfoService : IAccountInfoService
    {
        private readonly IAccountInfoRepositoryFactory _factory;
        private readonly Dictionary<long, IAccountInfoRepository> _cache = [];

        public long UserId { get; set; }
        public string MasterKey { get; set; } = "";
        public AccountInfoService(IAccountInfoRepositoryFactory factory)
        {
            _factory = factory;
        }

        private IAccountInfoRepository Repo
        {
            get
            {
                if (!_cache.TryGetValue(UserId, out var repo))
                {
                    repo = _factory.Create(UserId,MasterKey);
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

        public void Create(AccountInfo accountInfo)
        {
            CheckValidation(accountInfo);

            if (Repo.GetByNameAndServiceInfoId(accountInfo.Name,accountInfo.ServiceInfoId) != null)
            {
                throw new InvalidOperationException(
                    "An accountInfo with the same name already exists.");
            }

            long maxDisplayIndex = Repo
                .GetAll()
                .Select(u => u.DisplayIndex)
                .DefaultIfEmpty(0)
                .Max();
            accountInfo.DisplayIndex = maxDisplayIndex + 1;
            Repo.Create(accountInfo);
        }
        public void Create(AccountInfo accountInfo, ServiceInfo serviceInfo)
        {

        }
        public void Delete(long id)
        {
            Repo.DeleteById(id);
        }
        public void Update(AccountInfo accountInfo)
        {
            CheckValidation(accountInfo);
            
            var getAccountInfo = Repo.GetByName(accountInfo.Name);

            if (getAccountInfo != null && getAccountInfo.Id != accountInfo.Id)
            {
                throw new InvalidOperationException("An account with the same name already exists.");
            }

            Repo.Update(accountInfo);
        }

        public List<AccountInfo> GetAll()
        {
            return [.. Repo.GetAll()];
        }

        public AccountInfo? Get(long id)
        {
            return Repo.GetById(id);
        }
        public List<AccountInfo> GetByServiceInfoId(long serviceInfoId)
        {
            return [.. Repo.GetByServiceInfoId(serviceInfoId)];
        }

        private void CheckValidation(AccountInfo accountInfo)
        {
            ArgumentNullException.ThrowIfNull(accountInfo);

            if (accountInfo.AuthType is AuthType.UsernamePassword
                or AuthType.EmailPassword)
            {
                if (string.IsNullOrWhiteSpace(accountInfo.Name))
                {
                    throw new ArgumentException("User name cannot be blank.");
                }

                if (string.IsNullOrWhiteSpace(accountInfo.Password))
                {
                    throw new ArgumentException("Password cannot be blank.");
                }

                if(accountInfo.Name.Length > 50)
                {
                    throw new ArgumentException("Account name must be less than 50 characters.");
                }

                if (accountInfo.Password.Length > 300)
                {
                    throw new ArgumentException("Account name must be less than 300 characters.");
                }
            }
        }
    }
}