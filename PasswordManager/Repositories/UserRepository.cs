using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace PasswordManager.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);

        public IEnumerable<User> GetAll()
        {
            using var conn = CreateConnection();
            return conn.Query<User>("SELECT Id, Name, IsDefault, [Index] FROM Users");
        }
        public User? GetById(long id)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<User>(
                "SELECT Id, Name, IsDefault, [Index] FROM Users WHERE Id = @Id",
                new { Id = id }
            );
        }
        public User? GetByName(string name)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<User>(
                "SELECT Id, Name, IsDefault, [Index] FROM Users WHERE Name = @Name",
                new { Name = name }
            );
        }
        public void Create(User user)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"INSERT INTO Users (Name, IsDefault, [Index]) VALUES (@Name, @IsDefault, @Index)",
                user
            );
        }
        public void DeleteById(long id)
        {
            using var conn = CreateConnection();

            conn.Execute(
                "DELETE FROM Users WHERE Id = @Id",
                new { Id = id }
            );
        }
        public void DeleteByName(string name)
        {
            using var conn = CreateConnection();

            conn.Execute(
                "DELETE FROM Users WHERE Name = @Name",
                new { Name = name }
            );
        }
        public void Update(User user)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"UPDATE Users
                SET Name = @Name, IsDefault = @IsDefault, [Index] = @Index WHERE Id = @Id",
                user
            );
        }
    }
}
