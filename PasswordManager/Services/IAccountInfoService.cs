using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    public interface IAccountInfoService
    {
        void SetDB(long userId, string masterKey);
        void Create(AccountInfo accountInfo);
        void Delete(long id);
        void Update(AccountInfo accountInfo);
        List<AccountInfo> GetAll();
        AccountInfo? Get(long id);
    }
}
