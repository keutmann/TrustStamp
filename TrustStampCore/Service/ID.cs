using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Extensions;


namespace TrustStampCore.Service
{
    public class ID 
    {
        public static SHA256 Crypt = SHA256.Create();

        public string RawID { get; set; }

        public ID(string id)
        {
            RawID = id;
        }

        public string GetSafeSHA256ID()
        {
            byte[] hash;
            if (string.IsNullOrEmpty(RawID))
                throw new ApplicationException("Value cannot be empty");

            var id = this.RawID.ToUpper(); // Ensure that the same hash value
            if (id.Length != 64)
                throw new ApplicationException("Value is not 64 charators long");

            hash = Hex.ToBytes(id);
            if (hash.Length != 32)
                throw new ApplicationException("Invalid SHA256 hash format, byte length is " + hash.Length);

            return id;
        }

        public static string RandomSHA256Hex()
        {
            var hash =  Crypt.ComputeHash(Encoding.Unicode.GetBytes(Guid.NewGuid().ToString()));
            return hash.ToHex().ToUpper();
        }

        public static string GetSHA256Hex(string data)
        {
            var hash = Crypt.ComputeHash(Encoding.Unicode.GetBytes(data));
            return hash.ToHex().ToUpper();
        }

    }

}
