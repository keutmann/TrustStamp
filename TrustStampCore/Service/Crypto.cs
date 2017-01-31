using System;
using System.Text;
using NBitcoin.Crypto;

namespace TrustStampCore.Service
{
    public class Crypto
    {
        public static Func<byte[], byte[]> HashStrategy = (i) => Hashes.RIPEMD160(Hashes.SHA256(i), 0, 32);

        public static byte[] GetHash(string data)
        {
            return HashStrategy(Encoding.Unicode.GetBytes(data));
        }

        public static byte[] GetRandomHash()
        {
            return HashStrategy(Guid.NewGuid().ToByteArray());
        }

    }
}
