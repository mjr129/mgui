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
    }
}
