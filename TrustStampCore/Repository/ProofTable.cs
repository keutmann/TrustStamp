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
            CreateIfNotExist();
        }

        //public static ProofTable Get(SQLiteConnection connection)
        //{
        //    var table = new ProofTable(connection);
        //    return table;
        //}

        public void CreateIfNotExist()
        {
            if (TableExist())
                return;

            string sql = "create table if not exists Proof "+
                "(id integer primary key," +
                "hash text,"+
                "path text," +
                "partition text," +
                "timestamp datetime)";
            var command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();

            sql = "CREATE UNIQUE INDEX IF NOT EXISTS ProofHash on Proof (hash)";
            command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();
        }


        public void Add(JObject batch)
        {
            SQLiteCommand insertSQL = new SQLiteCommand("insert into Proof (hash, path, partition, timestamp) values (@hash,@path,@partition,@timestamp)", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@hash", batch["hash"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@path", batch["path"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@partition", batch["partition"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@timestamp", batch["timestamp"]));
            insertSQL.ExecuteNonQuery();
        }

        public JObject GetByHash(string hash)
        {
            JObject result = null;
            SQLiteCommand command = new SQLiteCommand("select * from Proof where hash = @hash", Connection);
            command.Parameters.Add(new SQLiteParameter("@hash", hash));
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.Read())
                result.Add(NewItem(reader));

            return result;
        }

        public JArray GetUnprocessed()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT DISTINCT partition FROM Proof WHERE path IS NULL or path ='' ORDER BY partition", Connection);
            return Query(command, (reader) => new JObject(new JProperty("partition", reader["partition"])));
        }

        public JArray Query(SQLiteCommand command, Func<SQLiteDataReader, JObject> newItemMethod = null)
        {
            if (newItemMethod == null)
                newItemMethod = NewItemDefaultReader;

            JArray result = new JArray();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(newItemMethod(reader));
            

            return result;
        }

        public JObject NewItemDefaultReader(SQLiteDataReader reader)
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
