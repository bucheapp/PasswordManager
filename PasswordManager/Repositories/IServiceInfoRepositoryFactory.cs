using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public interface IServiceInfoRepositoryFactory
    {
        IServiceInfoRepository Create(long userId, string masterKey);
    }
}
