using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public static class ControlHelper
    {
        public static void TypeText( this TextBox self, string newText )
        {
            string text = self.Text;                                                                                                      
            self.Text = text.Substring( 0, self.SelectionStart ) + newText + text.Substring( self.SelectionStart + self.SelectionLength ); ;
        }

        public static IEnumerable<TreeNode> GetAllNodes( this TreeView self )
        {           
            foreach (TreeNode node in self.Nodes)
            {
                foreach (TreeNode node2 in GetAllNodes( node ))
                {
                    yield return node2;
                }
            }
        }

        public static IEnumerable<TreeNode> GetAllNodes( this TreeNode self )
        {
            yield return self;

            foreach (TreeNode node in self.Nodes)
            {
                foreach (TreeNode node2 in GetAllNodes( node ))
                {
                    yield return node2;
                }
            }
        }
    }
}
