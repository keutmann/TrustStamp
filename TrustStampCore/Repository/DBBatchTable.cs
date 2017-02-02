using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;

namespace TrustStampCore.Repository
{
    public class DBBatchTable : DBTable
    {

        public DBBatchTable(SQLiteConnection connection, string tableName = "Batch")
        {
            Connection = connection;
            TableName = tableName;
        }

        public void CreateIfNotExist()
        {
            if (TableExist())
                return;

            string sql = "CREATE TABLE IF NOT EXISTS Batch "+
                "(" +
                "partition TEXT PRIMARY KEY," +
                "root BLOB," +
                "state TEXT,"+
                "tx TEXT,"+
                "active INTEGER," +
                "log TEXT" +
                ")";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand("CREATE INDEX IF NOT EXISTS BatchRoot ON Batch (root)", Connection);
            command.ExecuteNonQuery();
        }

        public JObject AddDefault(string partition)
        {
            var item = new JObject(
                new JProperty("partition", partition),
                new JProperty("root", null),
                new JProperty("state", new JObject()),
                new JProperty("tx", new JArray()),
                new JProperty("active", 1),
                new JProperty("log", new JArray())
                );

            Add(item);

            return item;
        }

        public int Add(JObject batch)
        {
            var command = new SQLiteCommand("INSERT INTO Batch (partition, root, state,tx, active, log) VALUES (@partition,@root,@state,@tx,@active,@log)", Connection);
            command.Parameters.Add(new SQLiteParameter("@partition", batch["partition"]));
            command.Parameters.Add(new SQLiteParameter("@root", batch["root"]));
            command.Parameters.Add(new SQLiteParameter("@state", batch["state"]));
            command.Parameters.Add(new SQLiteParameter("@tx", batch["tx"].ToString()));
            command.Parameters.Add(new SQLiteParameter("@active", batch["active"]));
            command.Parameters.Add(new SQLiteParameter("@log", batch["log"]));
            return command.ExecuteNonQuery();
        }

        public int Update(JObject batch)
        {
            var command = new SQLiteCommand("UPDATE Batch SET root = @root, state = @state, tx = @tx, active = @active, log = @log WHERE partition = @partition", Connection);
            command.Parameters.Add(new SQLiteParameter("@partition", batch["partition"]));
            command.Parameters.Add(new SQLiteParameter("@root", batch["root"]));
            command.Parameters.Add(new SQLiteParameter("@state", batch["state"]));
            command.Parameters.Add(new SQLiteParameter("@tx", batch["tx"].ToString()));
            command.Parameters.Add(new SQLiteParameter("@active", batch["active"]));
            command.Parameters.Add(new SQLiteParameter("@log", batch["log"]));
            return command.ExecuteNonQuery();
        }

        public int Count()
        {
            var command = new SQLiteCommand("SELECT count(*) FROM Batch", Connection);
            var result = Query(command, (reader) => new JObject(new JProperty("count", reader[0]))).FirstOrDefault();
            return (int)result["count"];
        }


        public int DropTable()
        {
            var command = new SQLiteCommand("DROP TABLE Batch", Connection);
            return command.ExecuteNonQuery();
        }

        public JArray Select(int count)
        {
            var command = new SQLiteCommand("SELECT * FROM Batch ORDER BY partition DESC LIMIT @count", Connection);
            command.Parameters.Add(new SQLiteParameter("@count", count));
            return Query(command, NewItem);
        }


        public JArray GetActive()
        {
            var command = new SQLiteCommand("SELECT * FROM Batch WHERE active = 1 ORDER BY partition DESC", Connection);
            return Query(command, NewItem);
        }


        public JObject GetByPartition(string partition)
        {
            var command = new SQLiteCommand("SELECT * FROM Batch WHERE partition = @partition LIMIT 1", Connection);
            command.Parameters.Add(new SQLiteParameter("@partition", partition));
            return (JObject)Query(command, NewItem).FirstOrDefault();
        }

        public JObject Ensure(string partition)
        {
            var item = GetByPartition(partition);
            if (item != null)
                return item;

            return AddDefault(partition);
        }

        public JObject NewItem(SQLiteDataReader reader)
        {
            return new JObject(
                    new JProperty("partition", reader["partition"]),
                    new JProperty("root", reader["root"]),
                    new JProperty("state", reader["state"]),
                    new JProperty("tx", !string.IsNullOrEmpty((string)reader["tx"]) ? JArray.Parse((string)reader["tx"]) : new JArray()),
                    new JProperty("active", reader["active"]),
                    new JProperty("log", !string.IsNullOrEmpty((string)reader["log"]) ? JArray.Parse((string)reader["log"]) : new JArray())
                    );
        }

        

    }
}

