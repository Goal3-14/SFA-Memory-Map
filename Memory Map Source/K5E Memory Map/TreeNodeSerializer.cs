using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace K5E_Memory_Map
{
    public static class TreeNodeSerializer
    {
        public static void SaveToFile(string filePath, Dictionary<string, TreeNode> nodeHash)
        {
            var serializableNodes = nodeHash.Values.ToDictionary(
                node => node.Mem,
                node => new SerializableTreeNode(node)
            );

            string json = JsonSerializer.Serialize(serializableNodes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static Dictionary<string, TreeNode> LoadFromFile(string filePath, Dictionary<string,TreeNode> nodeHash)
        {

            string json = File.ReadAllText(filePath);
            var serializableNodes = JsonSerializer.Deserialize<Dictionary<string, SerializableTreeNode>>(json);
            
            nodeHash.Clear();

            //Debug.WriteLine("-- 1 --");

            if (serializableNodes == null)
                throw new Exception("Failed to deserialize nodes.");

            nodeHash.Clear();

            // First pass: create all nodes
            foreach (var entry in serializableNodes)
            {
                //Debug.WriteLine("-- 2 --");
                var node = new TreeNode(entry.Key, nodeHash, null)
                {
                    Tag = entry.Value.Tag,
                    Text = entry.Value.Text,
                    TagText = entry.Value.TagText,
                    Colour = entry.Value.Colour,
                    MountCoords = entry.Value.MountCoords,
                    PracPath = entry.Value.PracPath
                };


            }

            // Second pass: restore relationships
            foreach (var entry in serializableNodes)
            {
                var node = nodeHash[entry.Key];
                foreach (string parentId in entry.Value.ParentIds)
                {
                    if (nodeHash.TryGetValue(parentId, out TreeNode parent))
                    {
                        node.AddParent(parent);
                        parent.AddChild(node);
                    }
                }
            }

            return nodeHash;
        }

        private class SerializableTreeNode
        {
            public string Mem { get; set; }
            public List<string> ParentIds { get; set; }
            public List<string> ChildIds { get; set; }
            public int? Tag { get; set; }
            public string? Text { get; set; }
            public string? TagText { get; set; }
            public string Colour { get; set; }
            public List<float[]> MountCoords { get; set; }
            public bool? PracPath { get; set; }

            public SerializableTreeNode() { }

            public SerializableTreeNode(TreeNode node)
            {
                Mem = node.Mem;
                ParentIds = node.Parents.Select(p => p.Mem).ToList();
                ChildIds = node.Children.Select(c => c.Mem).ToList();
                Tag = node.Tag;
                Text = node.Text;
                TagText = node.TagText;
                Colour = node.Colour;
                MountCoords = node.MountCoords;
                PracPath = node.PracPath;
            }
        }
    }
}
