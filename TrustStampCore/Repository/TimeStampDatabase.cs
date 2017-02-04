using System;
using System.Data.SQLite;
using System.IO;
using TrustStampCore.Service;
using TrustStampCore.Extensions;


namespace TrustStampCore.Repository
{
    public class TimeStampDatabase : IDisposable
    {
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
        }


        public TimeStampDatabase(string name)
        {
            Name = name;
        }

        public void CreateIfNotExist()
        {
            if (!IsMemoryDatabase && !File.Exists(Connection.FileName))
                SQLiteConnection.CreateFile(Connection.FileName);

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

            if(!string.IsNullOrEmpty((string)App.Config["dbconnectionstring"]))
            {
                Connection = new SQLiteConnection((string)App.Config["dbconnectionstring"]);
                Connection.Open();
                return Connection;
            }

            var dbFilename = (!string.IsNullOrEmpty((string)App.Config["dbfilename"])) ? (string)App.Config["dbfilename"] : Name;
            if (!string.IsNullOrEmpty(dbFilename))
            {
                var sb = new SQLiteConnectionStringBuilder();

                sb.DataSource = (string)App.Config["dbfilename"];
                var dbObject = App.Config["database"];
                sb.Flags = SQLiteConnectionFlags.UseConnectionPool;
                //tt.NoSharedFlags = false;

                sb.JournalMode = (SQLiteJournalModeEnum)dbObject["journalmode"].ToInteger((int)SQLiteJournalModeEnum.Default);
                sb.Pooling = dbObject["pooling"].ToBoolean(true);
                sb.ReadOnly = dbObject["readonly"].ToBoolean(false);
                sb.Add("cache", dbObject["cache"].ToStringOrDefault("shared"));
                sb.Add("Compress", dbObject["compress"].ToStringOrDefault("False"));
                sb.SyncMode = (SynchronizationModes)dbObject["syncmode"].ToInteger((int)SynchronizationModes.Normal);

                //sb.DefaultIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
                //tt.DefaultDbType = System.Data.DbType.
                //var dd = new SQLiteConnection(;

                Connection = new SQLiteConnection(sb.ConnectionString);
                Connection.Open();
                return Connection;
            }



            throw new ApplicationException("Not database connection found");
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
                var db = new TimeStampDatabase();
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
