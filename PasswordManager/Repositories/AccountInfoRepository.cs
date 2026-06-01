using Dapper;
using Microsoft.Data.Sqlite;
using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly string _connectionString;
        private readonly string _masterKey;
        public AccountInfoRepository(string connectionString,string masterKey)
        {
            _connectionString = connectionString;
            _masterKey = masterKey;

            using var conn = CreateConnection();

            string sql = @"
                CREATE TABLE IF NOT EXISTS AccountInfos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Url TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    AuthType TEXT NOT NULL,
                    DisplayIndex INTEGER NOT NULL DEFAULT 0
                );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection CreateConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();

            conn.Execute($"PRAGMA key = '{_masterKey}';");

            return conn;
        }

        public IEnumerable<AccountInfo> GetAll()
        {
            using var conn = CreateConnection();
            return conn.Query<AccountInfo>("SELECT Id, Url, Name, Title, Password, AuthType, DisplayIndex FROM AccountInfos");
        }
        public IEnumerable<AccountInfo> GetByUrl(string url)
        {
            using var conn = CreateConnection();

            return conn.Query<AccountInfo>(
                "SELECT Id, Url, Name, Title, Password, AuthType, DisplayIndex FROM AccountInfos WHERE Url = @Url",
                new { Url = url }
            );
        }
        public AccountInfo? GetById(long id)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo > (
                "SELECT Id, Url, Name, Title, Password, AuthType, DisplayIndex FROM AccountInfos WHERE Id = @Id",
                new { Id = id }
            );
        }
        public AccountInfo? GetByName(string name)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo>(
                "SELECT Id, Url, Name, Title, Password, AuthType, DisplayIndex FROM AccountInfos WHERE Name = @Name",
                new { Name = name }
            );
        }
        public void Create(AccountInfo accountInfo)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"INSERT INTO AccountInfos (Url, Name, Title, Password, AuthType, DisplayIndex) VALUES (@Url, @Name, @Title, @Password, @AuthType, @DisplayIndex)",
                accountInfo
            );
        }
        public void DeleteById(long id)
        {
            using var conn = CreateConnection();

            conn.Execute(
                "DELETE FROM AccountInfos WHERE Id = @Id",
                new { Id = id }
            );
        }
        public void DeleteByUrl(string url)
        {
            using var conn = CreateConnection();

            conn.Execute(
                "DELETE FROM AccountInfos WHERE Url = @Url",
                new { Url = url }
            );
        }
        public void Update(AccountInfo accountInfo)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"UPDATE AccountInfos
                SET Url = @Url,Name = @Name,Title = @Title,Password = @Password,AuthType = @AuthType,DisplayIndex = @DisplayIndex WHERE Id = @Id",
                accountInfo
            );
        }

    }
}
