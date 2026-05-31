using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    internal interface IAccountInfoRepository
    {
        IEnumerable<AccountInfo> GetAll();
        IEnumerable<AccountInfo> GetByUrl(string url);
        AccountInfo? GetById(long id);
        AccountInfo? GetByName(string name);
        void Create(AccountInfo accountInfo);
        void DeleteById(long id);
        void DeleteByUrl(string url);
        void Update(AccountInfo accountInfo);
    }
}
