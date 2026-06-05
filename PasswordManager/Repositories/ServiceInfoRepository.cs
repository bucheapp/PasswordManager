using Dapper;
using Microsoft.Data.Sqlite;
using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Repositories
{
    public class ServiceInfoRepository : IServiceInfoRepository
    {
        private readonly string _connectionString;
        private readonly string _masterKey;
        public ServiceInfoRepository(string connectionString, string masterKey)
        {
            _connectionString = connectionString;
            _masterKey = masterKey;

            using var conn = CreateConnection();

            string sql = @"
                CREATE TABLE IF NOT EXISTS ServiceInfos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Url TEXT,
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

        public IEnumerable<ServiceInfo> GetAll()
        {
            using var conn = CreateConnection();

            return conn.Query<ServiceInfo>(
                @"SELECT * FROM ServiceInfos
                ORDER BY DisplayIndex");
        }

        public ServiceInfo? GetById(long id)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<ServiceInfo>(
                @"SELECT * FROM ServiceInfos
                WHERE Id = @Id",
                new { Id = id }
            );
        }

        public ServiceInfo? GetByTitle(string title)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<ServiceInfo>(
                @"SELECT * FROM ServiceInfos
                WHERE Title = @Title",
                new { Title = title }
            );
        }

        public ServiceInfo? GetByUrl(string url)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<ServiceInfo>(
                @"SELECT * FROM ServiceInfos
                WHERE Url = @Url",
                new { Url = url }
            );
        }

        public void Create(ServiceInfo serviceInfo)
        {
            using var conn = CreateConnection();
            serviceInfo.Id = (long)conn.QuerySingle<long>(
                @"INSERT INTO ServiceInfos (Title, Url, DisplayIndex)
                    VALUES (@Title, @Url, @DisplayIndex);
                    SELECT last_insert_rowid();",
                serviceInfo
            );
        }

        public void DeleteById(long id)
        {
            using var conn = CreateConnection();
            conn.Execute(
                @"DELETE FROM ServiceInfos WHERE Id = @Id",
                new { Id = id }
            );
        }

        public void Update(ServiceInfo serviceInfo)
        {
            using var conn = CreateConnection();
            conn.Execute(
                @"UPDATE ServiceInfos
                SET Title = @Title, Url = @Url, DisplayIndex = @DisplayIndex
                WHERE Id = @Id",
                serviceInfo
            );
        }
    }
}