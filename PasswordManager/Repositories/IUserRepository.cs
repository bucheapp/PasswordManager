using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    internal interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(long id);
        User GetByName(string name);
        void Create(User user);
        void DeleteById(long id);
        void DeleteByName(string name);
        void Update(User user);
    }
}
