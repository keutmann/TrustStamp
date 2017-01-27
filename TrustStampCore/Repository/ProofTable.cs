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

        public static ProofTable Get(SQLiteConnection connection)
        {
            var table = new ProofTable(connection);
            table.CreateIfNotExist();
            return table;
        }

        public void CreateIfNotExist()
        {
            string sql = "create table if not exists Proof "+
                "(id integer primary key," +
                "hash text,"+
                "path text," +
                "timestamp datetime)";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();
        }


        public void Add(JObject batch)
        {
            SQLiteCommand insertSQL = new SQLiteCommand("insert into Proof (hash, path, timestamp) values (@hash,@path,@timestamp)", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@hash", batch["hash"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@path", batch["path"]));
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
                result = NewItem(reader["hash"], reader["path"], reader["timestamp"]);

            return result;
        }

        public JObject NewItem(object hash, object path = null, object timestamp = null)
        {
            return new JObject(
                new JProperty("hash", hash),
                new JProperty("path", path),
                new JProperty("timestamp", timestamp)
                );
        }

    }
}
