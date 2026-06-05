using PasswordManager.Models;
using PasswordManager.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Create(User user,string password)
        {
            CheckValidation(user,password);

            if(_userRepository.GetByName(user.Name) != null) {
                throw new InvalidOperationException("A user with the same name already exists.");
            }

            long maxDisplayIndex = _userRepository
                .GetAll()
                .Select(u => u.DisplayIndex)
                .DefaultIfEmpty(0)
                .Max();
            user.DisplayIndex = maxDisplayIndex + 1;

            _userRepository.Create(user);
        }

        public void Delete(string name)
        {
            _userRepository.DeleteByName(name);
        }
        public User? Get(string name)
        {
            return _userRepository.GetByName(name);
        }
        public User? Get(long id)
        {
            return _userRepository.GetById(id);
        }
        public List<User> GetAll()
        {
            IEnumerable<User> users = _userRepository.GetAll();
            return [.. users];
        }
        public void Update(User user,string password)
        {
            CheckValidation(user, password);

            User? getUser = _userRepository.GetByName(user.Name);
            if (getUser != null && getUser.Id != user.Id)
            {
                throw new InvalidOperationException("A user with the same name already exists.");
            }

            _userRepository.Update(user);
        }
        private void CheckValidation(User user,string password)
        {
            if (user == null)
            {
                ArgumentNullException.ThrowIfNull(user, nameof(user));
            }

            if (user.Name.Length < 3 || user.Name.Length > 10)
            {
                throw new ArgumentException("Username must be between 3 and 10 characters long.");
            }

            if (!Regex.IsMatch(user.Name, @"^[\p{L}\p{N}]+$"))
            {
                throw new ArgumentException("Username can only contain letters and numbers.");
            }

            if(password == null)
            {
                return;
            }

            if (password.Length < 5 || password.Length > 30)
            {
                throw new ArgumentException("Password must be between 5 and 30 characters long.");
            }

            if (!Regex.IsMatch(password, @"^[\dA-Za-z]+$"))
            {
                throw new ArgumentException("Password must contain only alphanumeric characters.");
            }
        }
    }
}
