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
        private readonly Dictionary<long, IAccountInfoRepository> _cache = new();

        public long UserId { get; set; }
        public string MasterKey { get; set; }
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

            if (Repo.GetByName(accountInfo.Name) != null)
            {
                throw new InvalidOperationException(
                    "An accountInfo with the same name already exists.");
            }

            Repo.Create(accountInfo);
        }

        public void Delete(long id)
        {
            Repo.DeleteById(id);
        }

        public void Delete(string url)
        {
            Repo.DeleteByUrl(url);
        }

        public void Update(AccountInfo accountInfo)
        {
            CheckValidation(accountInfo);
            Repo.Update(accountInfo);
        }

        public List<AccountInfo> GetAll()
        {
            return Repo.GetAll().ToList();
        }

        public AccountInfo? Get(long id)
        {
            return Repo.GetById(id);
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
            }
        }
    }
}