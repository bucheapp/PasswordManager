using Dapper;
using Microsoft.Data.Sqlite;
using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    internal class AccountInfoRepository : IAccountInfoRepository
    {
        private readonly string _connectionString;
        public AccountInfoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);

        public IEnumerable<AccountInfo> GetAll()
        {
            using var conn = CreateConnection();
            return conn.Query<AccountInfo>("SELECT Id, Name FROM AccountInfos");
        }
        public IEnumerable<AccountInfo> GetByUrl(string url)
        {
            using var conn = CreateConnection();

            return conn.Query<AccountInfo>(
                "SELECT Id, Name FROM AccountInfos WHERE Url = @Url",
                new { Url = url }
            );
        }
        public AccountInfo? GetById(long id)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo > (
                "SELECT Id, Name FROM AccountInfos WHERE Id = @Id",
                new { Id = id }
            );
        }
        public AccountInfo? GetByName(string name)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo>(
                "SELECT Id, Name FROM AccountInfos WHERE Name = @Name",
                new { Name = name }
            );
        }
        public void Create(AccountInfo accountInfo)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"INSERT INTO AccountInfos (Url, Name,Password,AuthType, [Index]) VALUES (@Url, @Name,@Password,@AuthType, @Index)",
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
                SET Url = @Url,Name = @Name,Password = @Password,AuthType = @AuthType,[Index] = @Index WHERE Id = @Id",
                accountInfo
            );
        }

    }
}
