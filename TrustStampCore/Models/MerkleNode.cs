using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using TrustStampCore.Extensions;


namespace TrustStampCore.Models
{
    public class MerkleNode
    {
        public byte[] Source { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Path { get; set; }

        public MerkleNode Left { get; set; }
        public MerkleNode Right { get; set; }
        public MerkleNode Parent { get; set; }

        public JObject Proof { get; set; }

        public MerkleNode(JObject proof) : this((byte[])proof["hash"])
        {
            Proof = proof;
        }

        public MerkleNode(byte[] hash)
        {
            Hash = hash;
        }

        public MerkleNode(byte[] hash, MerkleNode left, MerkleNode right)
        {
            Hash = hash;

            Left = left;
            Left.Parent = this;

            Right = right ?? left;
            Right.Parent = this;
            //Right.IsRight = true;
        }

    }

}
