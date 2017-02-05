using Newtonsoft.Json.Linq;
using System;
using TrustStampCore.Repository;
using TrustStampCore.Workflows;
using TrustStampCore.Extensions;


namespace TrustStampCore.Service
{
    public class Batch : BusinessService
    {
        public static Func<string> PartitionMethod = DefaultPartition;

        public static string DefaultPartition()
        {
            return GetPartition(DateTime.Now);
        } 

        public static string GetPartition(DateTime datetime)
        {
            return datetime.ToString( App.Config["partition"].ToStringValue("yyyyMMddhh0000"));
        }

        public static string GetCurrentPartition()
        {
            return PartitionMethod();
        }

        public static Batch OpenWithDatabase()
        {
            var p = new Batch(TimeStampDatabase.Open());
            p.LocalDB = true;
            return p;
        }

        public Batch(TimeStampDatabase db) : base(db)
        {
        }

        public JObject Get(string partition)
        {
            return DB.BatchTable.GetByPartition(partition);
        }

        public void Process()
        {
            EnsureNewBatchs(); // Make sure to create new Batchs
            ProcessBatchs(); // 
        }

        protected void ProcessBatchs()
        {
            var batchs = DB.BatchTable.GetActive();

            var engine = new WorkflowEngine(batchs);
            engine.Execute();
        }

        public void EnsureNewBatchs()
        {
            var currentPartition = GetCurrentPartition();// current partition snapshot
            var partitions = DB.ProofTable.GetUnprocessedPartitions(currentPartition); // partitions are ordered!

            foreach (var item in partitions)
            {
                var partition = item["partition"].ToString();
                DB.BatchTable.Ensure(partition);
            }
        }



    }
}
