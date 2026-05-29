using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    internal interface IAccountInfoService
    {
        void Create(AccountInfo accountInfo);
        void Delete(long id);
        void Delete(String url);
        void Update(AccountInfo accountInfo);
        List<AccountInfo> GetAll();
        AccountInfo Get(long id);
    }
}
