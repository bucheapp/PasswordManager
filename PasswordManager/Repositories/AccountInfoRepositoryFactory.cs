using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public class AccountInfoRepositoryFactory : IAccountInfoRepositoryFactory
    {
        public IAccountInfoRepository Create(long userId,string masterKey)
        {
            var path = $"db/{userId}_accountInfo.db";
            var connectionString = $"Data Source={path};";

            return new AccountInfoRepository(connectionString,masterKey);
        }
    }
}
