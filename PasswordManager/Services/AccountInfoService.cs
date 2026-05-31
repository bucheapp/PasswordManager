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
    internal class AccountInfoService : IAccountInfoService
    {
        IAccountInfoRepository _accountInfoRepository;
        public AccountInfoService(IAccountInfoRepository accountInfoRepository)
        {
            _accountInfoRepository = accountInfoRepository;
        }

        public void Create(AccountInfo accountInfo)
        {
            checkValidation(accountInfo);

            if(_accountInfoRepository.GetByName(accountInfo.Name) != null)
            {
                throw new InvalidOperationException("A accountInfo with the same name already exists.");
            }

            _accountInfoRepository.Create(accountInfo);
        }
        public void Delete(long id)
        {
            _accountInfoRepository.DeleteById(id);
        }
        public void Delete(string url)
        {
            _accountInfoRepository.DeleteByUrl(url);
        }
        public void Update(AccountInfo accountInfo)
        {
            checkValidation(accountInfo);

            if (_accountInfoRepository.GetByName(accountInfo.Name) != null)
            {
                throw new InvalidOperationException("A accountInfo with the same name already exists.");
            }

            _accountInfoRepository.Update(accountInfo);
        }
        public List<AccountInfo> GetAll()
        {
            IEnumerable<AccountInfo> accountInfos = _accountInfoRepository.GetAll();
            return accountInfos.ToList();
        }
        public AccountInfo? Get(long id)
        {
            return _accountInfoRepository.GetById(id);
        }

        private void checkValidation(AccountInfo accountInfo)
        {
            if (accountInfo == null)
            {
                throw new ArgumentNullException(nameof(accountInfo));
            }

            if(accountInfo.AuthType == AuthType.UsernamePassword
                ||
                accountInfo.AuthType == AuthType.EmailPassword)
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
