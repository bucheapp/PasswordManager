using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    public interface IUserService
    {
        void Create(User user, string password);
        void Delete(string name);
        void Update(User user, string password);
        List<User> GetAll();
        User? Get(string name);
        User? Get(long id);
    }
}
