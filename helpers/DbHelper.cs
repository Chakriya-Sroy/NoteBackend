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

            // Switch to master
            builder.InitialCatalog = "master";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync($@"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
                    CREATE DATABASE [{dbName}]
                ");
            }

            // Correct script path
            string scriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "init-db.sql"
            );

            if (!File.Exists(scriptPath))
            {
                Console.WriteLine("❌ init-db.sql NOT FOUND");
                return;
            }

            var script = await File.ReadAllTextAsync(scriptPath);

            using var dbConn = new SqlConnection(connectionString);
            await dbConn.OpenAsync();
            await dbConn.ExecuteAsync(script);

            Console.WriteLine("✅ Database & Notes table created");
        }
    }
}
