using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class BatchState 
    {
        public const string New = "New";
        public const string BuildMerkle = "BuildMerkle";
        public const string BuildMerkleDone = "BuildMerkleDone";
        public const string Timestamping = "Timestramping";
        public const string TimeStampDone = "TimestrampDone";
        public const string TimeStampVerified = "TimestampVerified";
        public const string Failed = "Failed";
    }
}
