using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    internal interface AccountInfoRepository
    {
        IEnumerable<AccountInfo> GetAll();
        IEnumerable<AccountInfo> GetByUrl(string url);
        AccountInfo GetById(int id);
        void Create(AccountInfo accountInfo);
        void Delete(int id);
        void DeleteByUrl(string url);
        void Update(AccountInfo accountInfo);
    }
}
