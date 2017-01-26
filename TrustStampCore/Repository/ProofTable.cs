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
                "root nvarchar(256),"+
                "state tinyint,"+
                "timestamp datetime default current_timestamp)";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();
        }

        public JArray GetUnprocessed()
        {
            JArray array = new JArray();
            var sql = "select * from Proof where state = 1 order by Timestamp";
            var command = new SQLiteCommand(sql, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                array.Add(new JObject(
                    new JProperty("id", reader["id"]),
                    new JProperty("root", reader["root"]),
                    new JProperty("state", reader["state"]),
                    new JProperty("timestamp", reader["timestamp"])
                    ));
            }

            return array;
        }

        public void Add(JObject batch)
        {
            SQLiteCommand insertSQL = new SQLiteCommand("insert into Proof (root, state) values (@root,@state)", Connection);
            insertSQL.Parameters.Add(new SQLiteParameter("@root", batch["root"]));
            insertSQL.Parameters.Add(new SQLiteParameter("@state", batch["state"]));
            insertSQL.ExecuteNonQuery();
        }
    }
}
