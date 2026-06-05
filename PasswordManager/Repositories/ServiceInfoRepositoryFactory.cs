using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public class ServiceInfoRepositoryFactory : IServiceInfoRepositoryFactory
    {
        public IServiceInfoRepository Create(long userId, string masterKey)
        {
            var path = $"db/{userId}_serviceInfo.db";
            var connectionString = $"Data Source={path};";
            return new ServiceInfoRepository(connectionString, masterKey);
        }
    }
}
