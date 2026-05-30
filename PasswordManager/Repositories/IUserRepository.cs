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
        User GetById(int id);
        void Create(User user);
        void Delete(int id);
        void Update(User user);
    }
}
