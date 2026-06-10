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
    public class CacheRepository : ICacheRepository
    {
        private readonly string _connectionString;

        public CacheRepository(string connectionString)
        {
            _connectionString = connectionString;

            using var conn = CreateConnection();
            conn.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Caches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Url TEXT,
                    ImageUrl TEXT NOT NULL
                );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
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
                @"INSERT INTO Caches (Url, ImageUrl) VALUES (@Url, @ImageUrl)",
                cache
            );
        }
        public void DeleteAll()
        {
            using var conn = CreateConnection();

            conn.Execute("DELETE FROM Caches");
        }
        public void Update(Cache cache)
        {
            using var conn = CreateConnection();

            conn.Execute(
                @"UPDATE Caches
                SET Url = @Url, ImageUrl = @ImageUrl WHERE Id = @Id",
                cache
            );
        }
    }
}
