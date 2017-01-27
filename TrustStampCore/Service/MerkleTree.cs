using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TrustStampCore.Models;
using TrustStampCore.Extensions;

namespace TrustStampCore.Service
{
    public class MerkleTree : IDisposable
    {
        public HashAlgorithm Crypt { get; set; }

        public MerkleTree()
        {
            Crypt = SHA256Managed.Create();

        }
        public TreeEntity BuildTree(List<TreeEntity> leafNodes)
        {
            TreeEntity root = null;
            var nodes = leafNodes;
            // Make Tree
            while (nodes.Count > 1)
            {
                var parents = new List<TreeEntity>();
                for (var i = 0; i < nodes.Count; i += 2)
                {
                    parents.Add(new TreeEntity(Crypt, nodes[i], (i + 1 < nodes.Count) ? nodes[i + 1] : null));
                }

                nodes = parents;
            }
            root = nodes[0];
            return root;
        }

        public void ComputeMerkleTree(TreeEntity root)
        {
            var merkle = new Stack<byte[]>();
            ComputeMerkleTree(root, merkle);
        }

        public void ComputeMerkleTree(TreeEntity node, Stack<byte[]> merkle)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                var tree = new List<byte>();
                foreach (var v in merkle)
                    tree.AddRange(v);

                node.MerkleTree = tree.ToArray();
            }

            if (node.Left != null)
            {
                merkle.Push(node.Right.Hash);
                merkle.Push(new byte[] { 0 }); // right code
                ComputeMerkleTree(node.Left, merkle);
            }

            if (node.Right != null)
            {
                merkle.Push(node.Left.Hash);
                merkle.Push(new byte[] { 1 }); // left code
                ComputeMerkleTree(node.Right, merkle);
            }

            if (merkle.Count > 0)
            {
                merkle.Pop(); // Pop left/right code
                merkle.Pop(); // Pop hash
            }
            return;
        }


        //public byte[] ComputeTree(Entity leaf)
        //{
        //    var hash = leaf.Hash;
        //    var node = leaf;

        //    while(node != null && node.Parent != null)
        //    {
        //        hash = (!node.IsRight) ? Crypt.ComputeHash(Crypt.ComputeHash(ByteArrayExtensions.Combine(hash, node.Parent.Right.Hash))) : Crypt.ComputeHash(Crypt.ComputeHash(ByteArrayExtensions.Combine(node.Parent.Left.Hash, hash)));
        //        node = node.Parent;

        //    }
        //    return hash;
        //}

        public static byte[] ComputeRoot(HashAlgorithm crypt, byte[] hash, byte[] path)
        {
            for (var i = 0; i < path.Length; i += 33)
            {

                var code = path[i];
                var merkle = new byte[32];
                Array.Copy(path, i + 1, merkle, 0, 32);

                hash = (code == 0) ? crypt.ComputeHash(crypt.ComputeHash(hash.Concat(merkle).ToArray())) : crypt.ComputeHash(crypt.ComputeHash(Hex.Combine(merkle, hash)));
            }
            return hash;
        }


        public byte[] ComputeRoot(TreeEntity node)
        {
            var hash = node.Hash;
            for (var i = 0; i < node.MerkleTree.Length; i += 33)
            {

                var code = node.MerkleTree[i];
                var merkle = new byte[32];
                Array.Copy(node.MerkleTree, i + 1, merkle, 0, 32);

                hash = (code == 0) ? Crypt.ComputeHash(Crypt.ComputeHash(Hex.Combine(hash, merkle))) : Crypt.ComputeHash(Crypt.ComputeHash(Hex.Combine(merkle, hash)));
            }
            return hash;
        }

        public void Dispose()
        {
            if (Crypt != null)
                Crypt.Dispose();
        }
    }
}
