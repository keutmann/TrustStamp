using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Repository
{
    public class ProofTable : DBTable
    {
        public ProofTable(SQLiteConnection connection, string tableName = "Proof")
        {
            Connection = connection;
            TableName = tableName;
        }

        public void CreateIfNotExist()
        {
            if (TableExist())
                return;

            string sql = "CREATE TABLE IF NOT EXISTS Proof "+
                "(id INTEGER PRIMARY KEY," +
                "hash BLOB," +
                "path BLOB," +
                "partition TEXT," +
                "timestamp DATETIME)";
            var command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand("CREATE INDEX IF NOT EXISTS ProofHash ON Proof (hash)", Connection);
            command.ExecuteNonQuery();
        }


        public void Add(JObject proof)
        {
            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO Proof (hash, path, partition, timestamp) VALUES (@hash,@path,@partition,@timestamp)", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@hash", proof["hash"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@path", proof["path"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@partition", proof["partition"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@timestamp", (DateTime)proof["timestamp"]));
            insertSQL.ExecuteNonQuery();
        }

        public void UpdatePath(JObject proof)
        {
            UpdatePath((byte[])proof["hash"], (byte[])proof["path"]);
        }

        public void UpdatePath(byte[] hash, byte[] path)
        {
            var insertSQL = new SQLiteCommand("UPDATE Proof SET path = @path WHERE hash = @hash", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@hash", hash));
            insertSQL.Parameters.Add(new SQLiteParameter("@path", path));
            insertSQL.ExecuteNonQuery();
        }

        public JObject GetByHash(byte[] hash)
        {
            var command = new SQLiteCommand("select * from Proof where hash = @hash LIMIT 1", Connection);
            command.Parameters.Add(new SQLiteParameter("@hash", hash));
            return (JObject)Query(command, NewItem).FirstOrDefault();
        }

        public JArray GetUnprocessed()
        {
            var command = new SQLiteCommand("SELECT DISTINCT partition FROM Proof WHERE path IS NULL or path ='' ORDER BY partition", Connection);
            return Query(command, (reader) => new JObject(new JProperty("partition", reader["partition"])));
        }

        public JArray GetByPartition(string partition)
        {
            var command = new SQLiteCommand("SELECT * FROM Proof WHERE partition = @partition", Connection);
            command.Parameters.Add(new SQLiteParameter("@partition", partition));
            return Query(command, NewItem);
        }

        public void DropTable()
        {
            var command = new SQLiteCommand("DROP TABLE Proof", Connection);
            command.ExecuteNonQuery();
        }

        public JObject NewItem(SQLiteDataReader reader)
        {
            return NewItem(reader["hash"], reader["path"], reader["partition"], reader["timestamp"]);
        }


        public JObject NewItem(object hash, object path = null, object partition = null, object timestamp = null)
        {
            return new JObject(
                new JProperty("hash", hash),
                new JProperty("path", path),
                new JProperty("partition", partition),
                new JProperty("timestamp", timestamp)
                );
        }

    }
}
