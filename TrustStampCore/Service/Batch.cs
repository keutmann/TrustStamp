using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Extensions;
using TrustStampCore.Models;
using NBitcoin;
using TrustStampCore.Workflows;

namespace TrustStampCore.Service
{
    public class Batch : BusinessService
    {
        public static Func<string> PartitionMethod = DefaultPartition;

        public static string DefaultPartition()
        {
            return string.Format("{0}0000", DateTime.Now.ToString("yyyyMMddHH"));
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
            return DB.Batch.GetByPartition(partition);
        }

        public void Process()
        {
            EnsureNewBatchs(); // Make sure to create new Batchs
            ProcessBatchs(); // 
        }

        public void ProcessBatchs()
        {
            var batchs = DB.Batch.GetActive();

            var engine = new WorkflowEngine(batchs);
            engine.Execute();
        }

        public void EnsureNewBatchs()
        {
            var currentPartition = GetCurrentPartition();// current partition snapshot
            var partitions = DB.Proof.GetUnprocessed(currentPartition); // partitions are ordered!

            foreach (var item in partitions)
            {
                var partition = item["partition"].ToString();
                DB.Batch.Ensure(partition);
            }
        }



    }
}
