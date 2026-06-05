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
    public class CacheRepository : ICacheRepository
    {
        private readonly string _connectionString;

        public CacheRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);
        public IEnumerable<Cache> GetAll()
        {
            using var conn = CreateConnection();
            return conn.Query<Cache>("SELECT * FROM Caches");
        }
        public Cache? GetByUrl(string url)
        {
            using var conn = CreateConnection();

            return conn.QueryFirstOrDefault<Cache>(
                "SELECT * FROM Caches WHERE Url = @Url",
                new { Url = url }
            );
        }
       public void Create(Cache cache)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"INSERT INTO Caches (Url, ImageUrl, Title) VALUES (@Url, @ImageUrl, @Title)",
                cache
            );
        }
        public void DeleteAll()
        {
            using var conn = CreateConnection();

            conn.Execute("DELETE FROM Caches");
        }
    }
}
