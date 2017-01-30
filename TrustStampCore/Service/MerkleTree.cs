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
        public static HashAlgorithm Crypt = SHA256.Create();

        public MerkleTree()
        {
        }

        public TreeEntity BuildTree(List<TreeEntity> leafNodes)
        {
            var nodes = new Queue<TreeEntity>(leafNodes);
            // Make Tree
            while (nodes.Count > 1)
            {
                var parents = new Queue<TreeEntity>();
                while (nodes.Count > 0)
                {
                    var first = nodes.Dequeue();
                    var second = nodes.Dequeue();
                    if (second == null)
                    {
                        parents.Enqueue(first);
                        break; // Stop no more nodes!
                    }

                    if (ByteArrayCompare(first.Hash, second.Hash) > 0)
                        parents.Enqueue(new TreeEntity(Crypt, first, second));
                    else
                        parents.Enqueue(new TreeEntity(Crypt, second, first));
                }

                //for (var i = 0; i < nodes.Count; i += 2)
                //{
                //    parents.Add(new TreeEntity(Crypt, nodes[i], (i + 1 < nodes.Count) ? nodes[i + 1] : null));
                //}

                nodes = parents;
            }
            return nodes.FirstOrDefault(); // root
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
                //merkle.Push(new byte[] { 0 }); // right code
                ComputeMerkleTree(node.Left, merkle);
            }

            if (node.Right != null)
            {
                merkle.Push(node.Left.Hash);
                //merkle.Push(new byte[] { 1 }); // left code
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

        //public static byte[] ComputeRoot(HashAlgorithm crypt, byte[] hash, byte[] path)
        //{
        //    for (var i = 0; i < path.Length; i += 32)
        //    {
        //        //var code = path[i];
                
        //        var merkle = new byte[32];
        //        Array.Copy(path, i + 1, merkle, 0, 32);
        //        var code = ByteArrayCompare(hash, merkle);
        //        if(code > 0)
        //            hash = crypt.ComputeHash(crypt.ComputeHash(hash.Concat(merkle).ToArray())) : crypt.ComputeHash(crypt.ComputeHash(Hex.Combine(merkle, hash)));
        //    }
        //    return hash;
        //}


        public byte[] ComputeRoot(TreeEntity node)
        {
            var hash = node.Hash;
            for (var i = 0; i < node.MerkleTree.Length; i += 32)
            {
                //var code = node.MerkleTree[i]; No more code!
                var merkle = new byte[32];
                Array.Copy(node.MerkleTree, i, merkle, 0, 32);
                if (ByteArrayCompare(hash, merkle) > 0)
                    hash = Crypt.ComputeHash(Crypt.ComputeHash(Hex.Combine(hash, merkle)));
                else
                    hash = Crypt.ComputeHash(Crypt.ComputeHash(Hex.Combine(merkle, hash)));
            }
            return hash;
        }

        public void Dispose()
        {
            if (Crypt != null)
                Crypt.Dispose();
        }

        static int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                throw new ApplicationException("Byte arrays has to have the same length");

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] > a2[i])
                    return 1;

                if (a1[i] < a2[i])
                    return -1;
            }

            return 0;
        }

    }
}
