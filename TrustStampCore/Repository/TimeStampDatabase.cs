using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Repository
{
    public class TimeStampDatabase : IDisposable
    {
        public static string DatabaseFilename = "test.db";

        public static string MemoryConnectionString = "Data Source=:memory:;Version=3;";
        public static TimeStampDatabase MemoryDatabase;

        public SQLiteConnection Connection;

        public string Name { get; set; }

        public static bool IsMemoryDatabase {get;set;}

        public TimeStampDatabase()
        {
            IsMemoryDatabase = true;
        }


        public TimeStampDatabase(string name)
        {
            Name = name;
            CreateIfNotExist();
        }

        public void CreateIfNotExist()
        {
            if (!IsMemoryDatabase && !File.Exists(Name))
                SQLiteConnection.CreateFile(Name);
        }

        public SQLiteConnection OpenConnection()
        {
            if(IsMemoryDatabase)
            {
                Connection = new SQLiteConnection(MemoryConnectionString);
                Connection.Open();
                return Connection;
            }

            var sb = new SQLiteConnectionStringBuilder();
            
            sb.DataSource = Name;
            sb.Flags = SQLiteConnectionFlags.UseConnectionPool;
            //tt.JournalMode = SQLiteJournalModeEnum.Default;
            //tt.NoSharedFlags = false;
            sb.Pooling = true;
            sb.ReadOnly = false;
            sb.Add("cache", "shared");
            //tt.Add("Compress", "True");
            //tt.SyncMode = SynchronizationModes.Normal;
            //tt.DefaultIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
            //tt.DefaultDbType = System.Data.DbType.
            //var dd = new SQLiteConnection(;

            Connection = new SQLiteConnection(sb.ConnectionString);
            Connection.Open();
            return Connection;
        }

        public static TimeStampDatabase Open()
        {
            if (IsMemoryDatabase)
            {
                if (MemoryDatabase == null) { 
                    MemoryDatabase = new TimeStampDatabase();
                    MemoryDatabase.OpenConnection();
                }
                return MemoryDatabase;
            }
            else
            {
                var db = new TimeStampDatabase(DatabaseFilename);
                return db;
            }
        }

        public void Dispose()
        {
            if (!IsMemoryDatabase)
                Connection.Dispose();
        }
    }
}
