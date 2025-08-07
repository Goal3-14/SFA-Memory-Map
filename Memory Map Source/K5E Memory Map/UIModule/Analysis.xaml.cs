using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for Analysis.xaml
    /// </summary>
    public partial class Analysis : System.Windows.Controls.UserControl
    {
        public MainWindow _MainWindow;
        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();

        public Analysis()
        {
            InitializeComponent();
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindow.NodeHash.Count() == 0)
            {
                return;
            }
            NodeHash = _MainWindow.NodeHash;


            TreeNode Start = NodeHash.First().Value;
            NodeCount1.Text = "Nodes: " + NodeHash.Count;


            string RootMem = ParentCheck(Start, NodeHash.Count, 0);
            if (RootMem == "Fail")
            {
                RootMem = HuntRoot();
            }
            Root1.Text = "Root: " + RootMem;








            var clonedNodeHash = CloneNodeHash(NodeHash);
            bool cycleExists = false;
            cycles = 0;
            edges = [];
            do
            {
                visiting.Clear();
                visited.Clear();

                if (!clonedNodeHash.TryGetValue(RootMem, out var rootNode))
                {
                    break; // root missing, nothing to check
                }

                cycleExists = CheckCycle(rootNode);

            } while (cycleExists);
            Cycles1.Text = "Cycles Found: " + cycles;




            foreach (TreeEdge edge in edges)
            {
                TreeNode Parent = edge.par();
                TreeNode Child = edge.chi();
                Debug.WriteLine(Parent.Mem + "  " + Child.Mem);


                NodeHash.TryGetValue(Parent.Mem, out Parent);
                NodeHash.TryGetValue(Child.Mem, out Child);


                if (TwoCycle(Parent, Child))
                {
                    Child.RemParent(Parent);
                    Parent.RemChild(Child);
                    cycles--;
                    continue;
                }

                if (Child.Stated == true)
                {
                    Child.RemParent(Parent);
                    Parent.RemChild(Child);
                    cycles--;
                    continue;
                }
            }

            Cycles2title.Visibility = Visibility.Visible;
            Cycles2.Text = "Cycles remaining: " + cycles;

        }


        private bool TwoCycle(TreeNode Parent, TreeNode Child) 
        { 

            if (Parent.Parents.Contains(Child) && Child.Children.Contains(Parent)){
                return true;
            }
            return false;
        }


        private string HuntRoot()
        {
            return "lol";
        }


        private string ParentCheck(TreeNode Node, int Count, int Attempt)
        {
            if (Node.Parents.Count == 0)
            {
                return Node.Mem;
            }
            else
            {
                if (Attempt > Count)
                {
                    return "Fail";
                }
                else
                {
                    return (ParentCheck(Node.Parents.First(), Count, Attempt + 1));
                }
            }
        }


        int cycles;
        HashSet<TreeNode> visiting = new HashSet<TreeNode>();
        HashSet<TreeNode> visited = new HashSet<TreeNode>();
        List<TreeEdge> edges;

        bool CheckCycle(TreeNode node)
        {

            if (visited.Contains(node))
                return false; // already processed, no cycle here

            visiting.Add(node);

            foreach (var child in node.Children)
            {
                if (visiting.Contains(child))
                {
                    Debug.WriteLine($"Cycle detected: {node.Mem} -> {child.Mem}");

                    TreeEdge edge = new TreeEdge(node.Mem, node, child);
                    edges.Add(edge);

                    cycles++;
                    node.RemChild(child);
                    child.RemParent(node);
                    return true; // back edge found, cycle exists
                }

                if (CheckCycle(child))
                    return true; // cycle found deeper in recursion
            }

                visiting.Remove(node);
            visited.Add(node);

            return false;
        }






        Dictionary<string, TreeNode> CloneNodeHash(Dictionary<string, TreeNode> original)
        {
            var clone = new Dictionary<string, TreeNode>();

            // Step 1: Create all nodes (no parents yet)
            foreach (var kvp in original)
            {
                var originalNode = kvp.Value;

                // Create the new TreeNode using its Mem and the clone dict (populates it)
                var clonedNode = new TreeNode(originalNode.Mem, clone); // parent = null
            }

            // Step 2: Rebuild child/parent links (manually)
            foreach (var kvp in original)
            {
                var originalNode = kvp.Value;
                var clonedNode = clone[originalNode.Mem];

                foreach (var child in originalNode.Children)
                {
                    var clonedChild = clone[child.Mem];

                    // Link manually — avoid constructor wiring this time
                    clonedNode.Children.Add(clonedChild);
                    clonedChild.Parents.Add(clonedNode);
                }
            }

            return clone;
        }

    }
}
