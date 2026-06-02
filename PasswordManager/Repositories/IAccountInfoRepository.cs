using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public interface IAccountInfoRepository
    {
        IEnumerable<AccountInfo> GetAll();
        AccountInfo? GetById(long id);
        AccountInfo? GetByName(string name);
        IEnumerable<AccountInfo> GetByServiceInfoId(long serviceInfoId);
        void Create(AccountInfo accountInfo);
        void DeleteById(long id);
        void Update(AccountInfo accountInfo);
    }
}
