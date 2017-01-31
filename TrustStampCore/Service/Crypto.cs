using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class Crypto
    {
        public static Func<byte[], byte[]> HashStrategy = (i) => Hashes.RIPEMD160(Hashes.Hash256(i).ToBytes(), 0, 32);
    }
}
