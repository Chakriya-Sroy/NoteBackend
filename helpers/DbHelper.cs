using Dapper;
using Microsoft.Data.SqlClient;

namespace NoteBackend.Helpers
{
    public static class DbHelper
    {
        public static async Task SetupDatabase(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var dbName = builder.InitialCatalog;

            // Switch to master to ensure DB exists
            builder.InitialCatalog = "master";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                await conn.ExecuteAsync($"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') CREATE DATABASE [{dbName}]");
            }

            // Path to your SQL script
            string scriptPath = Path.Combine(AppContext.BaseDirectory, "init-db.sql");

            if (File.Exists(scriptPath))
            {
                var script = await File.ReadAllTextAsync(scriptPath);

                using var dbConn = new SqlConnection(connectionString);


                await dbConn.ExecuteAsync("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'Notes') DROP TABLE Notes");

                await dbConn.ExecuteAsync(script);
            }
        }
    }
}