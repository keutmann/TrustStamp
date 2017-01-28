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
        
        public string Name { get; set; }
        public SQLiteConnection Connection { get; set; }

        public static bool IsMemoryDB
        {
            get
            {
                return DatabaseFilename.IndexOf(":memory:", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public TimeStampDatabase(string name)
        {
            Name = name;
            CreateIfNotExist();
        }

        public void CreateIfNotExist()
        {
            if (!IsMemoryDB && !File.Exists(Name))
                SQLiteConnection.CreateFile(Name);
        }

        public void OpenConnection()
        {
            var sb = new SQLiteConnectionStringBuilder();
            if (IsMemoryDB)
                sb.ConnectionString = DatabaseFilename;
            else
            {

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
            }

            Connection = new SQLiteConnection(sb.ConnectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        public static TimeStampDatabase Open()
        {
            var db = new TimeStampDatabase(DatabaseFilename);
            db.OpenConnection();
            return db;
        }
    }
}
