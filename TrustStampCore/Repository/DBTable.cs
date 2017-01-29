using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Repository
{
    public class DBTable
    {
        public SQLiteConnection Connection { get; set; }
        public string  TableName { get; set; }

        public bool TableExist()
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='@table' COLLATE NOCASE";
            var command = new SQLiteCommand(sql, Connection);
            command.Parameters.Add(new SQLiteParameter("@table", TableName));
            var reader = command.ExecuteReader();
            return (reader.Read());
        }
    }
}
