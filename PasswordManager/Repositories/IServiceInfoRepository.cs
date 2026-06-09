using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Repositories
{
    public interface IServiceInfoRepository
    {
        IEnumerable<ServiceInfo> GetAll();
        ServiceInfo? GetById(long id);
        ServiceInfo? GetByTitle(string title);
        ServiceInfo? GetByUrl(string url);
        void Create(ServiceInfo serviceInfo);
        void DeleteById(long id);
        void Update(ServiceInfo serviceInfo);
    }
}
