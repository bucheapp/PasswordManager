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
                    ServiceInfoId INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    DeletedAt TEXT
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
            return conn.Query<AccountInfo>("SELECT * FROM AccountInfos WHERE DeletedAt IS NULL");
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
                "SELECT * FROM AccountInfos WHERE Name = @Name AND DeletedAt IS NULL",
                new { Name = name }
            );
        }
        public AccountInfo? GetByNameAndServiceInfoId(string name, long serviceInfoId)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<AccountInfo>(
                @"SELECT * FROM AccountInfos WHERE Name = @Name
                AND ServiceInfoId = @ServiceInfoId AND DeletedAt IS NULL",
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
                "SELECT * FROM AccountInfos WHERE ServiceInfoId = @ServiceInfoId AND DeletedAt IS NULL",
                new { ServiceInfoId = serviceInfoId }
            );
        }
        public void Create(AccountInfo accountInfo)
        {
            accountInfo.CreatedAt = DateTime.UtcNow;
            using var conn = CreateConnection();

            accountInfo.Id = (long)conn.QuerySingle<long>(
                    @"INSERT INTO AccountInfos (Name, Password, AuthType, DisplayIndex, ServiceInfoId, CreatedAt, DeletedAt)
                    VALUES (@Name, @Password, @AuthType, @DisplayIndex, @ServiceInfoId, @CreatedAt, @DeletedAt);
                    SELECT last_insert_rowid();",
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
                SET Name = @Name, Password = @Password, AuthType = @AuthType, DisplayIndex = @DisplayIndex, ServiceInfoId = @ServiceInfoId, CreatedAt = @CreatedAt, DeletedAt = @DeletedAt WHERE Id = @Id",
                accountInfo
            );
        }
    }
}
