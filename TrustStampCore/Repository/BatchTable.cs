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
    public class BatchTable : DBTable
    {

        public BatchTable(SQLiteConnection connection, string tableName = "Batch")
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
                "lastupdate DATETIME"+
                ")";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand("CREATE UNIQUE INDEX IF NOT EXISTS BatchRoot ON Batch (root)", Connection);
            command.ExecuteNonQuery();
        }


        public void Add(JObject batch)
        {
            var insertSQL = new SQLiteCommand("INSERT INTO Batch (partition, root, state,tx, lastupdate) VALUES (@partition,@root,@state,@tx,@lastupdate)", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@partition", batch["partition"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@root", batch["root"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@state", batch["state"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@tx", batch["tx"].ToString()));
            insertSQL.Parameters.Add(new SQLiteParameter("@lastupdate", DateTime.Now));
            insertSQL.ExecuteNonQuery();
        }

        public void Update(JObject batch)
        {
            var insertSQL = new SQLiteCommand("UPDATE Batch SET root = @root, state = @state, tx = @tx, lastupdate = @lastupdate WHERE partition = @partition", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@partition", batch["partition"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@root", batch["root"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@state", batch["state"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@tx", batch["tx"].ToString()));
            insertSQL.Parameters.Add(new SQLiteParameter("@lastupdate", DateTime.Now));
            insertSQL.ExecuteNonQuery();
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

            item = new JObject(
                    new JProperty("partition", partition),
                    new JProperty("root", null),
                    new JProperty("state", BatchState.New),
                    new JProperty("tx", new JArray())
                    );

            Add(item);

            return item;
        }

        public JArray GetByState(string state)
        {
            var command = new SQLiteCommand("SELECT * FROM Batch WHERE state = @state ORDER BY partition COLLATE NOCASE", Connection);
            command.Parameters.Add(new SQLiteParameter("@state", state));
            return Query(command, NewItem);
        }

        public JObject NewItem(SQLiteDataReader reader)
        {
            return new JObject(
                    new JProperty("partition", reader["partition"]),
                    new JProperty("root", reader["root"]),
                    new JProperty("state", reader["state"]),
                    new JProperty("tx", JArray.Parse(reader["tx"].ToString())),
                    new JProperty("lastupdate", reader["lastupdate"])
                    );
        }

    }
}

