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
        public static bool IsMemoryDatabase { get; set; }

        public SQLiteConnection Connection;

        public string Name { get; set; }

        public DBProofTable _proof = null;
        public DBProofTable ProofTable
        {
            get
            {
                return _proof ?? (_proof = new DBProofTable(Connection));
            }
        }

        public DBBatchTable _batch = null;
        public DBBatchTable BatchTable
        {
            get
            {
                return _batch ?? (_batch = new DBBatchTable(Connection));
            }
        }

        public TimeStampDatabase()
        {
            IsMemoryDatabase = true;
        }


        public TimeStampDatabase(string name)
        {
            Name = name;
        }

        public void CreateIfNotExist()
        {
            if (!IsMemoryDatabase && !File.Exists(DatabaseFilename))
                SQLiteConnection.CreateFile(DatabaseFilename);

            ProofTable.CreateIfNotExist();
            BatchTable.CreateIfNotExist();
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
                    MemoryDatabase.CreateIfNotExist();
                }
                return MemoryDatabase;
            }
            else
            {
                var db = new TimeStampDatabase(DatabaseFilename);
                db.OpenConnection();
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
