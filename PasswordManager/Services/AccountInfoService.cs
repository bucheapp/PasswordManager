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

        void Create(AccountInfo accountInfo)
        {
            checkValidation(accountInfo);
            _accountInfoRepository.Create(accountInfo);
        }
        void Delete(long id)
        {
            _accountInfoRepository.DeleteById(id);
        }
        void Delete(String url)
        {
            _accountInfoRepository.DeleteByUrl(url);
        }
        void Update(AccountInfo accountInfo)
        {
            checkValidation(accountInfo);
            _accountInfoRepository.Update(accountInfo);
        }
        List<AccountInfo> GetAll()
        {
            IEnumerable<AccountInfo> accountInfos = _accountInfoRepository.GetAll();
            return accountInfos.ToList();
        }
        AccountInfo Get(long id)
        {
            return _accountInfoRepository.GetById(id);
        }

        private void checkValidation(AccountInfo accountInfo)
        {
            if (accountInfo == null)
            {
                throw new ArgumentNullException(nameof(accountInfo));
            }


        }
    }
}
