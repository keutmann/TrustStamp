using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using TrustStampCore.Extensions;


namespace TrustStampCore.Models
{
    public class TreeEntity
    {
        public byte[] Source { get; set; }
        public byte[] Hash { get; set; }
        public byte[] MerkleTree { get; set; }

        public TreeEntity Left { get; set; }
        public TreeEntity Right { get; set; }
        public TreeEntity Parent { get; set; }

        public bool IsRight { get; set; }

        public JObject Entity { get; set; }

        public TreeEntity(HashAlgorithm crypt, TreeEntity left, TreeEntity right)
        {
            Left = left;
            Left.Parent = this;

            Right = right ?? left;
            Right.Parent = this;
            Right.IsRight = true;
            Hash = crypt.ComputeHash(crypt.ComputeHash(Left.Hash.Concat(Right.Hash).ToArray()));
        }

        public TreeEntity(HashAlgorithm crypt)
        {
            Guid guid = Guid.NewGuid();
            Source = guid.ToByteArray();
            Hash = crypt.ComputeHash(Source);
        }

        public TreeEntity(HashAlgorithm crypt, int index)
        {
            Source = new byte[] { (byte)index };
            Hash = crypt.ComputeHash(Source);
        }

        public TreeEntity(string hash)
        {
            Hash = hash.ToBytes();
        }

        public TreeEntity(byte[] hash)
        {
            Hash = hash;
        }

        public TreeEntity(JObject entity) : this(entity["hash"].ToString())
        {
            Entity = entity;
        }
    }

}
