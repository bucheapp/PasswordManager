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
                    Name TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    AuthType TEXT NOT NULL,
                    DisplayIndex INTEGER NOT NULL DEFAULT 0,
                    ServiceInfoId INTEGER NOT NULL
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
            return conn.Query<AccountInfo>("SELECT * FROM AccountInfos");
        }
        public AccountInfo? GetById(long id)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo > (
                "SELECT * FROM AccountInfos WHERE Id = @Id",
                new { Id = id }
            );
        }
        public AccountInfo? GetByName(string name)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo>(
                "SELECT * FROM AccountInfos WHERE Name = @Name",
                new { Name = name }
            );
        }
        public AccountInfo? GetByNameAndServiceInfoId(string name, long serviceInfoId)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo>(
                @"SELECT * FROM AccountInfos WHERE Name = @Name
                AND ServiceInfoId = @ServiceInfoId",
                new
                {
                    Name = name,
                    ServiceInfoId = serviceInfoId
                });
        }
        public IEnumerable<AccountInfo> GetByServiceInfoId(long serviceInfoId)
        {
            using var conn = CreateConnection();
            return conn.Query<AccountInfo>(
                "SELECT * FROM AccountInfos WHERE ServiceInfoId = @ServiceInfoId",
                new { ServiceInfoId = serviceInfoId }
            );
        }
        public void Create(AccountInfo accountInfo)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"INSERT INTO AccountInfos (Name, Password, AuthType, DisplayIndex, ServiceInfoId) VALUES (@Name, @Password, @AuthType, @DisplayIndex, @ServiceInfoId)",
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
        public void Update(AccountInfo accountInfo)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"UPDATE AccountInfos
                SET Name = @Name, Password = @Password, AuthType = @AuthType, DisplayIndex = @DisplayIndex, ServiceInfoId = @ServiceInfoId WHERE Id = @Id",
                accountInfo
            );
        }
    }
}
