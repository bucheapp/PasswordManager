using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public interface ICacheRepository
    {
        IEnumerable<Cache> GetAll();
        Cache? GetByUrl(string url);
        void Create(Cache cache);
        void DeleteAll();
    }
}
