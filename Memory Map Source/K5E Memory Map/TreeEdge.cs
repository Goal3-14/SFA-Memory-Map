using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace K5E_Memory_Map
{


    public class TreeEdge
    {
        
        private TreeNode Parent { get; set; }
        private TreeNode Child { get; set; }
        

        public TreeEdge(string mem, TreeNode parent, TreeNode child )
        {
            
            Parent = parent;
            Child = child;

        }

        public TreeNode par()
        {
            return this.Parent;
        }

        public TreeNode chi()
        {
            return this.Child;
        }


        public void DelEdge(int? tag)
        {
            Parent.RemChild(Child);
            Child.RemParent(Parent);
        }

        









    }
}
