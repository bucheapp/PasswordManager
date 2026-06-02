using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public interface IServiceInfoService
    {
        void SetDB(long userId, string masterKey);
        void Create(ServiceInfo serviceInfo);
        void Delete(long id);
        void Update(ServiceInfo serviceInfo);
        List<ServiceInfo> GetAll();
        ServiceInfo? Get(long id);
    }
}
