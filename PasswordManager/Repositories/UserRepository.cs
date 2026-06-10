using Dapper;
using Microsoft.Data.Sqlite;
using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PasswordManager.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;

            using var conn = CreateConnection();
            conn.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    DisplayIndex INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    DeletedAt TEXT
                );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection CreateConnection() => new(_connectionString);

        public IEnumerable<User> GetAll()
        {
            using var conn = CreateConnection();
            return conn.Query<User>("SELECT * FROM Users WHERE DeletedAt IS NULL");
        }
        public User? GetById(long id)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Id = @Id AND DeletedAt IS NULL",
                new { Id = id }
            );
        }
        public User? GetByName(string name)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Name = @Name AND DeletedAt IS NULL",
                new { Name = name }
            );
        }
        public void Create(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            using var conn = CreateConnection();

            user.Id = (long)conn.QuerySingle<long>(
                @"INSERT INTO Users (Name, DisplayIndex, CreatedAt, DeletedAt)
                  VALUES (@Name, @DisplayIndex, @CreatedAt, @DeletedAt);
                  SELECT last_insert_rowid();",
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
                SET Name = @Name, DisplayIndex = @DisplayIndex, CreatedAt = @CreatedAt, DeletedAt = @DeletedAt WHERE Id = @Id",
                user
            );
        }
    }
}
